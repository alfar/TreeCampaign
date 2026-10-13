/// <reference types="vitest/config" />
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from "@tailwindcss/vite";
import { visualizer } from 'rollup-plugin-visualizer';

// https://vite.dev/config/
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { storybookTest } from '@storybook/addon-vitest/vitest-plugin';
import { playwright } from '@vitest/browser-playwright';
const dirname = typeof __dirname !== 'undefined' ? __dirname : path.dirname(fileURLToPath(import.meta.url));

// More info at: https://storybook.js.org/docs/next/writing-tests/integrations/vitest-addon
export default defineConfig({
  plugins: [react(), tailwindcss(), visualizer({
    filename: path.resolve(dirname, '../publish_site/wwwroot/assets/bundle-stats.html'),
    open: false,
    gzipSize: true,
    brotliSize: true
  })],
  server: {
    proxy: {
      '/api': 'http://localhost:5157'
    }
  },
  build: {
    outDir: path.resolve(dirname, '../publish_site/wwwroot'),
    emptyOutDir: true,
    sourcemap: true,
    // Split large vendor libraries into separate chunks to avoid a single huge bundle
    chunkSizeWarningLimit: 600,
    rollupOptions: {
      output: {
        manualChunks(id: string) {
          if (id.includes('node_modules')) {
            if (id.includes('react') || id.includes('react-dom')) return 'react-vendor';
            if (id.includes('leaflet') || id.includes('react-leaflet')) return 'leaflet-vendor';
            if (id.includes('@heroicons') || id.includes('heroicons')) return 'icons-vendor';
            if (id.includes('msw') || id.includes('msw-storybook-addon')) return 'msw-vendor';
            return 'vendor';
          }
        }
      }
    }
  },
  test: {
    projects: [{
      extends: true,
      plugins: [
      // The plugin will run tests for the stories defined in your Storybook config
      // See options at: https://storybook.js.org/docs/next/writing-tests/integrations/vitest-addon#storybooktest
      storybookTest({
        configDir: path.join(dirname, '.storybook')
      })],
      test: {
        name: 'storybook',
        browser: {
          enabled: true,
          headless: true,
          provider: playwright({}),
          instances: [{
            browser: 'chromium'
          }]
        }
      }
    }]
  }
});