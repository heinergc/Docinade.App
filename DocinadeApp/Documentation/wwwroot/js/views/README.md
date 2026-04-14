# JavaScript Organization - RubricasApp

## 📁 Estructura de Archivos JavaScript

```
wwwroot/js/
├── views/
│   └── evaluaciones/
│       └── create.js          # JavaScript para Create de Evaluaciones
└── site.js                    # JavaScript global de la aplicación
```

## 🎯 Organización por Vista

### **Evaluaciones/Create**
- **Archivo**: `wwwroot/js/views/evaluaciones/create.js`
- **Clase**: `EvaluacionesCreate`
- **Funcionalidad**: Maneja la creación dinámica de formularios de evaluación
- **Características**:
  - Carga dinámica de items y niveles de rúbricas
  - Generación automática de formularios
  - Validación en tiempo real
  - Cálculo de puntuaciones
  - Debug integrado

## 🔧 Uso en las Vistas

### Inclusión en Create.cshtml
```html
@section Scripts {
    <!-- JavaScript específico para Create de Evaluaciones -->
    <script src="~/js/views/evaluaciones/create.js"></script>
    
    <script>
        $(document).ready(function() {
            // Configurar URLs específicas de Razor
            const baseUrl = '@Url.Action("ObtenerItemsPorRubrica", "Evaluaciones")';
            window.evaluacionesCreate.setBaseUrl(baseUrl);
        });
    </script>
}
```

## 🏗️ Patrones de Desarrollo

### **1. Clases ES6**
- Cada vista compleja tiene su propia clase
- Encapsulación de funcionalidad
- Métodos organizados por responsabilidad

### **2. Separación de Responsabilidades**
- **Archivo JS**: Lógica de negocio y manipulación del DOM
- **Vista Razor**: Solo configuración de URLs y datos específicos del servidor

### **3. Debug Integrado**
- Función `debugEvaluacion()` disponible globalmente
- Logs detallados en consola para desarrollo
- Estados de aplicación fácilmente inspeccionables

## 📋 Convenciones de Nomenclatura

### **Archivos**
- `views/[controlador]/[accion].js`
- Ejemplo: `views/evaluaciones/create.js`

### **Clases**
- `[Controlador][Accion]`
- Ejemplo: `EvaluacionesCreate`

### **Variables Globales**
- `window.[controlador][Accion]`
- Ejemplo: `window.evaluacionesCreate`

## 🚀 Próximas Vistas

Para crear JavaScript para otras vistas, seguir el mismo patrón:

1. **Crear carpeta**: `wwwroot/js/views/[controlador]/`
2. **Crear archivo**: `[accion].js`
3. **Crear clase**: `[Controlador][Accion]`
4. **Incluir en vista**: Razor con `@section Scripts`

## 🎯 Beneficios

- ✅ **Mantenibilidad**: Código organizado por funcionalidad
- ✅ **Reutilización**: Clases pueden extenderse o reutilizarse
- ✅ **Debug**: Herramientas de debug integradas
- ✅ **Performance**: Scripts específicos solo se cargan cuando se necesitan
- ✅ **Escalabilidad**: Estructura preparada para crecimiento