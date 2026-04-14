using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;

namespace RubricasApp.Web.Utils
{
    public static class DatabaseGroupsInitializer
    {
        public static async Task<bool> CreateGroupsTablesAsync(RubricasDbContext context)
        {
            try
            {
                // Marcar migraci�n inicial como aplicada
                await context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
                    BEGIN
                        CREATE TABLE [__EFMigrationsHistory] (
                            [MigrationId] nvarchar(150) NOT NULL,
                            [ProductVersion] nvarchar(32) NOT NULL,
                            CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250828040336_InitialCreateSqlServer')
                    BEGIN
                        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                        VALUES ('20250828040336_InitialCreateSqlServer', '8.0.8');
                    END
                ");

                // Marcar migraci�n de grupos como aplicada
                await context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250830191805_SistemaGruposEstudiantes')
                    BEGIN
                        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                        VALUES ('20250830191805_SistemaGruposEstudiantes', '8.0.8');
                    END
                ");

                // Crear tabla GruposEstudiantes
                await context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GruposEstudiantes')
                    BEGIN
                        CREATE TABLE [dbo].[GruposEstudiantes](
                            [GrupoId] [int] IDENTITY(1,1) NOT NULL,
                            [Codigo] [nvarchar](20) NOT NULL,
                            [Nombre] [nvarchar](100) NOT NULL,
                            [Descripcion] [nvarchar](500) NULL,
                            [TipoGrupo] [nvarchar](50) NOT NULL,
                            [Nivel] [nvarchar](50) NULL,
                            [CapacidadMaxima] [int] NULL,
                            [PeriodoAcademicoId] [int] NOT NULL,
                            [Estado] [nvarchar](20) NOT NULL DEFAULT 'Activo',
                            [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
                            [FechaModificacion] [datetime2](7) NULL,
                            [CreadoPorId] [nvarchar](450) NULL,
                            [Observaciones] [nvarchar](1000) NULL,
                            
                            CONSTRAINT [PK_GruposEstudiantes] PRIMARY KEY ([GrupoId]),
                            CONSTRAINT [FK_GruposEstudiantes_PeriodosAcademicos] 
                                FOREIGN KEY ([PeriodoAcademicoId]) REFERENCES [PeriodosAcademicos] ([Id]),
                            CONSTRAINT [FK_GruposEstudiantes_AspNetUsers] 
                                FOREIGN KEY ([CreadoPorId]) REFERENCES [AspNetUsers] ([Id])
                        );
                        
                        CREATE UNIQUE INDEX [IX_GruposEstudiantes_Codigo_Periodo] 
                            ON [GruposEstudiantes] ([Codigo], [PeriodoAcademicoId]);
                        CREATE INDEX [IX_GruposEstudiantes_Estado] ON [GruposEstudiantes] ([Estado]);
                        CREATE INDEX [IX_GruposEstudiantes_TipoGrupo] ON [GruposEstudiantes] ([TipoGrupo]);
                    END
                ");

                // Crear tabla EstudianteGrupos
                await context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EstudianteGrupos')
                    BEGIN
                        CREATE TABLE [dbo].[EstudianteGrupos](
                            [Id] [int] IDENTITY(1,1) NOT NULL,
                            [EstudianteId] [int] NOT NULL,
                            [GrupoId] [int] NOT NULL,
                            [FechaAsignacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
                            [FechaDesasignacion] [datetime2](7) NULL,
                            [Estado] [nvarchar](20) NOT NULL DEFAULT 'Activo',
                            [AsignadoPorId] [nvarchar](450) NULL,
                            [MotivoAsignacion] [nvarchar](200) NULL,
                            [EsGrupoPrincipal] [bit] NOT NULL DEFAULT 1,
                            
                            CONSTRAINT [PK_EstudianteGrupos] PRIMARY KEY ([Id]),
                            CONSTRAINT [FK_EstudianteGrupos_Estudiantes] 
                                FOREIGN KEY ([EstudianteId]) REFERENCES [Estudiantes] ([IdEstudiante]),
                            CONSTRAINT [FK_EstudianteGrupos_GruposEstudiantes] 
                                FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]),
                            CONSTRAINT [FK_EstudianteGrupos_AspNetUsers] 
                                FOREIGN KEY ([AsignadoPorId]) REFERENCES [AspNetUsers] ([Id])
                        );
                        
                        CREATE INDEX [IX_EstudianteGrupos_GrupoId] ON [EstudianteGrupos] ([GrupoId]);
                        CREATE INDEX [IX_EstudianteGrupos_FechaAsignacion] ON [EstudianteGrupos] ([FechaAsignacion]);
                        CREATE INDEX [IX_EstudianteGrupos_Estado] ON [EstudianteGrupos] ([Estado]);
                        
                        CREATE UNIQUE INDEX [IX_EstudianteGrupos_Unique_Activo] 
                            ON [EstudianteGrupos] ([EstudianteId], [GrupoId])
                            WHERE [Estado] = 'Activo';
                    END
                ");

                // Crear tabla GrupoMaterias
                await context.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GrupoMaterias')
                    BEGIN
                        CREATE TABLE [dbo].[GrupoMaterias](
                            [Id] [int] IDENTITY(1,1) NOT NULL,
                            [GrupoId] [int] NOT NULL,
                            [MateriaId] [int] NOT NULL,
                            [FechaAsignacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
                            [Estado] [nvarchar](20) NOT NULL DEFAULT 'Activo',
                            [Observaciones] [nvarchar](500) NULL,
                            
                            CONSTRAINT [PK_GrupoMaterias] PRIMARY KEY ([Id]),
                            CONSTRAINT [FK_GrupoMaterias_GruposEstudiantes] 
                                FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]),
                            CONSTRAINT [FK_GrupoMaterias_Materias] 
                                FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId])
                        );
                        
