using RubricasApp.Web.Models;
using System.ComponentModel;

namespace RubricasApp.Web.Models.Permissions
{
    /// <summary>
    /// Define todos los permisos disponibles en el sistema
    /// </summary>
    public static class ApplicationPermissions
    {
        // Categorías de permisos
        public const string USUARIOS = "Usuarios";
        public const string RUBRICAS = "Rúbricas";
        public const string EVALUACIONES = "Evaluaciones";
        public const string ESTUDIANTES = "Estudiantes";
        public const string PROFESORES = "Profesores";
        public const string INSTITUCIONES = "Instituciones";
        public const string REPORTES = "Reportes";
        public const string CONFIGURACION = "Configuración";
        public const string AUDITORIA = "Auditoría";
        public const string PERIODOS = "Períodos Académicos";
        public const string NIVELES = "Niveles de Calificación";
        public const string ITEMS_EVALUACION = "Items de Evaluación";
        public const string INSTRUMENTOS_EVALUACION = "Instrumentos de Evaluación";
        public const string ASISTENCIA = "Asistencia";
        public const string TIPOS_GRUPO = "Tipos de Grupo";
        public const string GRUPOS_ESTUDIANTES = "Grupos de Estudiantes";
        public const string MATERIAS = "Materias";
        public const string INSTRUMENTO_MATERIAS = "Instrumento-Materias";
        public const string INSTRUMENTO_RUBRICA = "Instrumento-Rúbrica";
        public const string CUADERNO_CALIFICADOR = "Cuaderno Calificador";
        public const string SISTEMA_SEA = "SEA";
        public const string ADMIN = "Administración";


        /// <summary>
        /// Permisos relacionados con usuarios
        /// </summary>
        public static class Usuarios
        {
            public const string VER = "usuarios.ver";
            public const string CREAR = "usuarios.crear";
            public const string EDITAR = "usuarios.editar";
            public const string ELIMINAR = "usuarios.eliminar";
            public const string BLOQUEAR = "usuarios.bloquear";
            public const string CAMBIAR_ROLES = "usuarios.cambiar_roles";
            public const string VER_PERFIL_COMPLETO = "usuarios.ver_perfil_completo";
            public const string EXPORTAR = "usuarios.exportar";
            public const string IMPORTAR = "usuarios.importar";
            
            // Permisos adicionales para administración
            public const string View = VER;
            public const string Create = CREAR;
            public const string Edit = EDITAR;
            public const string Delete = ELIMINAR;
            public const string ManageRoles = "usuarios.gestionar_roles";
            public const string ManagePermissions = "usuarios.gestionar_permisos";
            public const string ResetPassword = "usuarios.restablecer_contraseña";
        }

        /// <summary>
        /// Permisos relacionados con rúbricas
        /// </summary>
        public static class Rubricas
        {
            public const string VER = "rubricas.ver";
            public const string VER_TODAS = "rubricas.ver_todas";
            public const string CREAR = "rubricas.crear";
            public const string EDITAR = "rubricas.editar";
            public const string EDITAR_TODAS = "rubricas.editar_todas";
            public const string ELIMINAR = "rubricas.eliminar";
            public const string ELIMINAR_TODAS = "rubricas.eliminar_todas";
            public const string DUPLICAR = "rubricas.duplicar";
            public const string PUBLICAR = "rubricas.publicar";
            public const string ARCHIVAR = "rubricas.archivar";
            public const string COMPARTIR = "rubricas.compartir";
            public const string EXPORTAR = "rubricas.exportar";
            public const string IMPORTAR = "rubricas.importar";
        }

        /// <summary>
        /// Permisos relacionados con evaluaciones
        /// </summary>
        public static class Evaluaciones
        {
            public const string VER = "evaluaciones.ver";
            public const string VER_TODAS = "evaluaciones.ver_todas";
            public const string CREAR = "evaluaciones.crear";
            public const string EDITAR = "evaluaciones.editar";
            public const string EDITAR_TODAS = "evaluaciones.editar_todas";
            public const string ELIMINAR = "evaluaciones.eliminar";
            public const string ELIMINAR_TODAS = "evaluaciones.eliminar_todas";
            public const string EVALUAR = "evaluaciones.evaluar";
            public const string REVISAR = "evaluaciones.revisar";
            public const string APROBAR = "evaluaciones.aprobar";
            public const string FINALIZAR = "evaluaciones.finalizar";
            public const string REABRIR = "evaluaciones.reabrir";
            public const string EXPORTAR = "evaluaciones.exportar";
            public const string VER_RESULTADOS = "evaluaciones.ver_resultados";
            public const string VER_ESTADISTICAS = "evaluaciones.ver_estadisticas";
            
            // Permisos para Items de Evaluación
            public const string ITEMS_VER = "evaluaciones.items.ver";
            public const string ITEMS_CREAR = "evaluaciones.items.crear";
            public const string ITEMS_EDITAR = "evaluaciones.items.editar";
            public const string ITEMS_ELIMINAR = "evaluaciones.items.eliminar";
        }

        /// <summary>
        /// Permisos relacionados con items de evaluación
        /// </summary>
        public static class ItemsEvaluacion
        {
            public const string VER = "items_evaluacion.ver";
            public const string CREAR = "items_evaluacion.crear";
            public const string EDITAR = "items_evaluacion.editar";
            public const string ELIMINAR = "items_evaluacion.eliminar";
            public const string DUPLICAR = "items_evaluacion.duplicar";
            public const string IMPORTAR = "items_evaluacion.importar";
            public const string EXPORTAR = "items_evaluacion.exportar";
            public const string REORDENAR = "items_evaluacion.reordenar";
            public const string GESTIONAR_CATEGORIAS = "items_evaluacion.gestionar_categorias";
        }

