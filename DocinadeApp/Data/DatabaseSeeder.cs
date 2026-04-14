using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

public static class DatabaseSeeder
{
    public static async Task SeedDatabase(RubricasDbContext context)
    {
        // Verificar si ya hay rúbricas de ejemplo
        if (await context.Rubricas.AnyAsync())
        {
            // Ya hay rúbricas, no insertar datos de ejemplo
            return;
        }

        // Obtener período académico existente (ya creado por SqlServerDatabaseInitializer)
        var periodo = await context.PeriodosAcademicos.FirstOrDefaultAsync();
        if (periodo == null)
        {
            // Si no existe, crear uno
            periodo = new PeriodoAcademico
            {
                Codigo = "C1",
                Nombre = "Primer Cuatrimestre",
                Tipo = TipoPeriodo.Cuatrimestre,
                Anio = 2025,
                NumeroPeriodo = 1,
                FechaInicio = new DateTime(2025, 1, 1),
                FechaFin = new DateTime(2025, 4, 30),
                Activo = true
            };
            context.PeriodosAcademicos.Add(periodo);
            await context.SaveChangesAsync();
        }

        // Obtener grupo de calificación existente o crear uno nuevo
        var grupo = await context.GruposCalificacion
            .FirstOrDefaultAsync(g => g.NombreGrupo == "Proyecto IO 2025");
        
        if (grupo == null)
        {
            grupo = new GrupoCalificacion
            {
                NombreGrupo = "Proyecto IO 2025",
                Descripcion = "Grupo de calificación para Investigación de Operaciones 2025",
                Estado = "ACTIVO",
                FechaCreacion = DateTime.Now
            };
            context.GruposCalificacion.Add(grupo);
            await context.SaveChangesAsync();
        }

        // Rúbricas de ejemplo
        var rubrica1 = new Rubrica
        {
            NombreRubrica = "Proyecto de Investigación",
            Descripcion = "Evaluación del proyecto de investigación - Análisis completo con formulación y solución",
            Estado = "ACTIVO",
            FechaCreacion = DateTime.Now,
            EsPublica = 1,
            IdGrupo = grupo.IdGrupo
        };
        
        var rubrica2 = new Rubrica
        {
            NombreRubrica = "Presentación Oral",
            Descripcion = "Evaluación de la presentación oral del proyecto - Comunicación y dominio del tema",
            Estado = "ACTIVO",
            FechaCreacion = DateTime.Now,
            EsPublica = 1,
            IdGrupo = grupo.IdGrupo
        };
        
        var rubrica3 = new Rubrica
        {
            NombreRubrica = "Trabajo en Equipo",
            Descripcion = "Evaluación de las competencias de trabajo colaborativo",
            Estado = "ACTIVO",
            FechaCreacion = DateTime.Now,
            EsPublica = 1,
            IdGrupo = grupo.IdGrupo
        };
        
        context.Rubricas.AddRange(rubrica1, rubrica2, rubrica3);
        await context.SaveChangesAsync();

        // Obtener niveles de calificación existentes (deben ser creados manualmente por el usuario)
        var niveles = await context.NivelesCalificacion
            .OrderBy(n => n.OrdenNivel)
            .ToListAsync();
        
        // Si no hay niveles, NO insertar rúbricas de ejemplo
        if (!niveles.Any())
        {
            // Eliminar las rúbricas que acabamos de crear
            context.Rubricas.RemoveRange(rubrica1, rubrica2, rubrica3);
            await context.SaveChangesAsync();
            return; // Salir sin insertar datos de ejemplo
        }

        // Ítems de evaluación - Rúbrica 1: Proyecto de Investigación
        var itemsRubrica1 = new List<ItemEvaluacion>
        {
            new ItemEvaluacion { NombreItem = "Análisis del problema", Descripcion = "Identificación y análisis profundo del problema planteado", IdRubrica = rubrica1.IdRubrica, OrdenItem = 1, Peso = 25.0M },
            new ItemEvaluacion { NombreItem = "Formulación matemática", Descripcion = "Modelado matemático correcto del problema", IdRubrica = rubrica1.IdRubrica, OrdenItem = 2, Peso = 25.0M },
            new ItemEvaluacion { NombreItem = "Solución e interpretación", Descripcion = "Aplicación de métodos de solución e interpretación de resultados", IdRubrica = rubrica1.IdRubrica, OrdenItem = 3, Peso = 25.0M },
            new ItemEvaluacion { NombreItem = "Presentación y documentación", Descripcion = "Calidad de la presentación escrita y documentación del proyecto", IdRubrica = rubrica1.IdRubrica, OrdenItem = 4, Peso = 25.0M }
        };
        
        // Ítems de evaluación - Rúbrica 2: Presentación Oral
        var itemsRubrica2 = new List<ItemEvaluacion>
        {
            new ItemEvaluacion { NombreItem = "Claridad en la exposición", Descripcion = "Capacidad de comunicar ideas de forma clara y organizada", IdRubrica = rubrica2.IdRubrica, OrdenItem = 1, Peso = 30.0M },
            new ItemEvaluacion { NombreItem = "Dominio del tema", Descripcion = "Conocimiento profundo y respuestas a preguntas", IdRubrica = rubrica2.IdRubrica, OrdenItem = 2, Peso = 35.0M },
            new ItemEvaluacion { NombreItem = "Material de apoyo", Descripcion = "Calidad de las diapositivas y recursos visuales", IdRubrica = rubrica2.IdRubrica, OrdenItem = 3, Peso = 20.0M },
            new ItemEvaluacion { NombreItem = "Gestión del tiempo", Descripcion = "Uso efectivo del tiempo asignado", IdRubrica = rubrica2.IdRubrica, OrdenItem = 4, Peso = 15.0M }
        };
        
        // Ítems de evaluación - Rúbrica 3: Trabajo en Equipo
        var itemsRubrica3 = new List<ItemEvaluacion>
        {
            new ItemEvaluacion { NombreItem = "Colaboración", Descripcion = "Participación activa y apoyo a los compañeros", IdRubrica = rubrica3.IdRubrica, OrdenItem = 1, Peso = 30.0M },
            new ItemEvaluacion { NombreItem = "Comunicación", Descripcion = "Comunicación efectiva dentro del equipo", IdRubrica = rubrica3.IdRubrica, OrdenItem = 2, Peso = 25.0M },
            new ItemEvaluacion { NombreItem = "Responsabilidad", Descripcion = "Cumplimiento de tareas y compromisos", IdRubrica = rubrica3.IdRubrica, OrdenItem = 3, Peso = 25.0M },
            new ItemEvaluacion { NombreItem = "Resolución de conflictos", Descripcion = "Capacidad de resolver desacuerdos constructivamente", IdRubrica = rubrica3.IdRubrica, OrdenItem = 4, Peso = 20.0M }
        };
        
        var items = new List<ItemEvaluacion>();
        items.AddRange(itemsRubrica1);
        items.AddRange(itemsRubrica2);
        items.AddRange(itemsRubrica3);
        context.ItemsEvaluacion.AddRange(items);
        await context.SaveChangesAsync();

        // Valores de rúbrica - crear valores para todos los ítems y niveles
        var valores = new List<ValorRubrica>();
        var valoresPuntos = new decimal[] { 4.0M, 3.0M, 2.0M, 1.0M };
        
        foreach (var item in items)
        {
            for (int j = 0; j < niveles.Count; j++)
            {
                valores.Add(new ValorRubrica
                {
                    IdItem = item.IdItem,
                    IdNivel = niveles[j].IdNivel,
                    IdRubrica = item.IdRubrica,
                    ValorPuntos = valoresPuntos[j]
                });
            }
        }
        context.ValoresRubrica.AddRange(valores);
        await context.SaveChangesAsync();

        // RubricaNiveles - asociar niveles con todas las rúbricas
        var rubricaNiveles = new List<RubricaNivel>();
        
        foreach (var rubrica in new[] { rubrica1, rubrica2, rubrica3 })
        {
            foreach (var nivel in niveles)
            {
                rubricaNiveles.Add(new RubricaNivel
                {
                    IdRubrica = rubrica.IdRubrica,
                    IdNivel = nivel.IdNivel
                });
            }
        }
        context.RubricaNiveles.AddRange(rubricaNiveles);
        await context.SaveChangesAsync();

        // Estudiante de prueba
        var estudiante = new Estudiante
        {
            Nombre = "JUAN PABLO",
            Apellidos = "ABARCA BRENES",
            NumeroId = "020839034",
            DireccionCorreo = "juan.pablo@uned.cr",
            Institucion = "PALMARES (06)",
            Grupos = "Grupo 02: Tutor Heiner Guido Cambronero",
            Anio = 2025,
            PeriodoAcademicoId = periodo.Id
        };
        context.Estudiantes.Add(estudiante);
        await context.SaveChangesAsync();
    }
}