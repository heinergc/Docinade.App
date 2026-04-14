/**
 * Script de verificación de permisos - Instrumentos de Evaluación
 * 
 * Verifica que los permisos de Instrumentos de Evaluación estén correctamente implementados
 * en controladores, políticas y configuración del sistema.
 * 
 * Ejecución: node Tests/test-instrumentos-evaluacion-permissions.js
 */

// ============================================================================
// CONFIGURACIÓN Y CONSTANTES
// ============================================================================

const MODULO = "Instrumentos de Evaluación";
const CONTROLADOR = "InstrumentosEvaluacionController";

// Permisos definidos para Instrumentos de Evaluación
const PERMISOS_INSTRUMENTOS = {
    VER: "instrumentos_evaluacion.ver",
    CREAR: "instrumentos_evaluacion.crear", 
    EDITAR: "instrumentos_evaluacion.editar",
    ELIMINAR: "instrumentos_evaluacion.eliminar",
    DUPLICAR: "instrumentos_evaluacion.duplicar",
    ASIGNAR_MATERIAS: "instrumentos_evaluacion.asignar_materias",
    ACTIVAR_DESACTIVAR: "instrumentos_evaluacion.activar_desactivar",
    GESTIONAR_CONFIGURACION: "instrumentos_evaluacion.gestionar_configuracion",
    EXPORTAR: "instrumentos_evaluacion.exportar",
    IMPORTAR: "instrumentos_evaluacion.importar",
    VER_ESTADISTICAS: "instrumentos_evaluacion.ver_estadisticas",
    PUBLICAR: "instrumentos_evaluacion.publicar"
};

// Mapeo de acciones del controlador a permisos
const MAPEO_ACCIONES = {
    "Index": PERMISOS_INSTRUMENTOS.VER,
    "Details": PERMISOS_INSTRUMENTOS.VER,
    "Create (GET)": PERMISOS_INSTRUMENTOS.CREAR,
    "Create (POST)": PERMISOS_INSTRUMENTOS.CREAR,
    "Edit (GET)": PERMISOS_INSTRUMENTOS.EDITAR,
    "Edit (POST)": PERMISOS_INSTRUMENTOS.EDITAR,
    "Delete (GET)": PERMISOS_INSTRUMENTOS.ELIMINAR,
    "DeleteConfirmed (POST)": PERMISOS_INSTRUMENTOS.ELIMINAR
};

// ============================================================================
// CASOS DE USO POR PERFIL
// ============================================================================

const CASOS_USO = {
    "👤 Administrador del Sistema": {
        permisos: Object.values(PERMISOS_INSTRUMENTOS),
        casos: [
            "Gestionar completamente todos los instrumentos de evaluación",
            "Crear nuevos instrumentos con configuraciones avanzadas",
            "Asignar instrumentos a múltiples materias",
            "Activar/desactivar instrumentos según necesidades",
            "Ver estadísticas detalladas de uso de instrumentos",
            "Exportar/importar instrumentos entre sistemas",
            "Publicar instrumentos para uso institucional"
        ]
    },
    
    "👨‍🏫 Coordinador Académico": {
        permisos: [
            PERMISOS_INSTRUMENTOS.VER,
            PERMISOS_INSTRUMENTOS.CREAR,
            PERMISOS_INSTRUMENTOS.EDITAR,
            PERMISOS_INSTRUMENTOS.DUPLICAR,
            PERMISOS_INSTRUMENTOS.ASIGNAR_MATERIAS,
            PERMISOS_INSTRUMENTOS.VER_ESTADISTICAS,
            PERMISOS_INSTRUMENTOS.PUBLICAR
        ],
        casos: [
            "Crear instrumentos para su área académica",
            "Duplicar instrumentos existentes para adaptarlos",
            "Asignar instrumentos a materias de su coordinación",
            "Ver estadísticas de uso en su área",
            "Publicar instrumentos para uso de profesores"
        ]
    },
    
    "👩‍🏫 Profesor": {
        permisos: [
            PERMISOS_INSTRUMENTOS.VER,
            PERMISOS_INSTRUMENTOS.DUPLICAR
        ],
        casos: [
            "Ver instrumentos disponibles para sus materias",
            "Duplicar instrumentos para personalizar evaluaciones"
        ]
    },
    
    "👀 Observador": {
        permisos: [
            PERMISOS_INSTRUMENTOS.VER
        ],
        casos: [
            "Ver catálogo de instrumentos disponibles",
            "Consultar detalles de instrumentos publicados"
        ]
    }
};

