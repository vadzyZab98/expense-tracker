using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db) => _db = db;

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub")!);

    public record ExpenseRequest(decimal Amount, string Description, DateTime Date, int CategoryId);

    // GET /api/expenses
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var expenses = await _db.Expenses
            .Where(e => e.UserId == CurrentUserId)
            .Include(e => e.Category)
            .OrderByDescending(e => e.Date)
            .Select(e => new
            {
                e.Id,
                e.Amount,
                e.Description,
                e.Date,
                e.CategoryId,
                Category = new { e.Category.Id, e.Category.Name, e.Category.Color }
            })
            .ToListAsync();

        return Ok(expenses);
    }

    // GET /api/expenses/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _db.Expenses
            .Where(e => e.Id == id && e.UserId == CurrentUserId)
            .Include(e => e.Category)
            .Select(e => new
            {
                e.Id,
                e.Amount,
                e.Description,
                e.Date,
                e.CategoryId,
                Category = new { e.Category.Id, e.Category.Name, e.Category.Color }
            })
            .FirstOrDefaultAsync();

        if (expense is null) return NotFound();
        return Ok(expense);
    }

    // POST /api/expenses
    [HttpPost]
    public async Task<IActionResult> Create(ExpenseRequest request)
    {
        var expense = new Expense
        {
            UserId = CurrentUserId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Description = request.Description,
            Date = request.Date
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        await _db.Entry(expense).Reference(e => e.Category).LoadAsync();

        return CreatedAtAction(nameof(GetAll), new { id = expense.Id }, new
        {
            expense.Id,
            expense.Amount,
            expense.Description,
            expense.Date,
            expense.CategoryId,
            Category = new { expense.Category.Id, expense.Category.Name, expense.Category.Color }
        });
    }

    // PUT /api/expenses/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ExpenseRequest request)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);
        if (expense is null) return NotFound();

        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.Date = request.Date;
        expense.CategoryId = request.CategoryId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/expenses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);
        if (expense is null) return NotFound();

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
