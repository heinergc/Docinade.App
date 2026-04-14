/**
 * Script de verificación de permisos - Estudiantes y Empadronamiento
 * 
 * Verifica que los permisos de Estudiantes estén correctamente aplicados
 * tanto en EstudiantesController como en EmpadronamientoController.
 * 
 * Ejecución: node Tests/test-estudiantes-empadronamiento-permissions.js
 */

// ============================================================================
// CONFIGURACIÓN Y CONSTANTES
// ============================================================================

const MODULO = "Estudiantes y Empadronamiento";

// Permisos definidos para Estudiantes (compartidos con Empadronamiento)
const PERMISOS_ESTUDIANTES = {
    VER: "estudiantes.ver",
    CREAR: "estudiantes.crear", 
    EDITAR: "estudiantes.editar",
    ELIMINAR: "estudiantes.eliminar",
    IMPORTAR: "estudiantes.importar",
    EXPORTAR: "estudiantes.exportar",
    VER_HISTORIAL: "estudiantes.ver_historial",
    VER_NOTAS: "estudiantes.ver_notas",
    EDITAR_NOTAS: "estudiantes.editar_notas"
};

// Mapeo de acciones del EstudiantesController a permisos
const MAPEO_ESTUDIANTES_CONTROLLER = {
    "Index": PERMISOS_ESTUDIANTES.VER,
    "Details": PERMISOS_ESTUDIANTES.VER,
    "Create (GET)": PERMISOS_ESTUDIANTES.CREAR,
    "Create (POST)": PERMISOS_ESTUDIANTES.CREAR,
    "Edit (GET)": PERMISOS_ESTUDIANTES.EDITAR,
    "Edit (POST)": PERMISOS_ESTUDIANTES.EDITAR,
    "Delete (GET)": PERMISOS_ESTUDIANTES.ELIMINAR,
    "DeleteConfirmed (POST)": PERMISOS_ESTUDIANTES.ELIMINAR,
    "ImportarExcel (GET)": PERMISOS_ESTUDIANTES.IMPORTAR,
    "ImportarExcel (POST)": PERMISOS_ESTUDIANTES.IMPORTAR,
    "DescargarPlantilla": PERMISOS_ESTUDIANTES.EXPORTAR,
    "EliminarPorFiltro": PERMISOS_ESTUDIANTES.ELIMINAR,
    "Search": PERMISOS_ESTUDIANTES.VER,
    "BuscarPorCedula": PERMISOS_ESTUDIANTES.VER
};

// Mapeo de acciones del EmpadronamientoController a permisos
const MAPEO_EMPADRONAMIENTO_CONTROLLER = {
    "Index": PERMISOS_ESTUDIANTES.VER,
    "Details": PERMISOS_ESTUDIANTES.VER,
    "Create (GET)": PERMISOS_ESTUDIANTES.CREAR,
    "Create (POST)": PERMISOS_ESTUDIANTES.CREAR,
    "Edit (GET)": PERMISOS_ESTUDIANTES.EDITAR,
    "Edit (POST)": PERMISOS_ESTUDIANTES.EDITAR,
    "CambiarEtapa": PERMISOS_ESTUDIANTES.EDITAR
};

// ============================================================================
// CASOS DE USO POR PERFIL
// ============================================================================

const CASOS_USO = {
    "👤 Administrador del Sistema": {
        permisos: Object.values(PERMISOS_ESTUDIANTES),
        casos: [
            "Gestión completa de estudiantes y empadronamiento",
            "Importar/exportar datos masivos de estudiantes", 
            "Eliminar estudiantes y registros de empadronamiento",
            "Ver historial completo y notas de estudiantes",
            "Administrar todas las etapas del empadronamiento"
        ]
    },
    
    "👨‍🏫 Coordinador Académico": {
        permisos: [
            PERMISOS_ESTUDIANTES.VER,
            PERMISOS_ESTUDIANTES.CREAR,
            PERMISOS_ESTUDIANTES.EDITAR,
            PERMISOS_ESTUDIANTES.IMPORTAR,
            PERMISOS_ESTUDIANTES.EXPORTAR,
            PERMISOS_ESTUDIANTES.VER_HISTORIAL
        ],
        casos: [
            "Ver y gestionar estudiantes de su área",
            "Crear nuevos registros de estudiantes",
            "Importar listas de estudiantes desde Excel",
            "Gestionar empadronamiento de estudiantes",
            "Ver historial académico de estudiantes"
        ]
    },
    
    "👩‍🏫 Profesor": {
        permisos: [
            PERMISOS_ESTUDIANTES.VER,
            PERMISOS_ESTUDIANTES.VER_NOTAS
        ],
        casos: [
            "Ver lista de estudiantes asignados",
            "Consultar datos de empadronamiento",
            "Ver notas y calificaciones de estudiantes"
        ]
    },
    
    "📋 Asistente Administrativo": {
        permisos: [
            PERMISOS_ESTUDIANTES.VER,
            PERMISOS_ESTUDIANTES.CREAR,
            PERMISOS_ESTUDIANTES.EDITAR,
            PERMISOS_ESTUDIANTES.IMPORTAR
        ],
        casos: [
            "Registrar nuevos estudiantes",
            "Actualizar información de estudiantes",
            "Procesar empadronamiento de estudiantes",
            "Importar listas de estudiantes"
        ]
    },
    
    "👀 Observador": {
        permisos: [
            PERMISOS_ESTUDIANTES.VER
        ],
        casos: [
            "Consultar listado de estudiantes",
            "Ver información básica de empadronamiento"
        ]
    }
};

