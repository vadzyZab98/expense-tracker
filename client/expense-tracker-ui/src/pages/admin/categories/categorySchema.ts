import * as Yup from 'yup';

export const categorySchema = Yup.object({
  name: Yup.string().required('Name is required'),
  color: Yup.string()
    .matches(/^#[0-9a-fA-F]{6}$/, 'Must be a valid hex color')
    .required('Color is required'),
});
