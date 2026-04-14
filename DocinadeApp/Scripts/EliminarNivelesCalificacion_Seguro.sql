-- Script SEGURO para eliminar niveles de calificación
-- Este script verifica dependencias antes de eliminar

USE RubricasDb;
GO

-- Mostrar información antes de eliminar
PRINT '=== INFORMACIÓN ANTES DE ELIMINAR ===';
SELECT 
    (SELECT COUNT(*) FROM NivelesCalificacion) as TotalNiveles,
    (SELECT COUNT(*) FROM ValoresRubrica) as TotalValores,
    (SELECT COUNT(*) FROM RubricaNiveles) as TotalRubricaNiveles,
    (SELECT COUNT(*) FROM DetallesEvaluacion WHERE IdNivel IS NOT NULL) as TotalDetallesConNivel;

-- Listar niveles existentes
PRINT '';
PRINT '=== NIVELES EXISTENTES ===';
SELECT 
    IdNivel,
    NombreNivel,
    Descripcion,
    OrdenNivel,
    IdGrupo,
    (SELECT NombreGrupo FROM GruposCalificacion WHERE IdGrupo = N.IdGrupo) as GrupoCalificacion
FROM NivelesCalificacion N
ORDER BY OrdenNivel;

-- Eliminar en orden correcto (de dependientes a padres)
PRINT '';
PRINT '=== INICIANDO ELIMINACIÓN ===';

BEGIN TRANSACTION;

BEGIN TRY
    -- Paso 1: Eliminar DetallesEvaluacion que referencian niveles
    DECLARE @DetallesEliminados INT;
    DELETE FROM DetallesEvaluacion WHERE IdNivel IS NOT NULL;
    SET @DetallesEliminados = @@ROWCOUNT;
    PRINT '✅ DetallesEvaluacion eliminados: ' + CAST(@DetallesEliminados AS VARCHAR);

    -- Paso 2: Eliminar ValoresRubrica
    DECLARE @ValoresEliminados INT;
    DELETE FROM ValoresRubrica;
    SET @ValoresEliminados = @@ROWCOUNT;
    PRINT '✅ ValoresRubrica eliminados: ' + CAST(@ValoresEliminados AS VARCHAR);

    -- Paso 3: Eliminar RubricaNiveles
    DECLARE @RubricaNivelesEliminados INT;
    DELETE FROM RubricaNiveles;
    SET @RubricaNivelesEliminados = @@ROWCOUNT;
    PRINT '✅ RubricaNiveles eliminados: ' + CAST(@RubricaNivelesEliminados AS VARCHAR);

    -- Paso 4: Eliminar NivelesCalificacion
    DECLARE @NivelesEliminados INT;
    DELETE FROM NivelesCalificacion;
    SET @NivelesEliminados = @@ROWCOUNT;
    PRINT '✅ NivelesCalificacion eliminados: ' + CAST(@NivelesEliminados AS VARCHAR);

    -- Verificación final
    PRINT '';
    PRINT '=== VERIFICACIÓN FINAL ===';
    SELECT 
        (SELECT COUNT(*) FROM NivelesCalificacion) as NivelesRestantes,
        (SELECT COUNT(*) FROM ValoresRubrica) as ValoresRestantes,
        (SELECT COUNT(*) FROM RubricaNiveles) as RubricaNivelesRestantes,
        (SELECT COUNT(*) FROM DetallesEvaluacion WHERE IdNivel IS NOT NULL) as DetallesConNivelRestantes;

    COMMIT TRANSACTION;
    PRINT '';
    PRINT '🎉 ÉXITO: Todos los niveles de calificación y sus datos relacionados han sido eliminados';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '❌ ERROR: No se pudieron eliminar los niveles de calificación';
    PRINT 'Mensaje: ' + ERROR_MESSAGE();
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR);
    
    -- Revertir cambios
    PRINT '';
    PRINT '⚠️ Se han revertido todos los cambios (ROLLBACK)';
END CATCH;
GO