                        CREATE UNIQUE INDEX [IX_GrupoMaterias_Unique] 
                            ON [GrupoMaterias] ([GrupoId], [MateriaId]);
                    END
                ");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando tablas de grupos: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> InsertSampleDataAsync(RubricasDbContext context)
        {
            try
            {
                // Verificar si ya hay datos
                var existingGroups = await context.Set<RubricasApp.Web.Models.GrupoEstudiante>().AnyAsync();
                if (existingGroups)
                {
                    Console.WriteLine("Ya existen grupos, no se insertan datos de ejemplo.");
                    return true;
                }

                // Obtener per�odo acad�mico activo
                var periodoActivo = await context.PeriodosAcademicos
                    .Where(p => p.Estado == "Activo")
                    .FirstOrDefaultAsync();

                if (periodoActivo == null)
                {
                    Console.WriteLine("No se encontr� un per�odo acad�mico activo.");
                    return false;
                }

                // Insertar grupos de ejemplo con AMBAS columnas: IdTipoGrupo (nuevo) y TipoGrupo (legacy)
                await context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO GruposEstudiantes (Codigo, Nombre, Descripcion, TipoGrupo, IdTipoGrupo, Nivel, CapacidadMaxima, PeriodoAcademicoId, Estado)
                    VALUES 
                    ('7-1', 'Séptimo Sección 1', 'Grupo de séptimo año, sección 1', 'Seccion', 1, 'SEPTIMO', 30, {0}, 'Activo'),
                    ('7-2', 'Séptimo Sección 2', 'Grupo de séptimo año, sección 2', 'Seccion', 1, 'SEPTIMO', 28, {0}, 'Activo'),
                    ('8-A', 'Octavo Sección A', 'Grupo de octavo año, sección A', 'Seccion', 1, 'OCTAVO', 32, {0}, 'Activo'),
                    ('G1', 'Grupo 1', 'Grupo general número 1', 'Custom', 4, NULL, 25, {0}, 'Activo'),
                    ('VIRT-A', 'Virtual Grupo A', 'Grupo modalidad virtual A', 'Modalidad', 3, 'VIRTUAL', 50, {0}, 'Activo')
                ", periodoActivo.Id);

                Console.WriteLine("Datos de ejemplo insertados exitosamente.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error insertando datos de ejemplo: {ex.Message}");
                return false;
            }
        }
    }
}