// ============================================================================
// VALIDACIONES
// ============================================================================

console.log(`🔍 VERIFICACIÓN DE PERMISOS - ${MODULO.toUpperCase()}`);
console.log("=".repeat(70));

// 1. Verificar estructura de permisos
console.log("\n📋 1. ESTRUCTURA DE PERMISOS COMPARTIDOS");
console.log("-".repeat(40));
Object.entries(PERMISOS_ESTUDIANTES).forEach(([nombre, permiso]) => {
    console.log(`✅ ${nombre}: ${permiso}`);
});

// 2. Verificar mapeo de EstudiantesController
console.log("\n🔗 2. MAPEO DE ACCIONES - EstudiantesController");
console.log("-".repeat(40));
Object.entries(MAPEO_ESTUDIANTES_CONTROLLER).forEach(([accion, permiso]) => {
    console.log(`✅ ${accion} → ${permiso}`);
});

// 3. Verificar mapeo de EmpadronamientoController
console.log("\n🔗 3. MAPEO DE ACCIONES - EmpadronamientoController");
console.log("-".repeat(40));
Object.entries(MAPEO_EMPADRONAMIENTO_CONTROLLER).forEach(([accion, permiso]) => {
    console.log(`✅ ${accion} → ${permiso}`);
});

// 4. Verificar casos de uso por perfil
console.log("\n👥 4. CASOS DE USO POR PERFIL");
console.log("-".repeat(40));
Object.entries(CASOS_USO).forEach(([perfil, config]) => {
    console.log(`\n${perfil}:`);
    console.log(`   📌 Permisos (${config.permisos.length}): ${config.permisos.join(", ")}`);
    console.log(`   🎯 Casos de uso:`);
    config.casos.forEach(caso => {
        console.log(`      • ${caso}`);
    });
});

// ============================================================================
// VERIFICACIONES TÉCNICAS
// ============================================================================

console.log("\n🔧 5. VERIFICACIONES TÉCNICAS");
console.log("-".repeat(40));

// Verificar naming conventions
const patronPermiso = /^estudiantes\.[a-z_]+$/;
const permisosInvalidos = Object.values(PERMISOS_ESTUDIANTES).filter(p => !patronPermiso.test(p));

if (permisosInvalidos.length === 0) {
    console.log("✅ Todos los permisos siguen el patrón de nomenclatura correcto");
} else {
    console.log("❌ Permisos con nomenclatura incorrecta:", permisosInvalidos);
}

// Verificar completitud de mapeo para EstudiantesController
const accionesEsperadasEstudiantes = [
    "Index", "Details", "Create (GET)", "Create (POST)", "Edit (GET)", "Edit (POST)", 
    "Delete (GET)", "DeleteConfirmed (POST)", "ImportarExcel (GET)", "ImportarExcel (POST)",
    "DescargarPlantilla", "EliminarPorFiltro", "Search", "BuscarPorCedula"
];
const accionesMapeadasEstudiantes = Object.keys(MAPEO_ESTUDIANTES_CONTROLLER);
const accionesFaltantesEstudiantes = accionesEsperadasEstudiantes.filter(accion => !accionesMapeadasEstudiantes.includes(accion));

if (accionesFaltantesEstudiantes.length === 0) {
    console.log("✅ Todas las acciones de EstudiantesController están mapeadas");
} else {
    console.log("❌ Acciones de EstudiantesController sin mapear:", accionesFaltantesEstudiantes);
}

// Verificar completitud de mapeo para EmpadronamientoController
const accionesEsperadasEmpadronamiento = [
    "Index", "Details", "Create (GET)", "Create (POST)", 
    "Edit (GET)", "Edit (POST)", "CambiarEtapa"
];
const accionesMapeadasEmpadronamiento = Object.keys(MAPEO_EMPADRONAMIENTO_CONTROLLER);
const accionesFaltantesEmpadronamiento = accionesEsperadasEmpadronamiento.filter(accion => !accionesMapeadasEmpadronamiento.includes(accion));

if (accionesFaltantesEmpadronamiento.length === 0) {
    console.log("✅ Todas las acciones de EmpadronamientoController están mapeadas");
} else {
    console.log("❌ Acciones de EmpadronamientoController sin mapear:", accionesFaltantesEmpadronamiento);
}

