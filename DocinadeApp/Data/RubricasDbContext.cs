using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Models;
using DocinadeApp.Models.Identity;
using DocinadeApp.Models.Audit;
using DocinadeApp.Models.Academic;
using DocinadeApp.Models.SEA;

namespace DocinadeApp.Data
{
    public class RubricasDbContext : IdentityDbContext<ApplicationUser>
    {
        public RubricasDbContext(DbContextOptions<RubricasDbContext> options) : base(options)
        {
        }

        public DbSet<Rubrica> Rubricas { get; set; }
        public DbSet<ItemEvaluacion> ItemsEvaluacion { get; set; }
        public DbSet<NivelCalificacion> NivelesCalificacion { get; set; }
        public DbSet<GrupoCalificacion> GruposCalificacion { get; set; }
        public DbSet<ValorRubrica> ValoresRubrica { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        public DbSet<DetalleEvaluacion> DetallesEvaluacion { get; set; }
        public DbSet<PeriodoAcademico> PeriodosAcademicos { get; set; }
        public DbSet<RubricaNivel> RubricaNiveles { get; set; }
        public DbSet<ConfiguracionSistema> ConfiguracionesSistema { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<MateriaRequisito> MateriaRequisitos { get; set; }
        public DbSet<MateriaPeriodo> MateriaPeriodos { get; set; }
        public DbSet<InstrumentoEvaluacion> InstrumentosEvaluacion { get; set; }
        public DbSet<InstrumentoRubrica> InstrumentoRubricas { get; set; }
        public DbSet<InstrumentoMateria> InstrumentoMaterias { get; set; }
        public DbSet<ConfiguracionComponenteSEA> ConfiguracionesComponenteSEA { get; set; }
        public DbSet<CuadernoCalificador> CuadernosCalificadores { get; set; }
        public DbSet<CuadernoInstrumento> CuadernoInstrumentos { get; set; }

        // Nuevas tablas para el sistema de grupos
        public DbSet<GrupoEstudiante> GruposEstudiantes { get; set; }
        public DbSet<EstudianteGrupo> EstudianteGrupos { get; set; }
        public DbSet<GrupoMateria> GrupoMaterias { get; set; }

        // 🔧 NUEVA: Tabla de asistencias
        public DbSet<Asistencia> Asistencias { get; set; }

        // 🔧 NUEVA: Tabla de lecciones/bloques horarios (especificación MEP)
        public DbSet<Leccion> Lecciones { get; set; }

        // 🔧 NUEVA: Tabla de catálogo de tipos de grupo
        public DbSet<TipoGrupoCatalogo> TiposGrupo { get; set; }

        // 🔧 NUEVA: Tabla de auditoría de operaciones
        public DbSet<AuditoriaOperacion> AuditoriasOperaciones { get; set; }

        // 🔧 NUEVA: Tabla de empadronamiento estudiantil
        public DbSet<EstudianteEmpadronamiento> EstudiantesEmpadronamiento { get; set; }

        // 🔧 NUEVA: Tabla de configuración ACS por estudiante e instrumento
        public DbSet<EstudianteInstrumentoACS> EstudiantesInstrumentosACS { get; set; }

        // 🔧 NUEVA: Entidades geográficas de Costa Rica (Profesores)
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Canton> Cantones { get; set; }
        public DbSet<Distrito> Distritos { get; set; }
        public DbSet<Institucion> Instituciones { get; set; }
        public DbSet<Facultad> Facultades { get; set; }
        public DbSet<Escuela> Escuelas { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<ProfesorFormacionAcademica> ProfesorFormacionAcademica { get; set; }
        public DbSet<ProfesorExperienciaLaboral> ProfesorExperienciaLaboral { get; set; }
        public DbSet<ProfesorCapacitacion> ProfesorCapacitacion { get; set; }
        public DbSet<ProfesorGrupo> ProfesorGrupo { get; set; }
        public DbSet<ProfesorGuia> ProfesorGuia { get; set; }

        // MÓDULO DE CONDUCTA - REA 40862-V21
        public DbSet<TipoFalta> TiposFalta { get; set; }
        public DbSet<BoletaConducta> BoletasConducta { get; set; }
        public DbSet<NotaConducta> NotasConducta { get; set; }
        public DbSet<ProgramaAccionesInstitucional> ProgramasAccionesInstitucional { get; set; }
        public DbSet<DecisionProfesionalConducta> DecisionesProfesionalesConducta { get; set; }
        public DbSet<ParametroInstitucion> ParametrosInstitucion { get; set; }

        // MÓDULO DE SLIDER DINÁMICO
        public DbSet<SliderItem> SliderItems { get; set; }

        // MÓDULO DE EXCEPCIONES
        public DbSet<ExcepcionSistema> ExcepcionesSistema { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llamar al método base para configurar Identity
            base.OnModelCreating(modelBuilder);

            // Configuración específica para SQL Server únicamente

            // configuracion de ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NumeroIdentificacion).HasMaxLength(20);
                entity.Property(e => e.Institucion).HasMaxLength(100);
                entity.Property(e => e.Departamento).HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasMaxLength(500);

                // Configuración para SQL Server únicamente
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Configurar propiedades específicas de ApplicationUser
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.LastLoginDate).IsRequired(false);
                entity.Property(e => e.UltimoAcceso).IsRequired(false);

                // Índices únicos para campos importantes
                entity.HasIndex(e => e.NumeroIdentificacion).IsUnique()
                    .HasFilter("[NumeroIdentificacion] IS NOT NULL");

                // Índices para mejorar rendimiento
                entity.HasIndex(e => e.CreatedDate);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.LastLoginDate);
            });

