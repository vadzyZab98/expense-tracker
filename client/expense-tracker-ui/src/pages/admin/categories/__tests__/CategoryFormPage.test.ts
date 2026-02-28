import { ValidationError } from 'yup';
import { categorySchema } from '../categorySchema';

describe('categorySchema', () => {
  const validCategory = { name: 'Food', color: '#FF6B6B' };

  it('accepts valid category data', async () => {
    const validInputs = [
      validCategory,
      { name: 'Transport', color: '#1677ff' },
      { name: 'Entertainment', color: '#00AA00' },
      { name: 'A', color: '#abcdef' },
    ];

    for (const input of validInputs) {
      await expect(categorySchema.validate(input)).resolves.toBeTruthy();
    }
  });

  it('rejects missing name', async () => {
    await expect(categorySchema.validate({ name: '', color: '#FF6B6B' }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('returns correct error message for missing name', async () => {
    await expect(categorySchema.validate({ name: '', color: '#FF6B6B' }))
      .rejects.toThrow('Name is required');
  });

  it('rejects missing color', async () => {
    await expect(categorySchema.validate({ name: 'Food', color: '' }))
      .rejects.toBeInstanceOf(ValidationError);
  });

  it('rejects invalid hex color formats', async () => {
    const invalidColors = [
      'red',
      '#FFF',
      '#GGGGGG',
      '123456',
      '#12345',
      '#1234567',
      'rgb(0,0,0)',
    ];

    for (const color of invalidColors) {
      await expect(categorySchema.validate({ name: 'Food', color }))
        .rejects.toBeInstanceOf(ValidationError);
    }
  });

  it('returns correct error message for invalid hex color', async () => {
    await expect(categorySchema.validate({ name: 'Food', color: 'red' }))
      .rejects.toThrow('Must be a valid hex color');
  });

  it('accepts valid hex colors with mixed case', async () => {
    const validColors = ['#aAbBcC', '#112233', '#FFFFFF', '#000000'];

    for (const color of validColors) {
      await expect(categorySchema.validate({ name: 'Test', color })).resolves.toBeTruthy();
    }
  });
});