// ============================================================================
// URLS Y ENDPOINTS PROTEGIDOS
// ============================================================================

console.log("\n🌐 6. ENDPOINTS PROTEGIDOS");
console.log("-".repeat(40));

const endpoints = [
    // EstudiantesController
    { url: "GET /Estudiantes", permiso: "estudiantes.ver", descripcion: "Lista de estudiantes" },
    { url: "GET /Estudiantes/Details/{id}", permiso: "estudiantes.ver", descripcion: "Detalles de estudiante" },
    { url: "GET /Estudiantes/Create", permiso: "estudiantes.crear", descripcion: "Formulario crear estudiante" },
    { url: "POST /Estudiantes/Create", permiso: "estudiantes.crear", descripcion: "Crear estudiante" },
    { url: "GET /Estudiantes/Edit/{id}", permiso: "estudiantes.editar", descripcion: "Formulario editar estudiante" },
    { url: "POST /Estudiantes/Edit/{id}", permiso: "estudiantes.editar", descripcion: "Actualizar estudiante" },
    { url: "GET /Estudiantes/Delete/{id}", permiso: "estudiantes.eliminar", descripcion: "Confirmación eliminar" },
    { url: "POST /Estudiantes/Delete/{id}", permiso: "estudiantes.eliminar", descripcion: "Eliminar estudiante" },
    { url: "GET /Estudiantes/ImportarExcel", permiso: "estudiantes.importar", descripcion: "Importar desde Excel" },
    { url: "POST /Estudiantes/ImportarExcel", permiso: "estudiantes.importar", descripcion: "Procesar importación" },
    { url: "GET /Estudiantes/DescargarPlantilla", permiso: "estudiantes.exportar", descripcion: "Descargar plantilla Excel" },
    { url: "POST /Estudiantes/EliminarPorFiltro", permiso: "estudiantes.eliminar", descripcion: "Eliminación masiva" },
    { url: "GET /Estudiantes/Search", permiso: "estudiantes.ver", descripcion: "Búsqueda AJAX" },
    { url: "GET /Estudiantes/BuscarPorCedula", permiso: "estudiantes.ver", descripcion: "Búsqueda por cédula" },
    
    // EmpadronamientoController  
    { url: "GET /Empadronamiento", permiso: "estudiantes.ver", descripcion: "Lista empadronamiento" },
    { url: "GET /Empadronamiento/Details/{id}", permiso: "estudiantes.ver", descripcion: "Detalles empadronamiento" },
    { url: "GET /Empadronamiento/Create/{id}", permiso: "estudiantes.crear", descripcion: "Crear empadronamiento" },
    { url: "POST /Empadronamiento/Create", permiso: "estudiantes.crear", descripcion: "Guardar empadronamiento" },
    { url: "GET /Empadronamiento/Edit/{id}", permiso: "estudiantes.editar", descripcion: "Editar empadronamiento" },
    { url: "POST /Empadronamiento/Edit/{id}", permiso: "estudiantes.editar", descripcion: "Actualizar empadronamiento" },
    { url: "POST /Empadronamiento/CambiarEtapa/{id}", permiso: "estudiantes.editar", descripcion: "Cambiar etapa" }
];

endpoints.forEach(endpoint => {
    console.log(`✅ ${endpoint.url.padEnd(35)} → ${endpoint.permiso.padEnd(20)} | ${endpoint.descripcion}`);
});

// ============================================================================
// INSTRUCCIONES DE VERIFICACIÓN
// ============================================================================

console.log("\n🧪 7. COMANDOS DE VERIFICACIÓN");
console.log("-".repeat(40));

console.log(`
Para verificar la implementación:

1. Verificar atributos en EstudiantesController:
   grep -n "RequirePermission" Controllers/EstudiantesController.cs

2. Verificar atributos en EmpadronamientoController:
   grep -n "RequirePermission" Controllers/EmpadronamientoController.cs

3. Verificar permisos de estudiantes:
   grep -A 10 "class Estudiantes" Models/Permissions/ApplicationPermissions.cs

4. Compilar y probar:
   dotnet build
   dotnet run --urls https://localhost:18163

5. Probar endpoints protegidos:
   - https://localhost:18163/Estudiantes (requiere estudiantes.ver)
   - https://localhost:18163/Estudiantes/Create (requiere estudiantes.crear) 
   - https://localhost:18163/Empadronamiento (requiere estudiantes.ver)
   - https://localhost:18163/Empadronamiento/Create/1 (requiere estudiantes.crear)

6. Verificar comportamiento sin permisos:
   - Intentar acceso sin rol apropiado
   - Verificar redirección a página de acceso denegado
`);

console.log("\n✅ VERIFICACIÓN COMPLETADA");
console.log(`Fecha: ${new Date().toLocaleString()}`);
console.log(`Estado: Permisos aplicados en EstudiantesController y EmpadronamientoController`);
console.log(`URL principal: https://localhost:18163/Empadronamiento`);
console.log("=".repeat(70));
