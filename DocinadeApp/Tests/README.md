# Pruebas E2E con Playwright

## Descripción
Este directorio contiene las pruebas End-to-End (E2E) para RubricasApp utilizando Playwright.

## Instalación

1. Instalar dependencias de Node.js:
```bash
npm install
```

2. Instalar navegadores de Playwright:
```bash
npx playwright install
```

## Estructura de Pruebas

```
tests/
├── experiencia-laboral.spec.ts  # Pruebas de Experiencia Laboral Previa (Step 5)
└── ...más pruebas
```

## Ejecutar Pruebas

### Todas las pruebas
```bash
npm test
```

### Pruebas específicas de Experiencia Laboral
```bash
npm run test:experiencia
```

### Modo interactivo con UI
```bash
npm run test:ui
```

### Ver navegadores ejecutándose
```bash
npm run test:headed
```

### Modo debug (paso a paso)
```bash
npm run test:debug
```

### Ver reporte de última ejecución
```bash
npm run test:report
```

### Generar código de pruebas automáticamente
```bash
npm run test:codegen
```

## Configuración

La configuración principal está en `playwright.config.ts`:

- **URL base**: `https://localhost:18163`
- **Navegadores**: Chromium, Firefox, WebKit
- **Timeout**: 60 segundos por prueba
- **Capturas**: Solo en fallos
- **Videos**: Solo en fallos
- **Reportes**: HTML, JSON y lista en consola

## Pruebas de Experiencia Laboral Previa

### Casos Cubiertos

1. **Visualización inicial**
   - Mostrar alerta cuando no hay experiencias
   - Ocultar tabla cuando está vacía

2. **Agregar experiencias**
   - Abrir modal de agregar
   - Llenar formulario correctamente
   - Validar campos requeridos
   - Agregar una experiencia
   - Agregar múltiples experiencias

3. **Visualización en tabla**
   - Mostrar institución, cargo, tipo
   - Calcular duración (años/meses)
   - Mostrar "Actualidad" para trabajos actuales
   - Aplicar estilos visuales (bordered, hover)

4. **Eliminar experiencias**
   - Eliminar una experiencia específica
   - Confirmar eliminación
   - Mostrar alerta al eliminar todas

5. **Validaciones**
   - Campos requeridos
   - Deshabilitar fecha fin cuando trabaja actualmente
   - Mantener orden de experiencias

6. **Persistencia**
   - Mantener datos al navegar entre pasos
   - Hidden inputs para envío de formulario

7. **Navegación del Wizard**
   - Clic en indicadores de pasos
   - Efectos hover en navegación
   - Saltar entre pasos libremente

## Mejores Prácticas

1. **Esperar elementos**: Usar `waitForSelector` para elementos dinámicos
2. **Interacciones**: Simular acciones de usuario reales
3. **Assertions**: Usar `expect` para verificaciones claras
4. **Timeout**: Ajustar según necesidad de cada prueba
5. **Cleanup**: Cada prueba debe ser independiente

## Debugging

### Ver trazas en Playwright Inspector
```bash
npm run test:debug -- tests/experiencia-laboral.spec.ts
```

### Ver capturas y videos de fallos
Los archivos se guardan en:
- `test-results/` - Capturas y videos
- `playwright-report/` - Reporte HTML

### Logs de consola
Los `console.log()` en las pruebas aparecen en la terminal.

## CI/CD

Las pruebas están configuradas para ejecutarse en:
- GitHub Actions
- Azure DevOps
- Otros pipelines CI

Variables de entorno importantes:
- `CI=true` - Activa reintentos automáticos
- `HEADED=true` - Muestra navegadores en CI

## Troubleshooting

### Error: "Certificate has expired"
La configuración ya incluye `ignoreHTTPSErrors: true` para desarrollo local.

### Error: "Timeout waiting for..."
Aumentar timeout específico o global en `playwright.config.ts`.

### Error: "Element not found"
Verificar selectores o agregar esperas explícitas con `waitForSelector`.

## Recursos

- [Documentación Playwright](https://playwright.dev)
- [API Reference](https://playwright.dev/docs/api/class-test)
- [Best Practices](https://playwright.dev/docs/best-practices)