        /// <summary>
        /// Permisos relacionados con instrumentos de evaluación
        /// </summary>
        public static class InstrumentosEvaluacion
        {
            public const string VER = "instrumentos_evaluacion.ver";
            public const string CREAR = "instrumentos_evaluacion.crear";
            public const string EDITAR = "instrumentos_evaluacion.editar";
            public const string ELIMINAR = "instrumentos_evaluacion.eliminar";
            public const string DUPLICAR = "instrumentos_evaluacion.duplicar";
            public const string ASIGNAR_MATERIAS = "instrumentos_evaluacion.asignar_materias";
            public const string ACTIVAR_DESACTIVAR = "instrumentos_evaluacion.activar_desactivar";
            public const string GESTIONAR_CONFIGURACION = "instrumentos_evaluacion.gestionar_configuracion";
            public const string EXPORTAR = "instrumentos_evaluacion.exportar";
            public const string IMPORTAR = "instrumentos_evaluacion.importar";
            public const string VER_ESTADISTICAS = "instrumentos_evaluacion.ver_estadisticas";
            public const string PUBLICAR = "instrumentos_evaluacion.publicar";
        }

        /// <summary>
        /// Permisos relacionados con estudiantes
        /// </summary>
        public static class Estudiantes
        {
            public const string VER = "estudiantes.ver";
            public const string CREAR = "estudiantes.crear";
            public const string EDITAR = "estudiantes.editar";
            public const string ELIMINAR = "estudiantes.eliminar";
            public const string IMPORTAR = "estudiantes.importar";
            public const string EXPORTAR = "estudiantes.exportar";
            public const string VER_HISTORIAL = "estudiantes.ver_historial";
            public const string VER_NOTAS = "estudiantes.ver_notas";
            public const string EDITAR_NOTAS = "estudiantes.editar_notas";
        }

        /// <summary>
        /// Permisos relacionados con instituciones educativas
        /// </summary>
        public static class Instituciones
        {
            public const string VER = "instituciones.ver";
            public const string CREAR = "instituciones.crear";
            public const string EDITAR = "instituciones.editar";
            public const string ELIMINAR = "instituciones.eliminar";
            public const string VER_DETALLES = "instituciones.ver_detalles";
            public const string EXPORTAR = "instituciones.exportar";
        }

        /// <summary>
        /// Permisos relacionados con profesores
        /// </summary>
        public static class Profesores
        {
            public const string VER = "profesores.ver";
            public const string CREAR = "profesores.crear";
            public const string EDITAR = "profesores.editar";
            public const string ELIMINAR = "profesores.eliminar";
            public const string VER_DETALLES = "profesores.ver_detalles";
            public const string IMPORTAR = "profesores.importar";
            public const string EXPORTAR = "profesores.exportar";
            public const string VER_HISTORIAL = "profesores.ver_historial";
            public const string ASIGNAR_GRUPOS = "profesores.asignar_grupos";
            public const string GESTIONAR_HORARIOS = "profesores.gestionar_horarios";
            public const string VER_ESTADISTICAS = "profesores.ver_estadisticas";
            public const string CAMBIAR_ESTADO = "profesores.cambiar_estado";
        }

        /// <summary>
        /// Permisos relacionados con reportes
        /// </summary>
        public static class Reportes
        {
            public const string VER_BASICOS = "reportes.ver_basicos";
            public const string VER_AVANZADOS = "reportes.ver_avanzados";
            public const string VER_TODOS = "reportes.ver_todos";
            public const string CREAR_PERSONALIZADOS = "reportes.crear_personalizados";
            public const string EXPORTAR = "reportes.exportar";
            public const string PROGRAMAR = "reportes.programar";
            public const string VER_ESTADISTICAS_INSTITUCIONALES = "reportes.ver_estadisticas_institucionales";
        }

        /// <summary>
        /// Permisos relacionados con el sistema SEA (Sistema de Evaluación MEP)
        /// </summary>
        public static class SEA
        {
            public const string VER_REPORTE = "sea.ver_reporte";
            public const string EXPORTAR_CSV = "sea.exportar_csv";
            public const string EXPORTAR_EXCEL = "sea.exportar_excel";
            public const string EXPORTAR_PDF = "sea.exportar_pdf";
            public const string CONFIGURAR = "sea.configurar";
            public const string VER_ESTADISTICAS = "sea.ver_estadisticas";
        }

        /// <summary>
        /// Permisos relacionados con configuración del sistema
        /// </summary>
        public static class Configuracion
        {
            public const string VER = "configuracion.ver";
            public const string EDITAR_SISTEMA = "configuracion.editar_sistema";
            public const string EDITAR_SEGURIDAD = "configuracion.editar_seguridad";
            public const string GESTIONAR_ROLES = "configuracion.gestionar_roles";
            public const string GESTIONAR_PERMISOS = "configuracion.gestionar_permisos";
            public const string BACKUP = "configuracion.backup";
            public const string RESTAURAR = "configuracion.restaurar";
            public const string VER_LOGS = "configuracion.ver_logs";
            public const string LIMPIAR_LOGS = "configuracion.limpiar_logs";
            public const string MODO_MANTENIMIENTO = "configuracion.modo_mantenimiento";
            
            public const string CONFIGURAR_EMAIL = "configuracion.configurar_email";

            // Permisos adicionales para administración
            public const string Manage = "configuracion.gestionar";
            public const string Initialize = "configuracion.inicializar";
            public const string SyncPermissions = "configuracion.sincronizar_permisos";
            public const string HealthCheck = "configuracion.verificar_estado";
        }

        /// <summary>
        /// Permisos relacionados con auditoría
        /// </summary>
        public static class Auditoria
        {
            public const string VER = "auditoria.ver";
            public const string VER_ACCESOS = "auditoria.ver_accesos";
            public const string VER_CAMBIOS = "auditoria.ver_cambios";
            public const string VER_ERRORES = "auditoria.ver_errores";
            public const string EXPORTAR = "auditoria.exportar";
            public const string LIMPIAR = "auditoria.limpiar";
            public const string VER_METRICAS = "auditoria.ver_metricas";
            
