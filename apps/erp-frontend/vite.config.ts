import path from 'node:path';
import { defineConfig } from 'vite';
import react, { reactCompilerPreset } from '@vitejs/plugin-react';
import babel from '@rolldown/plugin-babel';
import tailwindcss from '@tailwindcss/vite';

// https://vite.dev/config/
export default defineConfig({
  resolve: {
    alias: {
      '@contracts': path.resolve(import.meta.dirname, '../../contracts'),
    },
  },
  server: {
    fs: {
      allow: [path.resolve(import.meta.dirname, '../..')],
    },
  },
  plugins: [
    react(),
    babel({
      presets: [reactCompilerPreset()],
    }),
    tailwindcss(),
  ],
});
