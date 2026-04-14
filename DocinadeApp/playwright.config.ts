import { defineConfig, devices } from '@playwright/test';

/**
 * Configuración de Playwright para pruebas E2E
 * @see https://playwright.dev/docs/test-configuration
 */
export default defineConfig({
  testDir: './Tests',
  
  /* Tiempo máximo de ejecución por prueba */
  timeout: 60 * 1000,
  
  /* Configuración de expect */
  expect: {
    timeout: 10000
  },
  
  /* Ejecutar pruebas en paralelo */
  fullyParallel: true,
  
  /* Fallar en CI si dejaste test.only */
  forbidOnly: !!process.env.CI,
  
  /* Reintentos en CI */
  retries: process.env.CI ? 2 : 0,
  
  /* Workers en paralelo */
  workers: process.env.CI ? 1 : undefined,
  
  /* Reporter */
  reporter: [
    ['html', { outputFolder: 'playwright-report' }],
    ['list'],
    ['json', { outputFile: 'test-results/results.json' }]
  ],
  
  /* Configuración compartida para todos los proyectos */
  use: {
    /* URL base para la aplicación */
    baseURL: 'https://localhost:18163',
    
    /* Capturas de pantalla solo en fallos */
    screenshot: 'only-on-failure',
    
    /* Videos solo en fallos */
    video: 'retain-on-failure',
    
    /* Traza en el primer reintento */
    trace: 'on-first-retry',
    
    /* Ignorar errores de HTTPS para desarrollo */
    ignoreHTTPSErrors: true,
  },

  /* Configuración de proyectos para diferentes navegadores */
  projects: [
    {
      name: 'chromium',
      use: { 
        ...devices['Desktop Chrome'],
        viewport: { width: 1920, height: 1080 }
      },
    },

    {
      name: 'firefox',
      use: { 
        ...devices['Desktop Firefox'],
        viewport: { width: 1920, height: 1080 }
      },
    },

    {
      name: 'webkit',
      use: { 
        ...devices['Desktop Safari'],
        viewport: { width: 1920, height: 1080 }
      },
    },

    /* Pruebas en dispositivos móviles */
    {
      name: 'Mobile Chrome',
      use: { ...devices['Pixel 5'] },
    },
    {
      name: 'Mobile Safari',
      use: { ...devices['iPhone 12'] },
    },
  ],

  /* Servidor de desarrollo */
  // webServer: {
  //   command: 'dotnet run --urls https://localhost:18163',
  //   url: 'https://localhost:18163',
  //   reuseExistingServer: !process.env.CI,
  //   timeout: 120 * 1000,
  //   ignoreHTTPSErrors: true,
  // },
});