            // Permisos adicionales para administración
            public const string View = VER;
            public const string ViewStatistics = VER_METRICAS;
            public const string Export = EXPORTAR;
            public const string Cleanup = LIMPIAR;
        }

        /// <summary>
        /// Permisos relacionados con períodos académicos
        /// </summary>
        public static class Periodos
        {
            public const string VER = "periodos.ver";
            public const string CREAR = "periodos.crear";
            public const string EDITAR = "periodos.editar";
            public const string ELIMINAR = "periodos.eliminar";
            public const string ACTIVAR = "periodos.activar";
            public const string CERRAR = "periodos.cerrar";
            public const string GESTIONAR_CALENDARIO = "periodos.gestionar_calendario";
        }

        /// <summary>
        /// Permisos relacionados con niveles de calificación
        /// </summary>
        public static class Niveles
        {
            public const string VER = "niveles.ver";
            public const string CREAR = "niveles.crear";
            public const string EDITAR = "niveles.editar";
            public const string ELIMINAR = "niveles.eliminar";
            public const string REORDENAR = "niveles.reordenar";
            public const string GESTIONAR_GRUPOS = "niveles.gestionar_grupos";
        }

        /// <summary>
        /// Permisos relacionados con asistencia y pase de lista
        /// </summary>
        public static class Asistencia
        {
            public const string VER = "asistencia.ver";
            public const string TOMAR_ASISTENCIA = "asistencia.tomar_asistencia";
            public const string EDITAR_ASISTENCIA = "asistencia.editar_asistencia";
            public const string ELIMINAR_REGISTROS = "asistencia.eliminar_registros";
            public const string VER_RESUMEN = "asistencia.ver_resumen";
            public const string IMPRIMIR = "asistencia.imprimir";
            public const string VER_DIAGNOSTICO = "asistencia.ver_diagnostico";
            public const string EXPORTAR = "asistencia.exportar";
            public const string GESTIONAR_GRUPOS = "asistencia.gestionar_grupos";
            public const string VER_ESTADISTICAS = "asistencia.ver_estadisticas";
            public const string CONFIGURAR_PARAMETROS = "asistencia.configurar_parametros";
        }

        /// <summary>
        /// Permisos relacionados con tipos de grupo
        /// </summary>
        public static class TiposGrupo
        {
            public const string VER = "tipos_grupo.ver";
            public const string CREAR = "tipos_grupo.crear";
            public const string EDITAR = "tipos_grupo.editar";
            public const string ELIMINAR = "tipos_grupo.eliminar";
            public const string VER_DETALLES = "tipos_grupo.ver_detalles";
            public const string CAMBIAR_ESTADO = "tipos_grupo.cambiar_estado";
            public const string GESTIONAR = "tipos_grupo.gestionar";
        }

        /// <summary>
        /// Permisos relacionados con grupos de estudiantes
        /// </summary>
        public static class GruposEstudiantes
        {
            public const string VER = "grupos_estudiantes.ver";
            public const string CREAR = "grupos_estudiantes.crear";
            public const string EDITAR = "grupos_estudiantes.editar";
            public const string ELIMINAR = "grupos_estudiantes.eliminar";
            public const string VER_DETALLES = "grupos_estudiantes.ver_detalles";
            public const string ASIGNAR_ESTUDIANTES = "grupos_estudiantes.asignar_estudiantes";
            public const string DESASIGNAR_ESTUDIANTES = "grupos_estudiantes.desasignar_estudiantes";
            public const string ASIGNAR_MATERIAS = "grupos_estudiantes.asignar_materias";
            public const string DESASIGNAR_MATERIAS = "grupos_estudiantes.desasignar_materias";
            public const string VER_ESTADISTICAS = "grupos_estudiantes.ver_estadisticas";
            public const string EXPORTAR = "grupos_estudiantes.exportar";
            public const string EXPORTAR_ESTUDIANTES = "grupos_estudiantes.exportar_estudiantes";
            public const string GESTIONAR_ASIGNACIONES = "grupos_estudiantes.gestionar_asignaciones";
        }

        /// <summary>
        /// Permisos relacionados con materias
        /// </summary>
        public static class Materias
        {
            public const string VER = "materias.ver";
            public const string CREAR = "materias.crear";
            public const string EDITAR = "materias.editar";
            public const string ELIMINAR = "materias.eliminar";
            public const string VER_DETALLES = "materias.ver_detalles";
            public const string ASIGNAR_INSTRUMENTOS = "materias.asignar_instrumentos";
            public const string QUITAR_INSTRUMENTOS = "materias.quitar_instrumentos";
            public const string VER_INSTRUMENTOS = "materias.ver_instrumentos";
            public const string GESTIONAR_ASIGNACIONES = "materias.gestionar_asignaciones";
        }

        /// <summary>
        /// Permisos relacionados con relaciones instrumento-materia
        /// </summary>
        public static class InstrumentoMaterias
        {
            public const string VER = "instrumento_materias.ver";
            public const string CREAR = "instrumento_materias.crear";
            public const string EDITAR = "instrumento_materias.editar";
            public const string ELIMINAR = "instrumento_materias.eliminar";
            public const string VER_DETALLES = "instrumento_materias.ver_detalles";
            public const string GESTIONAR_RELACIONES = "instrumento_materias.gestionar_relaciones";
        }

        /// <summary>
        /// Permisos relacionados con relaciones instrumento-rúbrica
        /// </summary>
        public static class InstrumentoRubrica
        {
            public const string VER = "instrumento_rubrica.ver";
            public const string CREAR = "instrumento_rubrica.crear";
            public const string EDITAR = "instrumento_rubrica.editar";
            public const string ELIMINAR = "instrumento_rubrica.eliminar";
            public const string VER_DETALLES = "instrumento_rubrica.ver_detalles";
            public const string VER_POR_INSTRUMENTO = "instrumento_rubrica.ver_por_instrumento";
            public const string VER_POR_RUBRICA = "instrumento_rubrica.ver_por_rubrica";
            public const string ASIGNAR_MATERIA = "instrumento_rubrica.asignar_materia";
            public const string GESTIONAR_RELACIONES = "instrumento_rubrica.gestionar_relaciones";
        }

