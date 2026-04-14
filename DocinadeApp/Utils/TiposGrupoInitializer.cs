using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Utils
{
    /// <summary>
    /// Inicializador para el cat�logo simple de tipos de grupo
    /// </summary>
    public static class TiposGrupoInitializer
    {
        /// <summary>
        /// Inicializa el cat�logo de tipos de grupo con datos por defecto
        /// </summary>
        public static async Task<bool> InitializeTiposGrupoAsync(RubricasDbContext context)
        {
            try
            {
                // Verificar si ya existen tipos de grupo
                var existenTipos = await context.TiposGrupo.AnyAsync();
                if (existenTipos)
                {
                    Console.WriteLine("? Cat�logo de tipos de grupo ya existe");
                    return true;
                }

                Console.WriteLine("?? Creando cat�logo de tipos de grupo...");

                // Crear tipos de grupo por defecto
                var tiposGrupo = new List<TipoGrupoCatalogo>
                {
                    new TipoGrupoCatalogo
                    {
                        Nombre = "Secci�n",
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    },
                    new TipoGrupoCatalogo
                    {
                        Nombre = "Nivel",
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    },
                    new TipoGrupoCatalogo
                    {
                        Nombre = "Modalidad",
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    },
                    new TipoGrupoCatalogo
                    {
                        Nombre = "Personalizado",
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    },
                    new TipoGrupoCatalogo
                    {
                        Nombre = "Especial",
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    },
                    new TipoGrupoCatalogo
                    {
                        Nombre = "Refuerzo",
                        FechaRegistro = DateTime.Now,
                        Estado = "Activo"
                    }
                };

                // Agregar tipos al contexto
                await context.TiposGrupo.AddRangeAsync(tiposGrupo);
                
                // Guardar cambios
                var tiposCreados = await context.SaveChangesAsync();
                
                Console.WriteLine($"? Se crearon {tiposCreados} tipos de grupo en el cat�logo");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error al inicializar cat�logo de tipos de grupo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Migra grupos existentes para usar el cat�logo de tipos
        /// </summary>
        public static async Task<bool> MigrarGruposExistentesAsync(RubricasDbContext context)
        {
            try
            {
                Console.WriteLine("?? Migrando grupos existentes al cat�logo de tipos...");

                // Obtener grupos que no tengan IdTipoGrupo asignado
                var gruposSinTipo = await context.GruposEstudiantes
                    .Where(g => g.IdTipoGrupo == 0 || g.IdTipoGrupo == null)
                    .ToListAsync();

                if (!gruposSinTipo.Any())
                {
                    Console.WriteLine("? No hay grupos para migrar");
                    return true;
                }

                // Obtener mapeo de tipos del cat�logo
                var tiposMap = await context.TiposGrupo
                    .ToDictionaryAsync(t => t.Nombre, t => t.IdTipoGrupo);

                int gruposMigrados = 0;

                foreach (var grupo in gruposSinTipo)
                {
                    // Mapear enum a nombre del cat�logo
                    string nombreTipo = grupo.TipoGrupo switch
                    {
                        TipoGrupo.Seccion => "Secci�n",
                        TipoGrupo.Nivel => "Nivel",
                        TipoGrupo.Modalidad => "Modalidad",
                        TipoGrupo.Custom => "Personalizado",
                        _ => "Personalizado"
                    };

                    if (tiposMap.TryGetValue(nombreTipo, out int tipoId))
                    {
                        grupo.IdTipoGrupo = tipoId;
                        gruposMigrados++;
                    }
                    else
                    {
                        // Si no encuentra el tipo, asignar "Personalizado" por defecto
                        grupo.IdTipoGrupo = tiposMap.GetValueOrDefault("Personalizado", 4);
                        gruposMigrados++;
                    }
                }

                if (gruposMigrados > 0)
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine($"? Se migraron {gruposMigrados} grupos al cat�logo de tipos");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error al migrar grupos existentes: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Inicializaci�n completa del sistema de tipos de grupo
        /// </summary>
        public static async Task<bool> InitializeCompleteAsync(RubricasDbContext context)
        {
            try
            {
                Console.WriteLine("?? Inicializando sistema completo de tipos de grupo...");

                // 1. Crear cat�logo de tipos
                var catalogoCreado = await InitializeTiposGrupoAsync(context);
                if (!catalogoCreado)
                {
                    Console.WriteLine("? Fall� la creaci�n del cat�logo de tipos");
                    return false;
                }

                // 2. Migrar grupos existentes
                var gruposMigrados = await MigrarGruposExistentesAsync(context);
                if (!gruposMigrados)
                {
                    Console.WriteLine("?? Fall� la migraci�n de grupos existentes, pero el cat�logo est� disponible");
                }

                Console.WriteLine("?? Sistema de tipos de grupo inicializado completamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"?? Error en inicializaci�n completa: {ex.Message}");
                return false;
            }
        }
    }
}