            // configuracion de ItemEvaluacion
            modelBuilder.Entity<ItemEvaluacion>(entity =>
            {
                entity.HasKey(e => e.IdItem);
                entity.Property(e => e.NombreItem).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Peso).HasPrecision(5, 2).HasDefaultValue(0);
                entity.HasOne(e => e.Rubrica)
                    // CORRECCIÓN: Se cambia la relación a uno a muchos
                    .WithMany(r => r.ItemsEvaluacion)
                    .HasForeignKey(e => e.IdRubrica)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // configuracion de NivelCalificacion
            modelBuilder.Entity<NivelCalificacion>(entity =>
            {
                entity.HasKey(e => e.IdNivel);
                entity.Property(e => e.NombreNivel).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.OrdenNivel);

                // Configuración de relación con GrupoCalificacion
                entity.HasOne(e => e.GrupoCalificacion)
                    .WithMany(g => g.NivelesCalificacion)
                    .HasForeignKey(e => e.IdGrupo)
                    .HasConstraintName("FK_NivelesCalificacion_GruposCalificacion_IdGrupo")
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // configuracion de GrupoCalificacion
            modelBuilder.Entity<GrupoCalificacion>(entity =>
            {
                entity.HasKey(e => e.IdGrupo);
                entity.Property(e => e.NombreGrupo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
            });

            // configuracion de Rubrica
            modelBuilder.Entity<Rubrica>(entity =>
            {
                entity.HasKey(e => e.IdRubrica);
                entity.Property(e => e.NombreRubrica).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("ACTIVO");

                // Configuración para SQL Server únicamente
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.EsPublica); // FIXED: Ensure NOT NULL with default
                entity.Property(e => e.CreadoPorId).HasMaxLength(450);
                entity.Property(e => e.ModificadoPorId).HasMaxLength(450);

                // CONFIGURACIÓN CORREGIDA: Relación con GrupoCalificacion
                entity.HasOne(e => e.GrupoCalificacion)
                    .WithMany(g => g.Rubricas)
                    .HasForeignKey(e => e.IdGrupo)
                    .HasConstraintName("FK_Rubricas_GruposCalificacion_IdGrupo")
                    .OnDelete(DeleteBehavior.SetNull);

                // Configuración de relaciones con ApplicationUser
                entity.HasOne(e => e.CreadoPor)
                    .WithMany(u => u.RubricasCreadas)
                    .HasForeignKey(e => e.CreadoPorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ModificadoPor)
                    .WithMany()
                    .HasForeignKey(e => e.ModificadoPorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // configuracion de ValorRubrica
            modelBuilder.Entity<ValorRubrica>(entity =>
            {
                entity.HasKey(e => e.IdValor);
                entity.Property(e => e.ValorPuntos)
                    .HasPrecision(5, 2)
                    .HasDefaultValue(0)
                    .IsRequired(); // CRITICAL: Add this to ensure NOT NULL

                // Configuración de todas las relaciones de foreign key
                entity.HasOne(e => e.Rubrica)
                    .WithMany(r => r.ValoresRubrica)
                    .HasForeignKey(e => e.IdRubrica)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ItemEvaluacion)
                    .WithMany(i => i.ValoresRubrica)
                    .HasForeignKey(e => e.IdItem)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.NivelCalificacion)
                    .WithMany(n => n.ValoresRubrica)
                    .HasForeignKey(e => e.IdNivel)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice único compuesto para evitar duplicados
                entity.HasIndex(e => new { e.IdRubrica, e.IdItem, e.IdNivel })
                    .IsUnique()
                    .HasDatabaseName("IX_ValorRubrica_Unique");
            });

            // configuracion de Estudiante
            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.HasKey(e => e.IdEstudiante);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NumeroId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DireccionCorreo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Institucion).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Grupos).HasMaxLength(50);
                entity.Property(e => e.Anio).IsRequired();
                
                // 🆕 Configuración campos ACS
                entity.Property(e => e.TipoAdecuacion).IsRequired().HasMaxLength(20).HasDefaultValue("NoPresenta");
                entity.Property(e => e.DetallesACS).HasMaxLength(500);
                entity.Property(e => e.AplicarACSPeriodosAnteriores).HasDefaultValue(false);

                entity.HasIndex(e => e.NumeroId).IsUnique();
                entity.HasIndex(e => e.DireccionCorreo);
                
                // 🆕 Configuración de relaciones de navegación con PeriodoAcademico
                entity.HasOne(e => e.PeriodoAcademico)
                    .WithMany(p => p.Estudiantes)
                    .HasForeignKey(e => e.PeriodoAcademicoId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.PeriodoInicioACS)
                    .WithMany()
                    .HasForeignKey(e => e.PeriodoInicioACSId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            // 🆕 configuracion de EstudianteInstrumentoACS
            modelBuilder.Entity<EstudianteInstrumentoACS>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Configuración de propiedades
                entity.Property(e => e.PonderacionPersonalizadaPorcentaje)
                    .HasPrecision(5, 2)
                    .IsRequired(false);
                
                entity.Property(e => e.MotivoExencion).HasMaxLength(500);
                entity.Property(e => e.CriteriosAdaptados).HasMaxLength(1000);
                entity.Property(e => e.Observaciones).HasMaxLength(3000);
                entity.Property(e => e.UsuarioCreacion).HasMaxLength(256);
                entity.Property(e => e.UsuarioModificacion).HasMaxLength(256);
                
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Exento).HasDefaultValue(false);
                
                // Configuración de relaciones
                entity.HasOne(e => e.Estudiante)
                    .WithMany(est => est.ConfiguracionesACS)
                    .HasForeignKey(e => e.EstudianteId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Instrumento)
                    .WithMany()
                    .HasForeignKey(e => e.InstrumentoEvaluacionId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Periodo)
                    .WithMany()
                    .HasForeignKey(e => e.PeriodoAcademicoId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.RubricaModificada)
                    .WithMany()
                    .HasForeignKey(e => e.RubricaModificadaId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
                
                // Índices para mejorar rendimiento
                entity.HasIndex(e => new { e.EstudianteId, e.InstrumentoEvaluacionId, e.PeriodoAcademicoId })
                    .IsUnique()
                    .HasDatabaseName("IX_EstudianteInstrumentoACS_Unique");
                
                entity.HasIndex(e => e.EstudianteId);
                entity.HasIndex(e => e.PeriodoAcademicoId);
            });

