import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { config } from 'dotenv';

config();

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      'styles': '/src/styles',
      'components': '/src/components',
      'pages': '/src/pages'
    },
    define: {
      'process.env': process.env
    }
  },
});