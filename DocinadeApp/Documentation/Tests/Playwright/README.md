# Pruebas E2E de Asignación de Estudiantes

Este directorio contiene pruebas automatizadas end-to-end (E2E) para verificar la funcionalidad de asignación de estudiantes en RubricasApp usando Playwright.

## 🎯 Objetivo de las Pruebas

Las pruebas verifican que:
- ✅ Las tablas de estudiantes se refresquen correctamente después de asignar/desasignar
- ✅ Los contadores y estadísticas se actualicen en tiempo real
- ✅ Los filtros de búsqueda se mantengan después del refresco
- ✅ Las operaciones de asignación múltiple funcionen correctamente
- ✅ Las operaciones de desasignación múltiple funcionen correctamente
- ✅ La interfaz de usuario responda adecuadamente a las operaciones

## 🛠️ Configuración

### Prerrequisitos

1. **Node.js** (versión 16 o superior)
   - Descargar desde: https://nodejs.org/
   - Verificar instalación: `node --version`

2. **Aplicación ASP.NET ejecutándose**
   - La aplicación debe estar corriendo en: `https://localhost:18163`
   - Ejecutar desde el directorio principal: `dotnet run --urls https://localhost:18163`

### Instalación

#### En Windows:
```powershell
# Navegar al directorio de pruebas
cd Tests\Playwright

# Ejecutar el script de configuración
setup.bat
```

#### En Linux/Mac:
```bash
# Navegar al directorio de pruebas
cd Tests/Playwright

# Dar permisos de ejecución al script
chmod +x setup.sh

# Ejecutar el script de configuración
./setup.sh
```

#### Instalación Manual:
```bash
# Instalar dependencias
npm install

# Instalar navegadores de Playwright
npx playwright install
```

## 🚀 Ejecutar Pruebas

### Comandos Disponibles

```bash
# Ejecutar todas las pruebas (headless)
npm test

# Ejecutar pruebas con navegador visible
npm run test:headed

# Ejecutar pruebas en modo debug
npm run test:debug

# Ejecutar solo las pruebas de asignación de estudiantes
npm run test:asignacion

# Ejecutar pruebas de asignación con navegador visible
npm run test:asignacion:headed

# Abrir interfaz gráfica de Playwright
npm run test:ui

# Ver reporte de resultados después de ejecutar pruebas
npm run report
```

### Ejemplos de Ejecución

```bash
# Ejecutar una prueba específica
npx playwright test "debería cargar las tablas de estudiantes inicialmente"

# Ejecutar pruebas en un navegador específico
npx playwright test --project=chromium

# Ejecutar con configuración personalizada
npx playwright test --headed --slow-mo=1000
```

## 📋 Casos de Prueba

### 1. Carga Inicial de Tablas
- **Descripción**: Verifica que las tablas se carguen correctamente al acceder a la página
- **Valida**: 
  - Presencia de tablas de estudiantes disponibles y asignados
  - Actualización de badges con conteos correctos
  - Eliminación de indicadores de carga

### 2. Refresco tras Asignación Individual
- **Descripción**: Asigna un estudiante y verifica que las tablas se actualicen
- **Valida**:
  - Cambio en contadores de disponibles/asignados
  - Actualización visual de las tablas
  - Feedback correcto al usuario

### 3. Refresco tras Desasignación Individual
- **Descripción**: Desasigna un estudiante y verifica que las tablas se actualicen
- **Valida**:
  - Cambio en contadores de disponibles/asignados
  - Actualización visual de las tablas
  - Confirmación del usuario

### 4. Asignación Múltiple
- **Descripción**: Selecciona múltiples estudiantes y los asigna
- **Valida**:
  - Selección de múltiples checkboxes
  - Habilitación de botón de asignación múltiple
  - Actualización correcta tras la operación
  - Limpieza de selecciones

