# Pruebas E2E - Módulo de Conducta

Este documento describe las pruebas de extremo a extremo (E2E) para el módulo de conducta del sistema RubricasApp.

## Archivo de Pruebas

### `aplicar-decision-profesional.spec.ts`

Pruebas para la funcionalidad de **Aplicar Decisión Profesional** (Opción C del REA 40862-V21, Artículo 132).

**URL de la vista:** `https://localhost:18163/NotaConducta/AplicarDecisionProfesional?idEstudiante=2&idPeriodo=1`

## Estructura de las Pruebas

### 1. Vista Principal
- ✅ Carga correcta de la página
- ✅ Visualización de información del estudiante
- ✅ Breadcrumb de navegación
- ✅ Campos del formulario visibles
- ✅ Campos obligatorios marcados
- ✅ Cuadro informativo del artículo 132

### 2. Validaciones del Formulario
- ✅ Validación de campos obligatorios
- ✅ Validación de longitud máxima
- ✅ Comportamiento del campo NotaAjustada según decisión
- ✅ Validación de tipo numérico para nota

### 3. Funcionalidad de Envío
- ✅ Envío correcto de decisión "Mantener Aplazado"
- ✅ Envío correcto de decisión "Aprobar Conducta"
- ✅ Cancelación y retorno a página anterior
- ✅ Confirmación antes de enviar (opcional)

### 4. Interfaz de Usuario
- ✅ Diseño responsivo (móvil, tablet, desktop)
- ✅ Estilos Bootstrap aplicados
- ✅ Iconos Font Awesome visibles
- ✅ Tooltips en elementos informativos
- ✅ Labels asociados a inputs

### 5. Accesibilidad
- ✅ Navegación con tabulador
- ✅ Orden de tabulación lógico
- ✅ Atributos ARIA apropiados

### 6. Manejo de Errores
- ✅ Manejo de errores de red
- ✅ Validación de estudiante y período

### 7. Rendimiento
- ✅ Tiempo de carga < 3 segundos
- ✅ Recursos estáticos cargados correctamente

## Requisitos Previos

1. **Node.js y npm instalados**
2. **Playwright instalado:**
   ```bash
   npm install -D @playwright/test
   npx playwright install
   ```

3. **Aplicación en ejecución:**
   ```bash
   dotnet run --urls https://localhost:18163
   ```

4. **Base de datos con datos de prueba:**
   - Estudiante con ID=2
   - Período académico con ID=1
   - Estudiante en estado "Aplazado"

## Ejecutar las Pruebas

### Ejecutar todas las pruebas (modo headless)
```bash
npx playwright test Tests/aplicar-decision-profesional.spec.ts
```

### Ejecutar con navegador visible
```bash
npx playwright test Tests/aplicar-decision-profesional.spec.ts --headed
```

### Ejecutar en modo debug
```bash
npx playwright test Tests/aplicar-decision-profesional.spec.ts --debug
```

### Ejecutar una suite específica
```bash
npx playwright test Tests/aplicar-decision-profesional.spec.ts --grep "Vista Principal"
```

### Ejecutar un test específico
```bash
npx playwright test Tests/aplicar-decision-profesional.spec.ts --grep "Debe cargar la página correctamente"
```

### Ejecutar en un navegador específico
```bash
npx playwright test Tests/aplicar-decision-profesional.spec.ts --project=chromium
npx playwright test Tests/aplicar-decision-profesional.spec.ts --project=firefox
npx playwright test Tests/aplicar-decision-profesional.spec.ts --project=webkit
```

### Ver el reporte de resultados
```bash
npx playwright show-report
```

## Datos de Prueba

### Decisión: Mantener Aplazado
```typescript
{
  numeroActa: 'ACTA-COM-2025-001',
  miembrosComite: 'Director Académico, Coordinador de Conducta, Profesor Guía, Orientador',
  justificacionPedagogica: 'El estudiante no ha mostrado un cambio significativo...',
  decision: 'Mantener Aplazado'
}
```

### Decisión: Aprobar Conducta
```typescript
{
  numeroActa: 'ACTA-COM-2025-002',
  miembrosComite: 'Director Académico, Coordinador de Conducta, Profesor Guía...',
  justificacionPedagogica: 'El estudiante ha mostrado un cambio positivo significativo...',
  notaAjustada: '70.0',
  decision: 'Aprobar Conducta'
}
```

## Configuración de Playwright

El archivo `playwright.config.ts` debe incluir:

```typescript
export default defineConfig({
  testDir: './Tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'https://localhost:18163',
    trace: 'on-first-retry',
    ignoreHTTPSErrors: true, // Importante para desarrollo
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
  ],
});
```

## Casos de Prueba Importantes

### ✅ Test 1: Carga de la Vista
Verifica que la página carga correctamente con toda la información del estudiante.

### ✅ Test 2: Validación de Campos Obligatorios
Asegura que no se puede enviar el formulario sin completar los campos requeridos.

### ✅ Test 3: Decisión "Mantener Aplazado"
Prueba el flujo completo de mantener a un estudiante aplazado.

### ✅ Test 4: Decisión "Aprobar Conducta"
Prueba el flujo completo de aprobar la conducta con nota ajustada.

### ✅ Test 5: Responsividad
Verifica que la interfaz funciona en dispositivos móviles, tablets y desktop.

### ✅ Test 6: Accesibilidad
Asegura que el formulario es navegable con teclado y tiene atributos ARIA.

## Solución de Problemas

### Error: "net::ERR_CERT_AUTHORITY_INVALID"
**Solución:** Asegurar que `ignoreHTTPSErrors: true` está en la configuración de Playwright.

### Error: "Timeout exceeded"
**Solución:** 
- Verificar que la aplicación está corriendo en `https://localhost:18163`
- Aumentar el timeout: `test.setTimeout(60000)`

### Error: "Element not found"
**Solución:**
- Verificar que los IDs y selectores en la vista coinciden con los de las pruebas
- Usar `await page.waitForSelector('#elemento', { state: 'visible' })`

### Error: "No se encontró el estudiante"
**Solución:**
- Ejecutar el script de seed data: `Scripts/seed-data-conducta.sql`
- Verificar que existe el estudiante con ID=2 en estado "Aplazado"

## Mejores Prácticas

1. **Ejecutar las pruebas antes de hacer commit**
2. **Mantener los datos de prueba actualizados**
3. **Actualizar las pruebas cuando cambie la interfaz**
4. **Agregar nuevas pruebas para nuevas funcionalidades**
5. **Documentar casos edge y errores conocidos**

## Integración Continua (CI/CD)

Para integrar estas pruebas en un pipeline de CI/CD:

```yaml
# .github/workflows/playwright.yml
name: Playwright Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: 18
      - name: Install dependencies
        run: npm ci
      - name: Install Playwright Browsers
        run: npx playwright install --with-deps
      - name: Run Playwright tests
        run: npx playwright test
      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: playwright-report
          path: playwright-report/
```

## Recursos Adicionales

- [Documentación de Playwright](https://playwright.dev/)
- [Mejores prácticas de testing E2E](https://playwright.dev/docs/best-practices)
- [Selectores en Playwright](https://playwright.dev/docs/selectors)
- [Debugging en Playwright](https://playwright.dev/docs/debug)

## Contacto y Soporte

Para reportar problemas o sugerir mejoras a las pruebas, contactar al equipo de desarrollo.
