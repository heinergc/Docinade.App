-- Script para insertar datos iniciales del Módulo de Conducta

USE RubricasDb;
GO

-- Insertar tipos de falta según normativa educativa
IF NOT EXISTS (SELECT 1 FROM TiposFalta WHERE Nombre = 'Muy leve')
BEGIN
    INSERT INTO TiposFalta (Nombre, Definicion, Ejemplos, AccionCorrectiva, RebajoMinimo, RebajoMaximo, Orden, Activo, FechaCreacion)
    VALUES 
    ('Muy leve', 
     'Faltas menores que afectan mínimamente la convivencia escolar', 
     'Llegar tarde ocasionalmente, olvidar materiales, conversar en clase', 
     'Amonestación verbal, recordatorio de normas',
     1, 5, 1, 1, GETDATE());
    
    PRINT 'Tipo de falta MUY LEVE insertado.';
END
GO

IF NOT EXISTS (SELECT 1 FROM TiposFalta WHERE Nombre = 'Leve')
BEGIN
    INSERT INTO TiposFalta (Nombre, Definicion, Ejemplos, AccionCorrectiva, RebajoMinimo, RebajoMaximo, Orden, Activo, FechaCreacion)
    VALUES 
    ('Leve', 
     'Faltas que alteran levemente el ambiente de aprendizaje', 
     'Llegar tarde reiteradamente, no cumplir tareas menores, uso inadecuado de uniforme', 
     'Amonestación escrita, comunicación a padres',
     6, 15, 2, 1, GETDATE());
    
    PRINT 'Tipo de falta LEVE insertado.';
END
GO

IF NOT EXISTS (SELECT 1 FROM TiposFalta WHERE Nombre = 'Grave')
BEGIN
    INSERT INTO TiposFalta (Nombre, Definicion, Ejemplos, AccionCorrectiva, RebajoMinimo, RebajoMaximo, Orden, Activo, FechaCreacion)
    VALUES 
    ('Grave', 
     'Faltas que afectan significativamente la convivencia y el ambiente educativo', 
     'Faltasde respeto, uso de lenguaje inapropiado, incumplimiento reiterado de normas', 
     'Boleta de conducta, reunión con padres, compromiso de mejora',
     16, 30, 3, 1, GETDATE());
    
    PRINT 'Tipo de falta GRAVE insertado.';
END
GO

IF NOT EXISTS (SELECT 1 FROM TiposFalta WHERE Nombre = 'Muy grave')
BEGIN
    INSERT INTO TiposFalta (Nombre, Definicion, Ejemplos, AccionCorrectiva, RebajoMinimo, RebajoMaximo, Orden, Activo, FechaCreacion)
    VALUES 
    ('Muy grave', 
     'Faltas que atentan gravemente contra la integridad y el orden institucional', 
     'Agresión física, daño intencional a propiedad, fraude académico', 
     'Boleta de conducta, suspensión temporal, programa de acciones institucional',
     31, 50, 4, 1, GETDATE());
    
    PRINT 'Tipo de falta MUY GRAVE insertado.';
END
GO

IF NOT EXISTS (SELECT 1 FROM TiposFalta WHERE Nombre = 'Gravísima')
BEGIN
    INSERT INTO TiposFalta (Nombre, Definicion, Ejemplos, AccionCorrectiva, RebajoMinimo, RebajoMaximo, Orden, Activo, FechaCreacion)
    VALUES 
    ('Gravísima', 
     'Faltas de extrema gravedad que comprometen la seguridad institucional', 
     'Violencia grave, posesión de armas, tráfico de sustancias, acoso grave', 
     'Suspensión, expulsión, denuncia a autoridades, intervención especializada',
     51, 70, 5, 1, GETDATE());
    
    PRINT 'Tipo de falta GRAVÍSIMA insertado.';
END
GO

-- Insertar parámetro de nota mínima de aprobación de conducta
IF NOT EXISTS (SELECT 1 FROM ParametrosInstitucion WHERE Clave = 'CONDUCTA_NOTA_MINIMA_APROBACION')
BEGIN
    INSERT INTO ParametrosInstitucion 
    (Clave, Nombre, Descripcion, Valor, TipoDato, Categoria, Activo, FechaCreacion)
    VALUES 
    ('CONDUCTA_NOTA_MINIMA_APROBACION', 
     'Nota Mínima de Aprobación de Conducta', 
     'Calificación mínima requerida para aprobar la asignatura de conducta', 
     '70.00', 
     'decimal', 
     'Conducta', 
     1, 
     GETDATE());
    
    PRINT 'Parámetro CONDUCTA_NOTA_MINIMA_APROBACION insertado.';
END
GO

PRINT '';
PRINT '======================================';
PRINT 'Datos iniciales insertados exitosamente.';
PRINT '======================================';
GO
