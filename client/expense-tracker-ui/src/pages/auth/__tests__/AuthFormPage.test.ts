import { ValidationError } from 'yup';
import { loginSchema, registerSchema } from '../authSchemas';

describe('loginSchema', () => {
  it('accepts valid login data', async () => {
    const validInputs = [
      { email: 'user@example.com', password: 'pass' },
      { email: 'admin@mail.ru', password: '12345678' },
      { email: 'a@b.co', password: 'x' },
    ];

    for (const input of validInputs) {
      await expect(loginSchema.validate(input)).resolves.toBeTruthy();
    }
  });

  it('rejects missing email', async () => {
    await expect(loginSchema.validate({ email: '', password: 'pass' }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('rejects invalid email format', async () => {
    const invalidEmails = ['not-an-email', 'missing@', '@no-local.com', 'spaces in@mail.com'];

    for (const email of invalidEmails) {
      await expect(loginSchema.validate({ email, password: 'pass' }))
        .rejects.toBeInstanceOf(ValidationError);
    }
  });

  it('rejects missing password', async () => {
    await expect(loginSchema.validate({ email: 'user@example.com', password: '' }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('returns correct error message for invalid email', async () => {
    await expect(loginSchema.validate({ email: 'bad', password: 'pass' }))
      .rejects.toThrow('Invalid email');
  });

  it('returns correct error message for missing email', async () => {
    await expect(loginSchema.validate({ email: '', password: 'pass' }))
      .rejects.toThrow('Email is required');
  });
});

describe('registerSchema', () => {
  it('accepts valid registration data', async () => {
    const validInputs = [
      { email: 'user@example.com', password: '12345678' },
      { email: 'admin@mail.ru', password: 'securepassword' },
      { email: 'a@b.co', password: 'eightchr' },
    ];

    for (const input of validInputs) {
      await expect(registerSchema.validate(input)).resolves.toBeTruthy();
    }
  });

  it('rejects password shorter than 8 characters', async () => {
    const shortPasswords = ['1', '1234', '1234567'];

    for (const password of shortPasswords) {
      await expect(registerSchema.validate({ email: 'user@example.com', password }))
        .rejects.toBeInstanceOf(ValidationError);
    }
  });

  it('returns correct error message for short password', async () => {
    await expect(registerSchema.validate({ email: 'user@example.com', password: 'short' }))
      .rejects.toThrow('Password must be at least 8 characters');
  });

  it('rejects missing email', async () => {
    await expect(registerSchema.validate({ email: '', password: '12345678' }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('rejects invalid email format', async () => {
    const invalidEmails = ['not-an-email', 'missing@', '@no-local.com'];

    for (const email of invalidEmails) {
      await expect(registerSchema.validate({ email, password: '12345678' }))
        .rejects.toBeInstanceOf(ValidationError);
    }
  });

  it('rejects missing password', async () => {
    await expect(registerSchema.validate({ email: 'user@example.com', password: '' }))
      .rejects.toBeInstanceOf(ValidationError);
  });
});
