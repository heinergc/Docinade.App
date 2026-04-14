using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Scripts
{
    public class CrearTablaRubricaNivel
    {
        private readonly RubricasDbContext _context;

        public CrearTablaRubricaNivel(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task EjecutarAsync()
        {
            try
            {
                Console.WriteLine("Creando tabla GruposCalificacion...");

                // Crear la tabla GruposCalificacion
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS GruposCalificacion (
                        IdGrupo INTEGER PRIMARY KEY AUTOINCREMENT,
                        NombreGrupo TEXT NOT NULL,
                        Descripcion TEXT,
                        Estado TEXT DEFAULT 'ACTIVO',
                        FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP
                    )
                ");

                Console.WriteLine("Actualizando tabla NivelesCalificacion...");

                // Agregar columna IdGrupo a NivelesCalificacion si no existe
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE NivelesCalificacion ADD COLUMN IdGrupo INTEGER
                    ");
                }
                catch
                {
                    Console.WriteLine("Columna IdGrupo ya existe en NivelesCalificacion");
                }

                Console.WriteLine("Actualizando tabla Rubricas...");

                // Agregar columna IdGrupo a Rubricas si no existe
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE Rubricas ADD COLUMN IdGrupo INTEGER
                    ");
                }
                catch
                {
                    Console.WriteLine("Columna IdGrupo ya existe en Rubricas");
                }

                Console.WriteLine("Creando tabla RubricaNiveles...");

                // Crear la tabla RubricaNiveles
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS RubricaNiveles (
                        IdRubrica INTEGER NOT NULL,
                        IdNivel INTEGER NOT NULL,
                        OrdenEnRubrica INTEGER NOT NULL,
                        PRIMARY KEY (IdRubrica, IdNivel),
                        FOREIGN KEY (IdRubrica) REFERENCES Rubricas (IdRubrica) ON DELETE CASCADE,
                        FOREIGN KEY (IdNivel) REFERENCES NivelesCalificacion (IdNivel) ON DELETE CASCADE
                    )
                ");

                Console.WriteLine("Creando grupo por defecto y migrando datos...");
                
                // Crear grupo por defecto
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT OR IGNORE INTO GruposCalificacion (IdGrupo, NombreGrupo, Descripcion, Estado, FechaCreacion)
                    VALUES (1, 'Niveles Tradicionales', 'Grupo de niveles de calificación tradicionales', 'ACTIVO', datetime('now'))
                ");

                // Asignar niveles existentes al grupo por defecto
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE NivelesCalificacion 
                    SET IdGrupo = 1 
                    WHERE IdGrupo IS NULL
                ");

                // Asignar rúbricas existentes al grupo por defecto
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE Rubricas 
                    SET IdGrupo = 1 
                    WHERE IdGrupo IS NULL
                ");

                Console.WriteLine("Migración completada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la migración: {ex.Message}");
                throw;
            }
        }
    }
}