        /// <summary>
        /// Permisos relacionados con cuaderno calificador
        /// </summary>
        public static class CuadernoCalificador
        {
            public const string VER = "cuaderno_calificador.ver";
            public const string VER_TRADICIONAL = "cuaderno_calificador.ver_tradicional";
            public const string EXPORTAR_EXCEL = "cuaderno_calificador.exportar_excel";
            public const string EXPORTAR_DINAMICO = "cuaderno_calificador.exportar_dinamico";
            public const string GESTIONAR = "cuaderno_calificador.gestionar";
        }

        /// <summary>
        /// Permisos relacionados con administración general del sistema
        /// </summary>
        public static class Admin
        {
            public const string VER_DASHBOARD = "admin.ver_dashboard";
            public const string INICIALIZAR_SISTEMA = "admin.inicializar_sistema";
            public const string SINCRONIZAR_PERMISOS = "admin.sincronizar_permisos";
            public const string VER_ESTADISTICAS = "admin.ver_estadisticas";
            public const string VERIFICAR_SALUD = "admin.verificar_salud";
            public const string GESTIONAR_SISTEMA = "admin.gestionar_sistema";
            public const string ACCESO_COMPLETO = "admin.acceso_completo";
        }

        /// <summary>
        /// Obtiene todos los permisos del sistema agrupados por categoría
        /// </summary>
        public static Dictionary<string, List<PermissionInfo>> GetPermissionsByCategory()
        {
            return GetAllPermissionsGrouped();
        }

