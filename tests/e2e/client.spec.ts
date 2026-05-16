import fs from 'node:fs';
import path from 'node:path';

import { test, expect } from '@playwright/test';

const SCREENSHOT_DIR = 'tests/e2e/evidence';
fs.mkdirSync(SCREENSHOT_DIR, { recursive: true });

test('API docs page is accessible', async ({ page }) => {
  // Given the API is running
  const response = await page.goto('http://localhost:8080/docs');

  // Then the docs page should load
  expect(response?.status()).toBe(200);

  // Wait for page to render
  await page.waitForLoadState('networkidle', { timeout: 15000 });

  // Capture screenshot
  await page.screenshot({
    path: path.join(SCREENSHOT_DIR, '01_api_docs.png'),
    fullPage: true,
  });
});