### 5. Desasignación Múltiple
- **Descripción**: Selecciona múltiples estudiantes asignados y los desasigna
- **Valida**:
  - Selección de múltiples checkboxes en tabla de asignados
  - Habilitación de botón de desasignación múltiple
  - Confirmación de la operación
  - Actualización correcta de tablas

### 6. Actualización de Estadísticas
- **Descripción**: Verifica que las estadísticas se actualicen correctamente
- **Valida**:
  - Contadores de totales
  - Porcentaje de ocupación
  - Espacios disponibles
  - Coherencia entre badges y estadísticas

### 7. Mantenimiento de Filtros
- **Descripción**: Verifica que los filtros de búsqueda se mantengan tras refresco
- **Valida**:
  - Aplicación de filtro de búsqueda
  - Mantenimiento del filtro tras operación
  - Funcionalidad de filtrado después del refresco

## 📊 Reportes

Después de ejecutar las pruebas, se generan reportes en:
- **HTML**: `test-results/index.html` - Reporte visual detallado
- **JSON**: `test-results/results.json` - Datos estructurados
- **JUnit**: `test-results/results.xml` - Compatible con CI/CD

### Ver Reportes
```bash
# Abrir reporte HTML en el navegador
npm run report

# O manualmente
npx playwright show-report
```

## 🔧 Configuración Personalizada

### Modificar `playwright.config.js`

```javascript
// Cambiar URL base si la aplicación corre en otro puerto
use: {
  baseURL: 'https://localhost:5001', // Cambiar puerto aquí
}

// Configurar timeouts
use: {
  actionTimeout: 15000,     // Timeout para acciones
  expect: { timeout: 8000 } // Timeout para aserciones
}

// Agregar más navegadores
projects: [
  { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
  { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
  // Agregar más configuraciones aquí
]
```

## 🐛 Solución de Problemas

### Errores Comunes

1. **Error de conexión HTTPS**
   ```
   Error: net::ERR_CERT_AUTHORITY_INVALID
   ```
   **Solución**: Las pruebas incluyen `ignoreHTTPSErrors: true`

2. **Aplicación no disponible**
   ```
   Error: connect ECONNREFUSED 127.0.0.1:18163
   ```
   **Solución**: Asegurar que la aplicación ASP.NET esté ejecutándose

3. **Timeouts en carga de datos**
   ```
   Error: Test timeout of 30000ms exceeded
   ```
   **Solución**: Verificar que la base de datos esté accesible y con datos

4. **Fallos de autenticación**
   ```
   Error: Expected to be on login page
   ```
   **Solución**: Verificar configuración de autenticación en `beforeEach`

### Debug Tips

```bash
# Ejecutar con debug paso a paso
npx playwright test --debug

# Ejecutar con logs detallados
DEBUG=pw:api npx playwright test

# Ejecutar con video para ver qué sucedió
npx playwright test --video=on

# Ejecutar con trazas completas
npx playwright test --trace=on
```

## 📝 Logs y Diagnósticos

Las pruebas incluyen logging detallado:

```javascript
console.log('✅ Cargados 5 estudiantes disponibles');
console.log('🔄 Recargando datos de estudiantes...');
console.log('❌ Error cargando estudiantes:', error);
```

Estos logs aparecen en la consola durante la ejecución y en los reportes.

## 🔄 Integración Continua

Para usar en CI/CD, agregar al pipeline:

```yaml
# GitHub Actions ejemplo
- name: Run Playwright tests
  run: |
    cd Tests/Playwright
    npm ci
    npx playwright install --with-deps
    npx playwright test
  env:
    BASE_URL: ${{ secrets.APP_URL }}
```

## 📞 Soporte

Si encuentras problemas con las pruebas:

1. Verificar que la aplicación esté funcionando manualmente
2. Revisar los logs en la consola
3. Ejecutar en modo debug para ver paso a paso
4. Verificar configuración de red y permisos
5. Consultar documentación de Playwright: https://playwright.dev/