        /// <summary>
        /// Obtiene todos los permisos como una lista de strings
        /// </summary>
        public static List<string> GetAllPermissions()
        {
            return GetAllPermissionsList().Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Obtiene todos los permisos del sistema agrupados por categoría (implementación interna)
        /// </summary>
        private static Dictionary<string, List<PermissionInfo>> GetAllPermissionsGrouped()
        {
            return new Dictionary<string, List<PermissionInfo>>()
            {
                [USUARIOS] = new List<PermissionInfo>
                {
                    new(Usuarios.VER, "Ver usuarios", "Permite ver la lista de usuarios del sistema"),
                    new(Usuarios.CREAR, "Crear usuarios", "Permite crear nuevos usuarios"),
                    new(Usuarios.EDITAR, "Editar usuarios", "Permite editar información de usuarios"),
                    new(Usuarios.ELIMINAR, "Eliminar usuarios", "Permite eliminar usuarios del sistema"),
                    new(Usuarios.BLOQUEAR, "Bloquear usuarios", "Permite bloquear/desbloquear usuarios"),
                    new(Usuarios.CAMBIAR_ROLES, "Cambiar roles", "Permite asignar y quitar roles a usuarios"),
                    new(Usuarios.VER_PERFIL_COMPLETO, "Ver perfil completo", "Permite ver información detallada de usuarios"),
                    new(Usuarios.EXPORTAR, "Exportar usuarios", "Permite exportar lista de usuarios"),
                    new(Usuarios.IMPORTAR, "Importar usuarios", "Permite importar usuarios desde archivos"),
                    new(Usuarios.ManageRoles, "Gestionar roles", "Permite gestionar roles de usuarios"),
                    new(Usuarios.ManagePermissions, "Gestionar permisos", "Permite gestionar permisos de usuarios"),
                    new(Usuarios.ResetPassword, "Restablecer contraseña", "Permite restablecer contraseñas de usuarios")
                },
                [RUBRICAS] = new List<PermissionInfo>
                {
                    new(Rubricas.VER, "Ver rúbricas", "Permite ver rúbricas propias"),
                    new(Rubricas.VER_TODAS, "Ver todas las rúbricas", "Permite ver todas las rúbricas del sistema"),
                    new(Rubricas.CREAR, "Crear rúbricas", "Permite crear nuevas rúbricas"),
                    new(Rubricas.EDITAR, "Editar rúbricas", "Permite editar rúbricas propias"),
                    new(Rubricas.EDITAR_TODAS, "Editar todas las rúbricas", "Permite editar cualquier rúbrica"),
                    new(Rubricas.ELIMINAR, "Eliminar rúbricas", "Permite eliminar rúbricas propias"),
                    new(Rubricas.ELIMINAR_TODAS, "Eliminar todas las rúbricas", "Permite eliminar cualquier rúbrica"),
                    new(Rubricas.DUPLICAR, "Duplicar rúbricas", "Permite duplicar rúbricas"),
                    new(Rubricas.PUBLICAR, "Publicar rúbricas", "Permite publicar rúbricas"),
                    new(Rubricas.ARCHIVAR, "Archivar rúbricas", "Permite archivar rúbricas"),
                    new(Rubricas.COMPARTIR, "Compartir rúbricas", "Permite compartir rúbricas con otros usuarios"),
                    new(Rubricas.EXPORTAR, "Exportar rúbricas", "Permite exportar rúbricas"),
                    new(Rubricas.IMPORTAR, "Importar rúbricas", "Permite importar rúbricas")
                },
                [EVALUACIONES] = new List<PermissionInfo>
                {
                    new(Evaluaciones.VER, "Ver evaluaciones", "Permite ver evaluaciones propias"),
                    new(Evaluaciones.VER_TODAS, "Ver todas las evaluaciones", "Permite ver todas las evaluaciones"),
                    new(Evaluaciones.CREAR, "Crear evaluaciones", "Permite crear nuevas evaluaciones"),
                    new(Evaluaciones.EDITAR, "Editar evaluaciones", "Permite editar evaluaciones propias"),
                    new(Evaluaciones.EDITAR_TODAS, "Editar todas las evaluaciones", "Permite editar cualquier evaluación"),
                    new(Evaluaciones.ELIMINAR, "Eliminar evaluaciones", "Permite eliminar evaluaciones propias"),
                    new(Evaluaciones.ELIMINAR_TODAS, "Eliminar todas las evaluaciones", "Permite eliminar cualquier evaluación"),
                    new(Evaluaciones.EVALUAR, "Evaluar", "Permite realizar evaluaciones"),
                    new(Evaluaciones.REVISAR, "Revisar evaluaciones", "Permite revisar evaluaciones"),
                    new(Evaluaciones.APROBAR, "Aprobar evaluaciones", "Permite aprobar evaluaciones"),
                    new(Evaluaciones.FINALIZAR, "Finalizar evaluaciones", "Permite finalizar evaluaciones"),
                    new(Evaluaciones.REABRIR, "Reabrir evaluaciones", "Permite reabrir evaluaciones finalizadas"),
                    new(Evaluaciones.EXPORTAR, "Exportar evaluaciones", "Permite exportar evaluaciones"),
                    new(Evaluaciones.VER_RESULTADOS, "Ver resultados", "Permite ver resultados de evaluaciones"),
                    new(Evaluaciones.VER_ESTADISTICAS, "Ver estadísticas", "Permite ver estadísticas de evaluaciones"),
                    
                    // Items de evaluación
                    new(Evaluaciones.ITEMS_VER, "Ver items de evaluación", "Permite ver items de evaluación"),
                    new(Evaluaciones.ITEMS_CREAR, "Crear items de evaluación", "Permite crear nuevos items de evaluación"),
                    new(Evaluaciones.ITEMS_EDITAR, "Editar items de evaluación", "Permite editar items de evaluación"),
                    new(Evaluaciones.ITEMS_ELIMINAR, "Eliminar items de evaluación", "Permite eliminar items de evaluación")
                },
                [ITEMS_EVALUACION] = new List<PermissionInfo>
                {
                    new(ItemsEvaluacion.VER, "Ver items", "Permite ver items de evaluación"),
                    new(ItemsEvaluacion.CREAR, "Crear items", "Permite crear nuevos items de evaluación"),
                    new(ItemsEvaluacion.EDITAR, "Editar items", "Permite editar items de evaluación existentes"),
                    new(ItemsEvaluacion.ELIMINAR, "Eliminar items", "Permite eliminar items de evaluación"),
                    new(ItemsEvaluacion.DUPLICAR, "Duplicar items", "Permite duplicar items de evaluación"),
                    new(ItemsEvaluacion.IMPORTAR, "Importar items", "Permite importar items desde archivos"),
                    new(ItemsEvaluacion.EXPORTAR, "Exportar items", "Permite exportar items de evaluación"),
                    new(ItemsEvaluacion.REORDENAR, "Reordenar items", "Permite cambiar el orden de los items"),
                    new(ItemsEvaluacion.GESTIONAR_CATEGORIAS, "Gestionar categorías", "Permite gestionar categorías de items")
                },
                [INSTRUMENTOS_EVALUACION] = new List<PermissionInfo>
                {
                    new(InstrumentosEvaluacion.VER, "Ver instrumentos", "Permite ver instrumentos de evaluación"),
                    new(InstrumentosEvaluacion.CREAR, "Crear instrumentos", "Permite crear nuevos instrumentos de evaluación"),
                    new(InstrumentosEvaluacion.EDITAR, "Editar instrumentos", "Permite editar instrumentos de evaluación"),
                    new(InstrumentosEvaluacion.ELIMINAR, "Eliminar instrumentos", "Permite eliminar instrumentos de evaluación"),
                    new(InstrumentosEvaluacion.DUPLICAR, "Duplicar instrumentos", "Permite duplicar instrumentos de evaluación"),
                    new(InstrumentosEvaluacion.ASIGNAR_MATERIAS, "Asignar materias", "Permite asignar instrumentos a materias"),
                    new(InstrumentosEvaluacion.ACTIVAR_DESACTIVAR, "Activar/Desactivar", "Permite activar o desactivar instrumentos"),
                    new(InstrumentosEvaluacion.GESTIONAR_CONFIGURACION, "Gestionar configuración", "Permite gestionar configuración de instrumentos"),
                    new(InstrumentosEvaluacion.EXPORTAR, "Exportar instrumentos", "Permite exportar instrumentos de evaluación"),
                    new(InstrumentosEvaluacion.IMPORTAR, "Importar instrumentos", "Permite importar instrumentos de evaluación"),
                    new(InstrumentosEvaluacion.VER_ESTADISTICAS, "Ver estadísticas", "Permite ver estadísticas de uso de instrumentos"),
                    new(InstrumentosEvaluacion.PUBLICAR, "Publicar instrumentos", "Permite publicar instrumentos para uso")
                },
                [ESTUDIANTES] = new List<PermissionInfo>
                {
                    new(Estudiantes.VER, "Ver estudiantes", "Permite ver lista de estudiantes"),
                    new(Estudiantes.CREAR, "Crear estudiantes", "Permite crear nuevos estudiantes"),
                    new(Estudiantes.EDITAR, "Editar estudiantes", "Permite editar información de estudiantes"),
                    new(Estudiantes.ELIMINAR, "Eliminar estudiantes", "Permite eliminar estudiantes"),
                    new(Estudiantes.IMPORTAR, "Importar estudiantes", "Permite importar estudiantes desde archivos"),
                    new(Estudiantes.EXPORTAR, "Exportar estudiantes", "Permite exportar lista de estudiantes"),
                    new(Estudiantes.VER_HISTORIAL, "Ver historial", "Permite ver historial académico de estudiantes"),
                    new(Estudiantes.VER_NOTAS, "Ver notas", "Permite ver calificaciones de estudiantes"),
                    new(Estudiantes.EDITAR_NOTAS, "Editar notas", "Permite modificar calificaciones de estudiantes")
                },
                [INSTITUCIONES] = new List<PermissionInfo>
                {
                    new(Instituciones.VER, "Ver instituciones", "Permite ver lista de instituciones educativas"),
                    new(Instituciones.CREAR, "Crear instituciones", "Permite crear nuevas instituciones"),
                    new(Instituciones.EDITAR, "Editar instituciones", "Permite editar información de instituciones"),
                    new(Instituciones.ELIMINAR, "Eliminar instituciones", "Permite eliminar instituciones"),
                    new(Instituciones.VER_DETALLES, "Ver detalles", "Permite ver información detallada de instituciones"),
                    new(Instituciones.EXPORTAR, "Exportar instituciones", "Permite exportar lista de instituciones")
                },
                [PROFESORES] = new List<PermissionInfo>
                {
                    new(Profesores.VER, "Ver profesores", "Permite ver lista de profesores"),
                    new(Profesores.CREAR, "Crear profesores", "Permite crear nuevos profesores"),
                    new(Profesores.EDITAR, "Editar profesores", "Permite editar información de profesores"),
                    new(Profesores.ELIMINAR, "Eliminar profesores", "Permite eliminar profesores"),
                    new(Profesores.VER_DETALLES, "Ver detalles", "Permite ver información detallada de profesores"),
                    new(Profesores.IMPORTAR, "Importar profesores", "Permite importar profesores desde archivos"),
                    new(Profesores.EXPORTAR, "Exportar profesores", "Permite exportar lista de profesores"),
                    new(Profesores.VER_HISTORIAL, "Ver historial", "Permite ver historial profesional de profesores"),
                    new(Profesores.ASIGNAR_GRUPOS, "Asignar grupos", "Permite asignar grupos y materias a profesores"),
                    new(Profesores.GESTIONAR_HORARIOS, "Gestionar horarios", "Permite gestionar horarios de profesores"),
                    new(Profesores.VER_ESTADISTICAS, "Ver estadísticas", "Permite ver estadísticas de profesores"),
                    new(Profesores.CAMBIAR_ESTADO, "Cambiar estado", "Permite activar/desactivar profesores")
                },
                [ASISTENCIA] = new List<PermissionInfo>
                {
                    new(Asistencia.VER, "Ver asistencia", "Permite ver registros de asistencia y lista de grupos"),
                    new(Asistencia.TOMAR_ASISTENCIA, "Tomar asistencia", "Permite tomar asistencia/pase de lista a estudiantes"),
                    new(Asistencia.EDITAR_ASISTENCIA, "Editar asistencia", "Permite modificar registros de asistencia existentes"),
                    new(Asistencia.ELIMINAR_REGISTROS, "Eliminar registros", "Permite eliminar registros de asistencia"),
                    new(Asistencia.VER_RESUMEN, "Ver resumen", "Permite ver resúmenes y estadísticas de asistencia"),
                    new(Asistencia.IMPRIMIR, "Imprimir resumen", "Permite imprimir reportes de resumen de asistencia en PDF"),
                    new(Asistencia.VER_DIAGNOSTICO, "Ver diagnóstico", "Permite ver información de diagnóstico del sistema"),
                    new(Asistencia.EXPORTAR, "Exportar asistencia", "Permite exportar reportes de asistencia a Excel"),
                    new(Asistencia.GESTIONAR_GRUPOS, "Gestionar grupos", "Permite gestionar grupos de estudiantes para asistencia"),
                    new(Asistencia.VER_ESTADISTICAS, "Ver estadísticas", "Permite ver estadísticas detalladas de asistencia"),
                    new(Asistencia.CONFIGURAR_PARAMETROS, "Configurar parámetros", "Permite configurar parámetros del sistema de asistencia")
                },
                [REPORTES] = new List<PermissionInfo>
                {
                    new(Reportes.VER_BASICOS, "Ver reportes básicos", "Permite ver reportes básicos del sistema"),
                    new(Reportes.VER_AVANZADOS, "Ver reportes avanzados", "Permite ver reportes avanzados"),
                    new(Reportes.VER_TODOS, "Ver todos los reportes", "Permite acceso a todos los reportes"),
                    new(Reportes.CREAR_PERSONALIZADOS, "Crear reportes personalizados", "Permite crear reportes personalizados"),
                    new(Reportes.EXPORTAR, "Exportar reportes", "Permite exportar reportes"),
                    new(Reportes.PROGRAMAR, "Programar reportes", "Permite programar generación automática de reportes"),
                    new(Reportes.VER_ESTADISTICAS_INSTITUCIONALES, "Ver estadísticas institucionales", "Permite ver estadísticas a nivel institucional")
                },
                [CONFIGURACION] = new List<PermissionInfo>
                {
                    new(Configuracion.VER, "Ver configuración", "Permite ver configuración del sistema"),
                    new(Configuracion.EDITAR_SISTEMA, "Editar configuración del sistema", "Permite modificar configuración general"),
                    new(Configuracion.EDITAR_SEGURIDAD, "Editar configuración de seguridad", "Permite modificar configuración de seguridad"),
                    new(Configuracion.GESTIONAR_ROLES, "Gestionar roles", "Permite crear y modificar roles"),
                    new(Configuracion.GESTIONAR_PERMISOS, "Gestionar permisos", "Permite asignar permisos a roles"),
                    new(Configuracion.BACKUP, "Realizar backup", "Permite realizar copias de seguridad"),
                    new(Configuracion.RESTAURAR, "Restaurar backup", "Permite restaurar copias de seguridad"),
                    new(Configuracion.VER_LOGS, "Ver logs", "Permite ver logs del sistema"),
                    new(Configuracion.LIMPIAR_LOGS, "Limpiar logs", "Permite limpiar logs del sistema"),
                    new(Configuracion.MODO_MANTENIMIENTO, "Modo mantenimiento", "Permite activar modo mantenimiento"),
                    new(Configuracion.CONFIGURAR_EMAIL, "Configurar email", "Permite configurar parámetros SMTP del sistema"),
                    new(Configuracion.Manage, "Gestionar configuración", "Permite gestionar toda la configuración del sistema"),
                    new(Configuracion.Initialize, "Inicializar sistema", "Permite inicializar componentes del sistema"),
                    new(Configuracion.SyncPermissions, "Sincronizar permisos", "Permite sincronizar permisos del sistema"),
                    new(Configuracion.HealthCheck, "Verificar estado", "Permite verificar el estado del sistema")
                },
                [AUDITORIA] = new List<PermissionInfo>
                {
                    new(Auditoria.VER, "Ver auditoría", "Permite ver registros de auditoría"),
                    new(Auditoria.VER_ACCESOS, "Ver log de accesos", "Permite ver registros de acceso al sistema"),
                    new(Auditoria.VER_CAMBIOS, "Ver log de cambios", "Permite ver registros de cambios en datos"),
                    new(Auditoria.VER_ERRORES, "Ver log de errores", "Permite ver registros de errores del sistema"),
                    new(Auditoria.EXPORTAR, "Exportar logs", "Permite exportar registros de auditoría"),
                    new(Auditoria.LIMPIAR, "Limpiar logs", "Permite limpiar registros de auditoría"),
                    new(Auditoria.VER_METRICAS, "Ver métricas", "Permite ver métricas de uso del sistema")
                },
                [PERIODOS] = new List<PermissionInfo>
                {
                    new(Periodos.VER, "Ver períodos", "Permite ver períodos académicos"),
                    new(Periodos.CREAR, "Crear períodos", "Permite crear nuevos períodos académicos"),
                    new(Periodos.EDITAR, "Editar períodos", "Permite editar períodos académicos"),
                    new(Periodos.ELIMINAR, "Eliminar períodos", "Permite eliminar períodos académicos"),
                    new(Periodos.ACTIVAR, "Activar períodos", "Permite activar períodos académicos"),
                    new(Periodos.CERRAR, "Cerrar períodos", "Permite cerrar períodos académicos"),
                    new(Periodos.GESTIONAR_CALENDARIO, "Gestionar calendario", "Permite gestionar calendario académico")
                },
                [NIVELES] = new List<PermissionInfo>
                {
                    new(Niveles.VER, "Ver niveles", "Permite ver niveles de calificación"),
                    new(Niveles.CREAR, "Crear niveles", "Permite crear nuevos niveles"),
                    new(Niveles.EDITAR, "Editar niveles", "Permite editar niveles de calificación"),
                    new(Niveles.ELIMINAR, "Eliminar niveles", "Permite eliminar niveles de calificación"),
                    new(Niveles.REORDENAR, "Reordenar niveles", "Permite cambiar orden de niveles"),
                    new(Niveles.GESTIONAR_GRUPOS, "Gestionar grupos", "Permite gestionar grupos de calificación")
                },
                [TIPOS_GRUPO] = new List<PermissionInfo>
                {
                    new(TiposGrupo.VER, "Ver tipos de grupo", "Permite ver lista de tipos de grupo"),
                    new(TiposGrupo.CREAR, "Crear tipos de grupo", "Permite crear nuevos tipos de grupo"),
                    new(TiposGrupo.EDITAR, "Editar tipos de grupo", "Permite editar tipos de grupo existentes"),
                    new(TiposGrupo.ELIMINAR, "Eliminar tipos de grupo", "Permite eliminar tipos de grupo"),
                    new(TiposGrupo.VER_DETALLES, "Ver detalles", "Permite ver información detallada de tipos de grupo"),
                    new(TiposGrupo.CAMBIAR_ESTADO, "Cambiar estado", "Permite activar/desactivar tipos de grupo"),
                    new(TiposGrupo.GESTIONAR, "Gestionar tipos", "Permite gestión completa de tipos de grupo")
                },
                [GRUPOS_ESTUDIANTES] = new List<PermissionInfo>
                {
                    new(GruposEstudiantes.VER, "Ver grupos", "Permite ver lista de grupos de estudiantes"),
                    new(GruposEstudiantes.CREAR, "Crear grupos", "Permite crear nuevos grupos de estudiantes"),
                    new(GruposEstudiantes.EDITAR, "Editar grupos", "Permite editar información de grupos"),
                    new(GruposEstudiantes.ELIMINAR, "Eliminar grupos", "Permite eliminar grupos de estudiantes"),
                    new(GruposEstudiantes.VER_DETALLES, "Ver detalles", "Permite ver información detallada de grupos"),
                    new(GruposEstudiantes.ASIGNAR_ESTUDIANTES, "Asignar estudiantes", "Permite asignar estudiantes a grupos"),
                    new(GruposEstudiantes.DESASIGNAR_ESTUDIANTES, "Desasignar estudiantes", "Permite quitar estudiantes de grupos"),
                    new(GruposEstudiantes.ASIGNAR_MATERIAS, "Asignar materias", "Permite asignar materias a grupos"),
                    new(GruposEstudiantes.DESASIGNAR_MATERIAS, "Desasignar materias", "Permite quitar materias de grupos"),
                    new(GruposEstudiantes.VER_ESTADISTICAS, "Ver estadísticas", "Permite ver estadísticas de grupos"),
                    new(GruposEstudiantes.EXPORTAR, "Exportar grupos", "Permite exportar información de grupos"),
                    new(GruposEstudiantes.EXPORTAR_ESTUDIANTES, "Exportar estudiantes", "Permite exportar lista de estudiantes por grupo"),
                    new(GruposEstudiantes.GESTIONAR_ASIGNACIONES, "Gestionar asignaciones", "Permite gestionar todas las asignaciones de grupos")
                },
                [MATERIAS] = new List<PermissionInfo>
                {
                    new(Materias.VER, "Ver materias", "Permite ver lista de materias"),
                    new(Materias.CREAR, "Crear materias", "Permite crear nuevas materias"),
                    new(Materias.EDITAR, "Editar materias", "Permite editar información de materias"),
                    new(Materias.ELIMINAR, "Eliminar materias", "Permite eliminar materias"),
                    new(Materias.VER_DETALLES, "Ver detalles", "Permite ver información detallada de materias"),
                    new(Materias.ASIGNAR_INSTRUMENTOS, "Asignar instrumentos", "Permite asignar instrumentos de evaluación a materias"),
                    new(Materias.QUITAR_INSTRUMENTOS, "Quitar instrumentos", "Permite quitar instrumentos de materias"),
                    new(Materias.VER_INSTRUMENTOS, "Ver instrumentos", "Permite ver instrumentos asignados a materias"),
                    new(Materias.GESTIONAR_ASIGNACIONES, "Gestionar asignaciones", "Permite gestionar asignaciones de instrumentos")
                },
                [INSTRUMENTO_MATERIAS] = new List<PermissionInfo>
                {
                    new(InstrumentoMaterias.VER, "Ver relaciones", "Permite ver relaciones instrumento-materia"),
                    new(InstrumentoMaterias.CREAR, "Crear relaciones", "Permite crear nuevas relaciones instrumento-materia"),
                    new(InstrumentoMaterias.EDITAR, "Editar relaciones", "Permite editar relaciones existentes"),
                    new(InstrumentoMaterias.ELIMINAR, "Eliminar relaciones", "Permite eliminar relaciones instrumento-materia"),
                    new(InstrumentoMaterias.VER_DETALLES, "Ver detalles", "Permite ver detalles de relaciones"),
                    new(InstrumentoMaterias.GESTIONAR_RELACIONES, "Gestionar relaciones", "Permite gestión completa de relaciones instrumento-materia")
                },
                [INSTRUMENTO_RUBRICA] = new List<PermissionInfo>
                {
                    new(InstrumentoRubrica.VER, "Ver relaciones", "Permite ver relaciones instrumento-rúbrica"),
                    new(InstrumentoRubrica.CREAR, "Crear relaciones", "Permite crear nuevas relaciones instrumento-rúbrica"),
                    new(InstrumentoRubrica.EDITAR, "Editar relaciones", "Permite editar relaciones existentes"),
                    new(InstrumentoRubrica.ELIMINAR, "Eliminar relaciones", "Permite eliminar relaciones instrumento-rúbrica"),
                    new(InstrumentoRubrica.VER_DETALLES, "Ver detalles", "Permite ver detalles de relaciones"),
                    new(InstrumentoRubrica.VER_POR_INSTRUMENTO, "Ver por instrumento", "Permite ver rúbricas por instrumento"),
                    new(InstrumentoRubrica.VER_POR_RUBRICA, "Ver por rúbrica", "Permite ver instrumentos por rúbrica"),
                    new(InstrumentoRubrica.ASIGNAR_MATERIA, "Asignar materia", "Permite asignar materias a relaciones instrumento-rúbrica"),
                    new(InstrumentoRubrica.GESTIONAR_RELACIONES, "Gestionar relaciones", "Permite gestión completa de relaciones instrumento-rúbrica")
                },
                [CUADERNO_CALIFICADOR] = new List<PermissionInfo>
                {
                    new(CuadernoCalificador.VER, "Ver cuaderno", "Permite ver el cuaderno calificador"),
                    new(CuadernoCalificador.VER_TRADICIONAL, "Ver versión tradicional", "Permite ver cuaderno calificador tradicional"),
                    new(CuadernoCalificador.EXPORTAR_EXCEL, "Exportar a Excel", "Permite exportar cuaderno calificador a Excel"),
                    new(CuadernoCalificador.EXPORTAR_DINAMICO, "Exportar dinámico", "Permite exportar versión dinámica del cuaderno"),
                    new(CuadernoCalificador.GESTIONAR, "Gestionar cuaderno", "Permite gestión completa del cuaderno calificador")
                },
                [SISTEMA_SEA] = new List<PermissionInfo>
                {
                    new(SEA.VER_REPORTE, "Ver reporte SEA", "Permite ver el reporte SEA del MEP"),
                    new(SEA.EXPORTAR_CSV, "Exportar CSV", "Permite exportar reporte SEA en formato CSV"),
                    new(SEA.EXPORTAR_EXCEL, "Exportar Excel", "Permite exportar reporte SEA en formato Excel"),
                    new(SEA.EXPORTAR_PDF, "Exportar PDF", "Permite exportar reporte SEA en formato PDF"),
                    new(SEA.CONFIGURAR, "Configurar SEA", "Permite configurar componentes del sistema SEA"),
                    new(SEA.VER_ESTADISTICAS, "Ver estadísticas", "Permite ver estadísticas del reporte SEA")
                },
                [ADMIN] = new List<PermissionInfo>
                {
                    new(Admin.VER_DASHBOARD, "Ver dashboard", "Permite ver el panel de administración"),
                    new(Admin.INICIALIZAR_SISTEMA, "Inicializar sistema", "Permite inicializar componentes del sistema"),
                    new(Admin.SINCRONIZAR_PERMISOS, "Sincronizar permisos", "Permite sincronizar permisos del sistema"),
                    new(Admin.VER_ESTADISTICAS, "Ver estadísticas", "Permite ver estadísticas del sistema"),
                    new(Admin.VERIFICAR_SALUD, "Verificar salud", "Permite verificar el estado de salud del sistema"),
                    new(Admin.GESTIONAR_SISTEMA, "Gestionar sistema", "Permite gestión general del sistema"),
                    new(Admin.ACCESO_COMPLETO, "Acceso completo", "Permite acceso completo a funciones administrativas")
                }
            };
        }

        /// <summary>
        /// Obtiene todos los permisos como una lista plana
        /// </summary>
        public static List<PermissionInfo> GetAllPermissionsList()
        {
            return GetAllPermissionsGrouped().SelectMany(category => category.Value).ToList();
        }

        /// <summary>
        /// Verifica si un permiso existe
        /// </summary>
        public static bool IsValidPermission(string permission)
        {
            return GetAllPermissionsList().Any(p => p.Name == permission);
        }
    }

    /// <summary>
    /// Información de un permiso
    /// </summary>
    public record PermissionInfo(string Name, string DisplayName, string Description);
}
