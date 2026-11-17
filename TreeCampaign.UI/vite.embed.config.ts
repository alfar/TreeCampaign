import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const dirname = path.dirname(fileURLToPath(import.meta.url));

// Embeddable public intake form: one self-contained script (React and CSS included) with a
// fixed, unhashed name so the embedding page never needs to change its <script> tag.
// Build with `npm run build:embed`; output goes to dist-embed/.
export default defineConfig({
  plugins: [react(), tailwindcss()],
  publicDir: false,
  define: {
    'process.env.NODE_ENV': JSON.stringify('production'),
  },
  build: {
    outDir: path.resolve(dirname, '../publish_site/wwwroot'),
    emptyOutDir: false, // outDir is shared with the main app build
    minify: true,
    lib: {
      entry: path.resolve(dirname, 'src/embed/main.tsx'),
      name: 'TreeCampaignIntake',
      formats: ['iife'],
      fileName: () => 'tree-campaign-intake.js',
    },
  },
});