            // configuracion de Evaluacion
            modelBuilder.Entity<Evaluacion>(entity =>
            {
                entity.HasKey(e => e.IdEvaluacion);

                // Configuración para SQL Server únicamente
                entity.Property(e => e.FechaEvaluacion).HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.TotalPuntos).HasPrecision(5, 2);
                entity.Property(e => e.Observaciones).HasMaxLength(3000);
                entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("BORRADOR");
                entity.Property(e => e.EvaluadoPorId).HasMaxLength(450);

                entity.HasOne(e => e.Estudiante)
                    .WithMany(est => est.Evaluaciones)
                    .HasForeignKey(e => e.IdEstudiante)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Rubrica)
                    .WithMany(r => r.Evaluaciones)
                    .HasForeignKey(e => e.IdRubrica)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EvaluadoPor)
                    .WithMany(u => u.EvaluacionesRealizadas)
                    .HasForeignKey(e => e.EvaluadoPorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // configuracion de DetalleEvaluacion
            modelBuilder.Entity<DetalleEvaluacion>(entity =>
            {
                entity.HasKey(e => e.IdDetalle);
                entity.Property(e => e.PuntosObtenidos).HasPrecision(5, 2);

                entity.HasOne(e => e.Evaluacion)
                    .WithMany(ev => ev.DetallesEvaluacion)
                    .HasForeignKey(e => e.IdEvaluacion)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ItemEvaluacion)
                    .WithMany(i => i.DetallesEvaluacion)
                    .HasForeignKey(e => e.IdItem)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.NivelCalificacion)
                    .WithMany(n => n.DetallesEvaluacion)
                    .HasForeignKey(e => e.IdNivel)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // configuracion de PeriodoAcademico
            modelBuilder.Entity<PeriodoAcademico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100);
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin).IsRequired();
                entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Activo");
                entity.Property(e => e.Activo).HasDefaultValue(false);

                entity.HasIndex(e => e.FechaInicio);
                entity.HasIndex(e => e.FechaFin);
                entity.HasIndex(e => e.Estado);
            });

            // configuracion de RubricaNivel
            modelBuilder.Entity<RubricaNivel>(entity =>
            {
                entity.HasKey(e => new { e.IdRubrica, e.IdNivel });
                entity.Property(e => e.OrdenEnRubrica).HasDefaultValue(0);

                entity.HasOne(e => e.Rubrica)
                    .WithMany(r => r.RubricaNiveles)
                    .HasForeignKey(e => e.IdRubrica)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.NivelCalificacion)
                    .WithMany(n => n.RubricaNiveles)
                    .HasForeignKey(e => e.IdNivel)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // configuracion de ConfiguracionSistema
            modelBuilder.Entity<ConfiguracionSistema>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Clave).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Valor).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descripcion).HasMaxLength(500);

                // Configuración para SQL Server únicamente
                entity.Property(e => e.FechaModificacion).HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);

                entity.HasIndex(e => e.Clave).IsUnique();
            });

            // configuracion de AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityId).HasMaxLength(50);
                entity.Property(e => e.EntityName).HasMaxLength(200);

                // Configurar tipo de columna para SQL Server únicamente
                entity.Property(e => e.OldValues).HasColumnType("NVARCHAR(MAX)");
                entity.Property(e => e.NewValues).HasColumnType("NVARCHAR(MAX)");
                entity.Property(e => e.Metadata).HasColumnType("NVARCHAR(MAX)");
                entity.Property(e => e.Timestamp).HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.LogLevel).HasMaxLength(50).HasDefaultValue("Information");
                entity.Property(e => e.AdditionalInfo).HasMaxLength(500);
                entity.Property(e => e.Success).HasDefaultValue(true);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.Property(e => e.DurationMs);
                entity.Property(e => e.SessionId).HasMaxLength(36);
                entity.Property(e => e.ClientInfo).HasMaxLength(100);
                entity.Property(e => e.Referrer).HasMaxLength(500);
                entity.Property(e => e.HttpMethod).HasMaxLength(10);
                entity.Property(e => e.RequestUrl).HasMaxLength(500);
                entity.Property(e => e.ResponseStatusCode);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.EntityType);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.Action);
                entity.HasIndex(e => new { e.EntityType, e.EntityId });
            });

            // configuracion de Materia
            modelBuilder.Entity<Materia>(entity =>
            {
                entity.HasKey(e => e.MateriaId);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);

                // Configuración para SQL Server únicamente
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Activa).HasDefaultValue(true);
                entity.Property(e => e.Tipo).HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Estado).HasMaxLength(20);

                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            // Configuración de MateriaRequisito
            modelBuilder.Entity<MateriaRequisito>(entity =>
            {
                entity.HasKey(mr => new { mr.MateriaId, mr.RequisitoId });

                entity.HasOne(mr => mr.Materia)
                    .WithMany(m => m.EsRequisitoPara)
                    .HasForeignKey(mr => mr.MateriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(mr => mr.Requisito)
                    .WithMany(m => m.Prerequisitos)
                    .HasForeignKey(mr => mr.RequisitoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });



            // Configuración de MateriaPeriodo
            modelBuilder.Entity<MateriaPeriodo>(entity =>
            {
                entity.HasKey(mp => mp.Id);
                entity.Property(mp => mp.Cupo).HasDefaultValue(0);
                entity.Property(mp => mp.Estado).HasMaxLength(20).HasDefaultValue("Abierta");

                // Configuración para SQL Server únicamente
                entity.Property(mp => mp.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.Property(mp => mp.Observaciones).HasMaxLength(500);

                entity.HasOne(mp => mp.Materia)
                    .WithMany(m => m.Ofertas)
                    .HasForeignKey(mp => mp.MateriaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mp => mp.PeriodoAcademico)
                    .WithMany()
                    .HasForeignKey(mp => mp.PeriodoAcademicoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de InstrumentoEvaluacion
            modelBuilder.Entity<InstrumentoEvaluacion>(entity =>
            {
                entity.HasKey(e => e.InstrumentoId);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descripcion).HasMaxLength(500);

                // Configuración para SQL Server únicamente
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Activo).HasDefaultValue(true);
            });

            // Configuración de InstrumentoRubrica
            modelBuilder.Entity<InstrumentoRubrica>(entity =>
            {
                entity.HasKey(ir => new { ir.InstrumentoEvaluacionId, ir.RubricaId });

                // Configuración para SQL Server únicamente
                entity.Property(ir => ir.FechaAsignacion).HasDefaultValueSql("GETDATE()");

                entity.Property(ir => ir.EsObligatorio).HasDefaultValue(false);
                entity.Property(ir => ir.Ponderacion).HasPrecision(5, 2).HasDefaultValue(0);

                entity.HasOne(ir => ir.InstrumentoEvaluacion)
                    .WithMany(i => i.InstrumentoRubricas)
                    .HasForeignKey(ir => ir.InstrumentoEvaluacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ir => ir.Rubrica)
                    .WithMany()
                    .HasForeignKey(ir => ir.RubricaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de InstrumentoMateria
            modelBuilder.Entity<InstrumentoMateria>(entity =>
            {
                entity.HasKey(im => new { im.InstrumentoEvaluacionId, im.MateriaId });

                // Configuración para SQL Server únicamente
                entity.Property(im => im.FechaAsignacion).HasDefaultValueSql("GETDATE()");

                entity.Property(im => im.EsObligatorio).HasDefaultValue(false);

                entity.HasOne(im => im.InstrumentoEvaluacion)
                    .WithMany(i => i.InstrumentoMaterias)
                    .HasForeignKey(im => im.InstrumentoEvaluacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(im => im.Materia)
                    .WithMany(m => m.InstrumentoMaterias)
                    .HasForeignKey(im => im.MateriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ConfiguracionComponenteSEA
            modelBuilder.Entity<ConfiguracionComponenteSEA>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.ComponenteSEA).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Porcentaje).IsRequired().HasPrecision(5, 2);
                entity.Property(c => c.Activo).HasDefaultValue(true);
                entity.Property(c => c.FechaConfiguracion).HasDefaultValueSql("GETDATE()");

                entity.HasOne(c => c.Materia)
                    .WithMany()
                    .HasForeignKey(c => c.MateriaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.InstrumentoEvaluacion)
                    .WithMany()
                    .HasForeignKey(c => c.InstrumentoEvaluacionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => new { c.MateriaId, c.InstrumentoEvaluacionId }).IsUnique();
                entity.HasIndex(c => new { c.MateriaId, c.ComponenteSEA });
            });

            // Configuración de CuadernoCalificador
            modelBuilder.Entity<CuadernoCalificador>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("ACTIVO");

                // Configuración para SQL Server únicamente
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Materia)
                    .WithMany(m => m.CuadernosCalificadores)
                    .HasForeignKey(e => e.MateriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PeriodoAcademico)
                    .WithMany()
                    .HasForeignKey(e => e.PeriodoAcademicoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de CuadernoInstrumento
            modelBuilder.Entity<CuadernoInstrumento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PonderacionPorcentaje).IsRequired().HasPrecision(5, 2);
                entity.Property(e => e.EsObligatorio).HasDefaultValue(true);

                entity.HasOne(e => e.CuadernoCalificador)
                    .WithMany(c => c.CuadernoInstrumentos)
                    .HasForeignKey(e => e.CuadernoCalificadorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Rubrica)
                    .WithMany()
                    .HasForeignKey(e => e.RubricaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============ CONFIGURACIÓN DE NUEVAS TABLAS PARA SISTEMA DE GRUPOS ============

            // 🔧 NUEVA: Configuración de TipoGrupoCatalogo
            modelBuilder.Entity<TipoGrupoCatalogo>(entity =>
            {
                entity.HasKey(e => e.IdTipoGrupo);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Activo");

                // Índices
                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_TiposGrupo_Estado");
                entity.HasIndex(e => e.FechaRegistro)
                    .HasDatabaseName("IX_TiposGrupo_FechaRegistro");
            });

            // Configuración de GrupoEstudiante
            modelBuilder.Entity<GrupoEstudiante>(entity =>
            {
                entity.HasKey(e => e.GrupoId);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);

                // 🔧 ACTUALIZACIÓN: Usar IdTipoGrupo en lugar de TipoGrupoId
                entity.Property(e => e.IdTipoGrupo).IsRequired();

                // 🔧 MANTENER: Configuración del enum para compatibilidad
                entity.Property(e => e.TipoGrupo).IsRequired()
                    .HasConversion(
                        v => v.ToString(),
                        v => (TipoGrupo)Enum.Parse(typeof(TipoGrupo), v));
                
                entity.Property(e => e.Nivel).HasMaxLength(50);
                entity.Property(e => e.Estado).IsRequired()
                    .HasConversion(
                        v => v.ToString(),
                        v => (EstadoGrupo)Enum.Parse(typeof(EstadoGrupo), v));
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CreadoPorId).HasMaxLength(450);
                entity.Property(e => e.Observaciones).HasMaxLength(1000);

                // 🔧 NUEVA: Relación con catálogo de tipos
                entity.HasOne(e => e.TipoGrupoCatalogo)
                    .WithMany(t => t.GruposEstudiantes)
                    .HasForeignKey(e => e.IdTipoGrupo)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relaciones existentes
                entity.HasOne(e => e.PeriodoAcademico)
                    .WithMany()
                    .HasForeignKey(e => e.PeriodoAcademicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreadoPor)
                    .WithMany()
                    .HasForeignKey(e => e.CreadoPorId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Índices para optimización
                entity.HasIndex(e => new { e.Codigo, e.PeriodoAcademicoId })
                    .IsUnique()
                    .HasDatabaseName("IX_GruposEstudiantes_Codigo_Periodo");
                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_GruposEstudiantes_Estado");
                entity.HasIndex(e => e.IdTipoGrupo)
                    .HasDatabaseName("IX_GruposEstudiantes_IdTipoGrupo");
            });

            // Configuración de EstudianteGrupo
            modelBuilder.Entity<EstudianteGrupo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).IsRequired()
                    .HasConversion(
                        v => v.ToString(),
                        v => (EstadoAsignacion)Enum.Parse(typeof(EstadoAsignacion), v ?? "Activo"))
                    .HasDefaultValue(EstadoAsignacion.Activo);
                entity.Property(e => e.AsignadoPorId).HasMaxLength(450);
                entity.Property(e => e.MotivoAsignacion).HasMaxLength(200);
                entity.Property(e => e.EsGrupoPrincipal).HasDefaultValue(true);

                // Relaciones
                entity.HasOne(e => e.Estudiante)
                    .WithMany()
                    .HasForeignKey(e => e.EstudianteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Grupo)
                    .WithMany(g => g.EstudianteGrupos)
                    .HasForeignKey(e => e.GrupoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AsignadoPor)
                    .WithMany()
                    .HasForeignKey(e => e.AsignadoPorId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Índices
                entity.HasIndex(e => e.GrupoId)
                    .HasDatabaseName("IX_EstudianteGrupos_GrupoId");
                entity.HasIndex(e => e.FechaAsignacion)
                    .HasDatabaseName("IX_EstudianteGrupos_FechaAsignacion");
                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_EstudianteGrupos_Estado");

                // Índice único para evitar duplicados activos
                entity.HasIndex(e => new { e.EstudianteId, e.GrupoId, e.Estado })
                    .HasDatabaseName("IX_EstudianteGrupos_Unique_Activo")
                    .HasFilter("[Estado] = 'Activo'");
            });

            // Configuración de GrupoMateria
            modelBuilder.Entity<GrupoMateria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).IsRequired()
                    .HasConversion(
                        v => v.ToString(),
                        v => (EstadoAsignacion)Enum.Parse(typeof(EstadoAsignacion), v ?? "Activo"))
                    .HasDefaultValue(EstadoAsignacion.Activo);
                entity.Property(e => e.Observaciones).HasMaxLength(500);

                // Relaciones
                entity.HasOne(e => e.Grupo)
                    .WithMany(g => g.GrupoMaterias)
                    .HasForeignKey(e => e.GrupoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Materia)
                    .WithMany()
                    .HasForeignKey(e => e.MateriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice único para evitar duplicados
                entity.HasIndex(e => new { e.GrupoId, e.MateriaId })
                    .IsUnique()
                    .HasDatabaseName("IX_GrupoMaterias_Unique");
            });

            // 🔧 NUEVA: Configuración de AuditoriaOperacion
            modelBuilder.Entity<AuditoriaOperacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TipoOperacion).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TablaAfectada).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Motivo).HasMaxLength(1000);
                entity.Property(e => e.DireccionIP).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.MensajeError).HasMaxLength(1000);
                entity.Property(e => e.FechaOperacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UsuarioId).IsRequired();

                // Relación con Usuario
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Índices para optimizar consultas
                entity.HasIndex(e => e.TablaAfectada)
                    .HasDatabaseName("IX_AuditoriaOperacion_TablaAfectada");
                entity.HasIndex(e => e.RegistroId)
                    .HasDatabaseName("IX_AuditoriaOperacion_RegistroId");
                entity.HasIndex(e => e.FechaOperacion)
                    .HasDatabaseName("IX_AuditoriaOperacion_FechaOperacion");
                entity.HasIndex(e => e.UsuarioId)
                    .HasDatabaseName("IX_AuditoriaOperacion_UsuarioId");
                entity.HasIndex(e => new { e.TablaAfectada, e.RegistroId })
                    .HasDatabaseName("IX_AuditoriaOperacion_Tabla_Registro");
            });

            // 🔧 NUEVA: Configuración de Asistencia
            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.HasKey(e => e.AsistenciaId);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(2);
                entity.Property(e => e.Justificacion).HasMaxLength(500);
                entity.Property(e => e.Observaciones).HasMaxLength(1000);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.RegistradoPorId).HasMaxLength(450);
                entity.Property(e => e.ModificadoPorId).HasMaxLength(450);
                entity.Property(e => e.EsModificacion).HasDefaultValue(false);

                // Relaciones
                entity.HasOne(e => e.Estudiante)
                    .WithMany()
                    .HasForeignKey(e => e.EstudianteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Grupo)
                    .WithMany()
                    .HasForeignKey(e => e.GrupoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Materia)
                    .WithMany()
                    .HasForeignKey(e => e.MateriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Leccion)
                    .WithMany(l => l.Asistencias)
                    .HasForeignKey(e => e.IdLeccion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RegistradoPor)
                    .WithMany()
                    .HasForeignKey(e => e.RegistradoPorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.ModificadoPor)
                    .WithMany()
                    .HasForeignKey(e => e.ModificadoPorId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Índices para optimización
                // Restricción única según especificación MEP: (EstudianteId, GrupoId, IdLeccion, Fecha)
                entity.HasIndex(e => new { e.EstudianteId, e.GrupoId, e.IdLeccion, e.Fecha })
                    .IsUnique()
                    .HasDatabaseName("IX_Asistencias_Unique_Estudiante_Grupo_Leccion_Fecha")
                    .HasFilter("[IdLeccion] IS NOT NULL"); // Solo para registros con lección especificada

                // Mantener índice para compatibilidad con registros antiguos (sin IdLeccion)
                entity.HasIndex(e => new { e.EstudianteId, e.GrupoId, e.MateriaId, e.Fecha })
                    .HasDatabaseName("IX_Asistencias_Estudiante_Grupo_Materia_Fecha")
                    .HasFilter("[IdLeccion] IS NULL");

                entity.HasIndex(e => e.Fecha)
                    .HasDatabaseName("IX_Asistencias_Fecha");
                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_Asistencias_Estado");
                entity.HasIndex(e => e.GrupoId)
                    .HasDatabaseName("IX_Asistencias_GrupoId");
                entity.HasIndex(e => e.MateriaId)
                    .HasDatabaseName("IX_Asistencias_MateriaId");
                entity.HasIndex(e => e.FechaRegistro)
                    .HasDatabaseName("IX_Asistencias_FechaRegistro");
                entity.HasIndex(e => e.IdLeccion)
                    .HasDatabaseName("IX_Asistencias_IdLeccion");
            });

            // 🔧 NUEVA: Configuración de Leccion (especificación MEP)
            modelBuilder.Entity<Leccion>(entity =>
            {
                entity.HasKey(e => e.IdLeccion);
                entity.Property(e => e.Activa).HasDefaultValue(true);
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                // Relaciones
                entity.HasOne(e => e.Grupo)
                    .WithMany()
                    .HasForeignKey(e => e.IdGrupo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Materia)
                    .WithMany()
                    .HasForeignKey(e => e.MateriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices para optimización y restricción única
                // Una lección específica por grupo/materia/día/bloque
                entity.HasIndex(e => new { e.IdGrupo, e.MateriaId, e.DiaSemana, e.NumeroBloque })
                    .IsUnique()
                    .HasDatabaseName("IX_Lecciones_Unique_Grupo_Materia_Dia_Bloque");

                entity.HasIndex(e => e.IdGrupo)
                    .HasDatabaseName("IX_Lecciones_IdGrupo");
                entity.HasIndex(e => e.MateriaId)
                    .HasDatabaseName("IX_Lecciones_MateriaId");
                entity.HasIndex(e => e.DiaSemana)
                    .HasDatabaseName("IX_Lecciones_DiaSemana");
                entity.HasIndex(e => e.NumeroBloque)
                    .HasDatabaseName("IX_Lecciones_NumeroBloque");
                entity.HasIndex(e => e.Activa)
                    .HasDatabaseName("IX_Lecciones_Activa");

                // Índice compuesto para consultas de horario por día
                entity.HasIndex(e => new { e.IdGrupo, e.DiaSemana, e.HoraInicio })
                    .HasDatabaseName("IX_Lecciones_Grupo_Dia_Hora");
            });

            // 🔧 NUEVA: Configuración de EstudianteEmpadronamiento
            modelBuilder.Entity<EstudianteEmpadronamiento>(entity =>
            {
                entity.HasKey(e => e.IdEstudiante);
                
                // Configuración de campos principales
                entity.Property(e => e.NumeroId).HasMaxLength(20);
                
                // Datos personales complementarios
                entity.Property(e => e.Genero).HasMaxLength(20);
                entity.Property(e => e.Nacionalidad).HasMaxLength(50);
                entity.Property(e => e.EstadoCivil).HasMaxLength(30);
                
                // Contacto y residencia
                entity.Property(e => e.Provincia).HasMaxLength(50);
                entity.Property(e => e.Canton).HasMaxLength(50);
                entity.Property(e => e.Distrito).HasMaxLength(50);
                entity.Property(e => e.Barrio).HasMaxLength(100);
                entity.Property(e => e.Senas).HasMaxLength(500);
                entity.Property(e => e.TelefonoAlterno).HasMaxLength(20);
                entity.Property(e => e.CorreoAlterno).HasMaxLength(100);
                
                // Responsables
                entity.Property(e => e.NombrePadre).HasMaxLength(100);
                entity.Property(e => e.NombreMadre).HasMaxLength(100);
                entity.Property(e => e.NombreTutor).HasMaxLength(100);
                entity.Property(e => e.ContactoEmergencia).HasMaxLength(100);
                entity.Property(e => e.TelefonoEmergencia).HasMaxLength(20);
                entity.Property(e => e.RelacionEmergencia).HasMaxLength(50);
                
                // Salud
                entity.Property(e => e.Alergias).HasMaxLength(500);
                entity.Property(e => e.CondicionesMedicas).HasMaxLength(500);
                entity.Property(e => e.Medicamentos).HasMaxLength(500);
                entity.Property(e => e.SeguroMedico).HasMaxLength(100);
                entity.Property(e => e.CentroMedicoHabitual).HasMaxLength(100);
                
                // Historial académico
                entity.Property(e => e.InstitucionProcedencia).HasMaxLength(100);
                entity.Property(e => e.UltimoNivelCursado).HasMaxLength(50);
                entity.Property(e => e.PromedioAnterior).HasPrecision(5, 2);
                entity.Property(e => e.AdaptacionesPrevias).HasMaxLength(500);
                
                // Documentación
                entity.Property(e => e.DocumentosRecibidosJson).HasColumnType("NVARCHAR(MAX)");
                entity.Property(e => e.DocumentosPendientesJson).HasColumnType("NVARCHAR(MAX)");
                
                // Estado del proceso
                entity.Property(e => e.EtapaActual).HasMaxLength(50);
                entity.Property(e => e.UsuarioEtapa).HasMaxLength(100);
                entity.Property(e => e.NotasInternas).HasColumnType("NVARCHAR(MAX)");
                
                // Auditoría
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
                entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);
                
                // Relación 1:1 con Estudiante
                entity.HasOne(e => e.Estudiante)
                    .WithOne()
                    .HasForeignKey<EstudianteEmpadronamiento>(e => e.IdEstudiante)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Índices para optimización
                entity.HasIndex(e => e.NumeroId)
                    .HasDatabaseName("IX_EstudianteEmpadronamiento_NumeroId");
                entity.HasIndex(e => e.EtapaActual)
                    .HasDatabaseName("IX_EstudianteEmpadronamiento_EtapaActual");
                entity.HasIndex(e => e.FechaCreacion)
                    .HasDatabaseName("IX_EstudianteEmpadronamiento_FechaCreacion");
            });

            // NUEVA: Configuración de SliderItem
            modelBuilder.Entity<SliderItem>(entity =>
            {
                entity.ToTable("SliderItems");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Subtitulo).HasMaxLength(500);
                entity.Property(e => e.ImagenUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Orden).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UsuarioCreacionId).HasMaxLength(450);
                entity.Property(e => e.UsuarioModificacionId).HasMaxLength(450);

                // Relaciones con ApplicationUser
                entity.HasOne(e => e.UsuarioCreacion)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioCreacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UsuarioModificacion)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioModificacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.Orden).HasDatabaseName("IX_SliderItem_Orden");
                entity.HasIndex(e => e.Activo).HasDatabaseName("IX_SliderItem_Activo");
            });

            // 🔧 NUEVA: Configuración de BoletaConducta
            modelBuilder.Entity<BoletaConducta>(entity =>
            {
                entity.HasKey(e => e.IdBoleta);
                
                // Relación con Estudiante
                entity.HasOne(e => e.Estudiante)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstudiante)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con PeriodoAcademico
                entity.HasOne(e => e.Periodo)
                    .WithMany()
                    .HasForeignKey(e => e.IdPeriodo)
                    .OnDelete(DeleteBehavior.NoAction);
                
                // Relación con TipoFalta
                entity.HasOne(e => e.TipoFalta)
                    .WithMany(t => t.Boletas)
                    .HasForeignKey(e => e.IdTipoFalta)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con ProfesorGuia (opcional)
                entity.HasOne(e => e.ProfesorGuia)
                    .WithMany()
                    .HasForeignKey(e => e.ProfesorGuiaId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con DocenteEmisor
                entity.HasOne(e => e.DocenteEmisor)
                    .WithMany()
                    .HasForeignKey(e => e.DocenteEmisorId)
                    .OnDelete(DeleteBehavior.NoAction);
                
                // Relación con AnuladaPor (opcional)
                entity.HasOne(e => e.AnuladaPor)
                    .WithMany()
                    .HasForeignKey(e => e.AnuladaPorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de DecisionProfesionalConducta
            modelBuilder.Entity<DecisionProfesionalConducta>(entity =>
            {
                entity.HasKey(e => e.IdDecision);
                
                // Relación con Estudiante - NoAction para evitar múltiples cascadas
                entity.HasOne(e => e.Estudiante)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstudiante)
                    .OnDelete(DeleteBehavior.NoAction);
                
                // Relación con PeriodoAcademico - NoAction para evitar múltiples cascadas
                entity.HasOne(e => e.Periodo)
                    .WithMany()
                    .HasForeignKey(e => e.IdPeriodo)
                    .OnDelete(DeleteBehavior.NoAction);
                
                // Relación con NotaConducta
                entity.HasOne(e => e.NotaConducta)
                    .WithOne(n => n.DecisionProfesional)
                    .HasForeignKey<DecisionProfesionalConducta>(e => e.IdNotaConducta)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con TomaDecisionPor (ApplicationUser)
                entity.HasOne(e => e.TomaDecisionPor)
                    .WithMany()
                    .HasForeignKey(e => e.TomaDecisionPorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de NotaConducta
            modelBuilder.Entity<NotaConducta>(entity =>
            {
                entity.HasKey(e => e.IdNotaConducta);

                // Relación con Estudiante - NoAction para evitar múltiples cascadas
                entity.HasOne(e => e.Estudiante)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstudiante)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con PeriodoAcademico - NoAction para evitar múltiples cascadas
                entity.HasOne(e => e.Periodo)
                    .WithMany()
                    .HasForeignKey(e => e.IdPeriodo)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de ProgramaAccionesInstitucional
            modelBuilder.Entity<ProgramaAccionesInstitucional>(entity =>
            {
                entity.HasKey(e => e.IdPrograma);

                // Relación con Estudiante - NoAction para evitar múltiples cascadas
                entity.HasOne(e => e.Estudiante)
                    .WithMany()
                    .HasForeignKey(e => e.IdEstudiante)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con PeriodoAcademico - NoAction para evitar múltiples cascadas
                entity.HasOne(e => e.Periodo)
                    .WithMany()
                    .HasForeignKey(e => e.IdPeriodo)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con NotaConducta - NoAction para evitar múltiples cascadas
                entity.HasOne(e => e.NotaConducta)
                    .WithOne(n => n.ProgramaAcciones)
                    .HasForeignKey<ProgramaAccionesInstitucional>(e => e.IdNotaConducta)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con ResponsableSupervision
                entity.HasOne(e => e.ResponsableSupervision)
                    .WithMany()
                    .HasForeignKey(e => e.ResponsableSupervisionId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Relación con VerificadoPor
                entity.HasOne(e => e.VerificadoPor)
                    .WithMany()
                    .HasForeignKey(e => e.VerificadoPorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}