using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(RubricasDbContext context)
        {
            context.Database.EnsureCreated();
            
            // Verificar y agregar columna Estado si no existe
            EnsureEstadoColumnExists(context);

            // Verificar si ya hay datos
            if (context.PeriodosAcademicos.Any())
            {
                return; // La base de datos ya tiene datos
            }

            // Crear períodos académicos de ejemplo
            var periodos = new PeriodoAcademico[]
            {
                new PeriodoAcademico
                {
                    Codigo = "C1-2025",
                    Nombre = "Primer Cuatrimestre 2025",
                    Tipo = TipoPeriodo.Cuatrimestre,
                    Anio = 2025,
                    Ciclo  = "I",
                    NumeroPeriodo = 1,
                    FechaInicio = new DateTime(2025, 1, 15),
                    FechaFin = new DateTime(2025, 5, 15),
                    Activo = true
                },
                new PeriodoAcademico
                {
                    Codigo = "C2-2025",
                    Nombre = "Segundo Cuatrimestre 2025",
                    Tipo = TipoPeriodo.Cuatrimestre,
                    Anio = 2025,
                    Ciclo  = "II",  
                    NumeroPeriodo = 2,
                    FechaInicio = new DateTime(2025, 5, 20),
                    FechaFin = new DateTime(2025, 9, 20),
                    Activo = false
                },
                new PeriodoAcademico
                {
                    Codigo = "S1-2025",
                    Nombre = "Primer Semestre 2025",
                    Tipo = TipoPeriodo.Semestre,
                    Anio = 2025,
                    Ciclo  = "SI",
                    NumeroPeriodo = 1,
                    FechaInicio = new DateTime(2025, 1, 10),
                    FechaFin = new DateTime(2025, 6, 30),
                    Activo = false
                }
            };

            context.PeriodosAcademicos.AddRange(periodos);
            context.SaveChanges();

            // Crear niveles de calificación
            var niveles = new NivelCalificacion[]
            {
                new NivelCalificacion { NombreNivel = "Excelente", Descripcion = "Cumple completamente con los criterios", OrdenNivel = 1 },
                new NivelCalificacion { NombreNivel = "Bueno", Descripcion = "Cumple parcialmente con los criterios", OrdenNivel = 2 },
                new NivelCalificacion { NombreNivel = "Regular", Descripcion = "Cumple mínimamente con los criterios", OrdenNivel = 3 },
                new NivelCalificacion { NombreNivel = "Deficiente", Descripcion = "No cumple con los criterios", OrdenNivel = 4 }
            };

            context.NivelesCalificacion.AddRange(niveles);
            context.SaveChanges();

            // Crear una rúbrica de ejemplo
            var rubrica = new Rubrica
            {
                NombreRubrica = "Evaluación de Proyecto Final",
                Descripcion = "Rúbrica para evaluar proyectos finales de curso",
                Estado = "ACTIVO",
                FechaCreacion = DateTime.Now
            };

            context.Rubricas.Add(rubrica);
            context.SaveChanges();

            // Crear items de evaluación
            var items = new ItemEvaluacion[]
            {
                new ItemEvaluacion { NombreItem = "Contenido y Organización", IdRubrica = rubrica.IdRubrica, OrdenItem = 1 },
                new ItemEvaluacion { NombreItem = "Presentación y Diseño", IdRubrica = rubrica.IdRubrica, OrdenItem = 2 },
                new ItemEvaluacion { NombreItem = "Calidad Técnica", IdRubrica = rubrica.IdRubrica, OrdenItem = 3 },
                new ItemEvaluacion { NombreItem = "Creatividad e Innovación", IdRubrica = rubrica.IdRubrica, OrdenItem = 4 }
            };

            context.ItemsEvaluacion.AddRange(items);
            context.SaveChanges();

            // Crear valores de rúbrica (puntos por item/nivel)
            var valores = new List<ValorRubrica>();
            decimal[] puntajes = { 25, 20, 15, 10 }; // Excelente, Bueno, Regular, Deficiente

            foreach (var item in items)
            {
                for (int i = 0; i < niveles.Length; i++)
                {
                    valores.Add(new ValorRubrica
                    {
                        IdRubrica = rubrica.IdRubrica,
                        IdItem = item.IdItem,
                        IdNivel = niveles[i].IdNivel,
                        ValorPuntos = puntajes[i]
                    });
                }
            }

            context.ValoresRubrica.AddRange(valores);
            context.SaveChanges();

            // Crear algunos estudiantes de ejemplo
            var estudiantes = new Estudiante[]
            {
                new Estudiante
                {
                    Nombre = "LADY YARENIS",
                    Apellidos = "ABARCA BRENES",
                    NumeroId = "0208390339",
                    DireccionCorreo = "lady.abarca@uned.cr",
                    Institucion = "PALMARES (06)",
                    Grupos = "Grupo 02: Tutor Heiner Guido Cambronero",
                    Anio = 2025,
                    PeriodoAcademicoId = periodos[0].Id
                },
                new Estudiante
                {
                    Nombre = "NIKOL NAOMY",
                    Apellidos = "ALFARO SERRANO",
                    NumeroId = "0208570391",
                    DireccionCorreo = "nikol.alfaro@uned.cr",
                    Institucion = "ATENAS (35)",
                    Grupos = "Grupo 02: Tutor Heiner Guido Cambronero",
                    Anio = 2025,
                    PeriodoAcademicoId = periodos[0].Id
                },
                new Estudiante
                {
                    Nombre = "CARLOS ALBERTO",
                    Apellidos = "GONZALEZ MORA",
                    NumeroId = "0107890123",
                    DireccionCorreo = "carlos.gonzalez@uned.cr",
                    Institucion = "SAN JOSE (01)",
                    Grupos = "Grupo 01: Tutor María Pérez Castro",
                    Anio = 2025,
                    PeriodoAcademicoId = periodos[0].Id
                }
            };

            context.Estudiantes.AddRange(estudiantes);
            context.SaveChanges();

            // Crear configuraciones iniciales del sistema
            var configuraciones = new ConfiguracionSistema[]
            {
                new ConfiguracionSistema
                {
                    Clave = ConfiguracionClaves.ModoRegistroUsuarios,
                    Valor = ModoRegistroUsuarios.Abierto.ToString(),
                    Descripcion = "Controla si el registro público de usuarios está habilitado (Abierto) o deshabilitado (Cerrado)",
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    UsuarioModificacion = "Sistema"
                },
                new ConfiguracionSistema
                {
                    Clave = ConfiguracionClaves.MensajeRegistroCerrado,
                    Valor = "El registro de cuentas está deshabilitado. Contacte al administrador para obtener acceso al sistema.",
                    Descripcion = "Mensaje mostrado a los usuarios cuando intentan registrarse y el sistema está en modo cerrado",
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    UsuarioModificacion = "Sistema"
                },
                new ConfiguracionSistema
                {
                    Clave = ConfiguracionClaves.PermitirInvitacionesEspeciales,
                    Valor = "false",
                    Descripcion = "Permite el acceso mediante invitaciones especiales incluso cuando el registro está cerrado (funcionalidad futura)",
                    FechaCreacion = DateTime.UtcNow,
                    FechaModificacion = DateTime.UtcNow,
                    UsuarioModificacion = "Sistema"
                }
            };

            context.ConfiguracionesSistema.AddRange(configuraciones);
            context.SaveChanges();
        }
        
        private static void EnsureEstadoColumnExists(RubricasDbContext context)
        {
            try
            {
                // Obtener la cadena de conexión
                var connectionString = context.Database.GetConnectionString();
                
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    
                    // Verificar si la columna Estado existe
                    var command = connection.CreateCommand();
                    command.CommandText = "PRAGMA table_info(Evaluaciones)";
                    
                    bool estadoExists = false;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"].ToString() == "Estado")
                            {
                                estadoExists = true;
                                break;
                            }
                        }
                    }
                    
                    // Si la columna no existe, agregarla
                    if (!estadoExists)
                    {
                        Console.WriteLine("Agregando columna Estado a la tabla Evaluaciones...");
                        
                        // Agregar la columna
                        command = connection.CreateCommand();
                        command.CommandText = "ALTER TABLE Evaluaciones ADD COLUMN Estado TEXT DEFAULT 'BORRADOR'";
                        command.ExecuteNonQuery();
                        
                        // Actualizar registros existentes
                        command = connection.CreateCommand();
                        command.CommandText = "UPDATE Evaluaciones SET Estado = 'COMPLETADA' WHERE Estado IS NULL OR Estado = ''";
                        var affectedRows = command.ExecuteNonQuery();
                        
                        Console.WriteLine($"Columna Estado agregada. Se actualizaron {affectedRows} registros existentes.");
                    }
                    
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar/agregar columna Estado: {ex.Message}");
                // No lanzar excepción para no interrumpir la inicialización
            }
        }
    }
}