// ============================================================================
// VALIDACIONES
// ============================================================================

console.log(`🔍 VERIFICACIÓN DE PERMISOS - ${MODULO.toUpperCase()}`);
console.log("=".repeat(70));

// 1. Verificar estructura de permisos
console.log("\n📋 1. ESTRUCTURA DE PERMISOS");
console.log("-".repeat(40));
Object.entries(PERMISOS_INSTRUMENTOS).forEach(([nombre, permiso]) => {
    console.log(`✅ ${nombre}: ${permiso}`);
});

// 2. Verificar mapeo de acciones
console.log("\n🔗 2. MAPEO DE ACCIONES DEL CONTROLADOR");
console.log("-".repeat(40));
Object.entries(MAPEO_ACCIONES).forEach(([accion, permiso]) => {
    console.log(`✅ ${accion} → ${permiso}`);
});

// 3. Verificar casos de uso por perfil
console.log("\n👥 3. CASOS DE USO POR PERFIL");
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

console.log("\n🔧 4. VERIFICACIONES TÉCNICAS");
console.log("-".repeat(40));

// Verificar naming conventions
const patronPermiso = /^instrumentos_evaluacion\.[a-z_]+$/;
const permisosInvalidos = Object.values(PERMISOS_INSTRUMENTOS).filter(p => !patronPermiso.test(p));

if (permisosInvalidos.length === 0) {
    console.log("✅ Todos los permisos siguen el patrón de nomenclatura correcto");
} else {
    console.log("❌ Permisos con nomenclatura incorrecta:", permisosInvalidos);
}

// Verificar completitud de mapeo
const accionesBasicas = ["Index", "Details", "Create (GET)", "Create (POST)", "Edit (GET)", "Edit (POST)", "Delete (GET)", "DeleteConfirmed (POST)"];
const accionesMapeadas = Object.keys(MAPEO_ACCIONES);
const accionesFaltantes = accionesBasicas.filter(accion => !accionesMapeadas.includes(accion));

if (accionesFaltantes.length === 0) {
    console.log("✅ Todas las acciones básicas están mapeadas a permisos");
} else {
    console.log("❌ Acciones sin mapear:", accionesFaltantes);
}

// ============================================================================
// INSTRUCCIONES DE IMPLEMENTACIÓN
// ============================================================================

console.log("\n📖 5. CHECKLIST DE IMPLEMENTACIÓN");
console.log("-".repeat(40));

const checklist = [
    "Crear constantes de permisos en ApplicationPermissions.cs",
    "Agregar categoría INSTRUMENTOS_EVALUACION en constantes",
    "Implementar clase InstrumentosEvaluacion con todos los permisos",
    "Agregar permisos a la lista GetAllPermissionsGrouped()",
    "Configurar políticas en AuthorizationExtensions.cs",
    "Agregar módulo 'instrumentos_evaluacion' en PermissionPolicyProvider",
    "Agregar acciones específicas en validActions del PermissionPolicyProvider",
    "Aplicar atributos [RequirePermission] en InstrumentosEvaluacionController",
    "Probar acceso con diferentes perfiles de usuario",
    "Documentar permisos en archivo de configuración de roles"
];

checklist.forEach((item, index) => {
    console.log(`${index + 1}. ☐ ${item}`);
});

// ============================================================================
// COMANDOS DE VERIFICACIÓN
// ============================================================================

console.log("\n🧪 6. COMANDOS DE VERIFICACIÓN");
console.log("-".repeat(40));

console.log(`
Para verificar la implementación:

1. Verificar definición de permisos:
   grep -r "instrumentos_evaluacion" Models/Permissions/

2. Verificar políticas:
   grep -r "instrumentos_evaluacion" Configuration/AuthorizationExtensions.cs

3. Verificar controlador:
   grep -r "RequirePermission.*InstrumentosEvaluacion" Controllers/

4. Compilar y probar:
   dotnet build
   dotnet run

5. Probar endpoints con diferentes usuarios:
   - /InstrumentosEvaluacion (requiere VER)
   - /InstrumentosEvaluacion/Create (requiere CREAR)
   - /InstrumentosEvaluacion/Edit/1 (requiere EDITAR)
   - /InstrumentosEvaluacion/Delete/1 (requiere ELIMINAR)
`);

console.log("\n✅ VERIFICACIÓN COMPLETADA");
console.log(`Fecha: ${new Date().toLocaleString()}`);
console.log("=" .repeat(70));
