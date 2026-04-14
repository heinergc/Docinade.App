using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Utils
{
    /// <summary>
    /// Inicializa los datos semilla del módulo de conducta según REA 40862-V21
    /// </summary>
    public static class ConductaSeedData
    {
        /// <summary>
        /// Inicializa tipos de falta y parámetros institucionales
        /// </summary>
        public static async Task<bool> InitializeAsync(RubricasDbContext context)
        {
            try
            {
                // Verificar si ya existen tipos de falta
                var existentes = await context.TiposFalta.AnyAsync();
                if (existentes)
                {
                    Console.WriteLine("✅ Tipos de falta ya inicializados");
                    return true;
                }

                // Crear los 5 tipos de falta según Art. 137 MEP REA 40862-V21
                var tiposFalta = new List<TipoFalta>
                {
                    new TipoFalta
                    {
                        Nombre = "Muy leve",
                        Definicion = "Comportamientos menores que no afectan significativamente la convivencia escolar ni el proceso educativo.",
                        Ejemplos = "Llegar tarde sin justificación, no traer materiales, usar celular en clase (primera vez), mascar chicle, desorganización personal leve",
                        AccionCorrectiva = "Llamado de atención verbal, reflexión con el estudiante, comunicación al profesor guía, compromiso de mejora",
                        RebajoMinimo = 1,
                        RebajoMaximo = 5,
                        Orden = 1,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    },
                    new TipoFalta
                    {
                        Nombre = "Leve",
                        Definicion = "Conductas que alteran levemente el ambiente de aprendizaje o las normas de convivencia establecidas.",
                        Ejemplos = "Uso reiterado de celular en clase, interrupciones constantes, vocabulario inapropiado ocasional, incumplimiento de tareas repetido, desacato leve a instrucciones",
                        AccionCorrectiva = "Amonestación escrita, citación al encargado legal, reflexión escrita del estudiante, acuerdo de mejora con seguimiento, servicio comunitario interno",
                        RebajoMinimo = 6,
                        RebajoMaximo = 10,
                        Orden = 2,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    },
                    new TipoFalta
                    {
                        Nombre = "Grave",
                        Definicion = "Comportamientos que afectan seriamente el ambiente educativo, la convivencia o el respeto entre miembros de la comunidad educativa.",
                        Ejemplos = "Agresión verbal a compañeros, falta de respeto al personal docente o administrativo, daño intencional a propiedad ajena o institucional (menor), fraude académico (copia en exámenes), ausencias injustificadas reiteradas, conducta disruptiva constante",
                        AccionCorrectiva = "Suspensión temporal (1-3 días), entrevista con comité de convivencia, compromiso formal escrito ante encargado legal, programa de servicio comunitario, derivación a orientación o psicología, reposición o reparación de daños",
                        RebajoMinimo = 11,
                        RebajoMaximo = 19,
                        Orden = 3,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    },
                    new TipoFalta
                    {
                        Nombre = "Muy grave",
                        Definicion = "Acciones que atentan gravemente contra la integridad física, psicológica o moral de las personas, o que comprometen seriamente el ambiente educativo.",
                        Ejemplos = "Agresión física a compañeros o personal, acoso escolar (bullying) comprobado, intimidación o amenazas, posesión o consumo de sustancias prohibidas en el centro educativo, robo o hurto, vandalismo institucional grave, suplantación de identidad, falsificación de documentos",
                        AccionCorrectiva = "Suspensión prolongada (4-7 días), denuncia ante autoridades correspondientes si procede, intervención del comité de convivencia institucional, matrícula condicional, programa obligatorio de orientación y seguimiento, reparación económica de daños, posible traslado a otra institución",
                        RebajoMinimo = 20,
                        RebajoMaximo = 32,
                        Orden = 4,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    },
                    new TipoFalta
                    {
                        Nombre = "Gravísima",
                        Definicion = "Conductas que ponen en riesgo la vida, la seguridad o la integridad de las personas, o que constituyen delitos según la legislación costarricense.",
                        Ejemplos = "Agresión física grave con lesiones, porte o uso de armas (de cualquier tipo), tráfico de sustancias ilícitas, abuso sexual o acoso sexual, extorsión, amenazas graves de muerte o daño, actos que pongan en peligro la seguridad del centro educativo, participación en redes delictivas",
                        AccionCorrectiva = "Expulsión inmediata del centro educativo, denuncia obligatoria ante OIJ y Ministerio Público, intervención del PANI si es menor de edad, proceso legal según corresponda, traslado obligatorio a otra institución educativa, seguimiento psicosocial por autoridades competentes",
                        RebajoMinimo = 33,
                        RebajoMaximo = 45,
                        Orden = 5,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    }
                };

                await context.TiposFalta.AddRangeAsync(tiposFalta);
                await context.SaveChangesAsync();
                
                Console.WriteLine("✅ 5 tipos de falta creados según REA 40862-V21 Art. 137");

                // Crear parámetro de nota mínima de aprobación
                var parametroExiste = await context.ParametrosInstitucion
                    .AnyAsync(p => p.Clave == "NotaMinimaAprobacionConducta");

                if (!parametroExiste)
                {
                    var parametro = new ParametroInstitucion
                    {
                        Clave = "NotaMinimaAprobacionConducta",
                        Nombre = "Nota Mínima de Aprobación en Conducta",
                        Descripcion = "Calificación mínima requerida para aprobar conducta según REA 40862-V21. Por defecto 65 puntos sobre 100.",
                        Valor = "65",
                        TipoDato = "Decimal",
                        Categoria = "Conducta",
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    };

                    await context.ParametrosInstitucion.AddAsync(parametro);
                    await context.SaveChangesAsync();
                    
                    Console.WriteLine("✅ Parámetro NotaMinimaAprobacionConducta = 65 creado");
                }

                Console.WriteLine("🎉 Seed data del módulo de conducta inicializado correctamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inicializando seed data de conducta: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene estadísticas de los tipos de falta configurados
        /// </summary>
        public static async Task<(int TotalTipos, int Activos, int Inactivos)> GetEstadisticasAsync(RubricasDbContext context)
        {
            var total = await context.TiposFalta.CountAsync();
            var activos = await context.TiposFalta.CountAsync(t => t.Activo);
            var inactivos = total - activos;

            return (total, activos, inactivos);
        }

        /// <summary>
        /// Verifica la integridad de los datos de conducta
        /// </summary>
        public static async Task<bool> VerificarIntegridadAsync(RubricasDbContext context)
        {
            try
            {
                // Verificar que existan los 5 tipos de falta
                var tiposFalta = await context.TiposFalta.CountAsync();
                if (tiposFalta < 5)
                {
                    Console.WriteLine($"⚠️ Advertencia: Solo hay {tiposFalta} tipos de falta (se esperan 5)");
                    return false;
                }

                // Verificar rangos de rebajos
                var tiposConRangoInvalido = await context.TiposFalta
                    .Where(t => t.RebajoMinimo < 1 || t.RebajoMaximo > 45 || t.RebajoMinimo > t.RebajoMaximo)
                    .CountAsync();

                if (tiposConRangoInvalido > 0)
                {
                    Console.WriteLine($"⚠️ Advertencia: {tiposConRangoInvalido} tipos de falta tienen rangos de rebajo inválidos");
                    return false;
                }

                // Verificar parámetro de nota mínima
                var parametro = await context.ParametrosInstitucion
                    .FirstOrDefaultAsync(p => p.Clave == "NotaMinimaAprobacionConducta");

                if (parametro == null)
                {
                    Console.WriteLine("⚠️ Advertencia: No existe el parámetro NotaMinimaAprobacionConducta");
                    return false;
                }

                if (!decimal.TryParse(parametro.Valor, out decimal notaMinima) || notaMinima < 0 || notaMinima > 100)
                {
                    Console.WriteLine($"⚠️ Advertencia: Nota mínima inválida: {parametro.Valor}");
                    return false;
                }

                Console.WriteLine("✅ Integridad de datos de conducta verificada correctamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error verificando integridad de datos de conducta: {ex.Message}");
                return false;
            }
        }
    }
}
