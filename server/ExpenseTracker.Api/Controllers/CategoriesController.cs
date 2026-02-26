using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db) => _db = db;

    public record CategoryRequest(string Name, string Color);

    // GET /api/categories — public
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name, c.Color })
            .ToListAsync();

        return Ok(categories);
    }

    // POST /api/categories — Admin only
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Color = request.Color
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = category.Id },
            new { category.Id, category.Name, category.Color });
    }

    // PUT /api/categories/{id} — Admin only
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CategoryRequest request)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null) return NotFound();

        category.Name = request.Name;
        category.Color = request.Color;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/categories/{id} — Admin only
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null) return NotFound();

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
