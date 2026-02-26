/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#e6f4ff',
          100: '#bae0ff',
          200: '#91caff',
          300: '#69b1ff',
          400: '#4096ff',
          500: '#1677ff',
          600: '#0958d9',
          700: '#003eb3',
          800: '#002c8c',
          900: '#001d66',
        },
        danger: {
          50: '#fff1f0',
          100: '#ffccc7',
          200: '#ffa39e',
          400: '#ff4d4f',
          500: '#f5222d',
          600: '#cf1322',
        },
      },
    },
  },
  plugins: [],
};
