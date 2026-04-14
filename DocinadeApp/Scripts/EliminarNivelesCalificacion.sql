-- Script para eliminar todos los niveles de calificación
-- ADVERTENCIA: Este script eliminará TODOS los niveles de calificación y sus datos relacionados
-- Ejecutar con precaución

USE RubricasDb;
GO

BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Eliminar RubricaNiveles (relación entre rúbricas y niveles)
    DELETE FROM RubricaNiveles;
    PRINT '✅ RubricaNiveles eliminados';

    -- 2. Eliminar ValoresRubrica (valores asignados a niveles)
    DELETE FROM ValoresRubrica;
    PRINT '✅ ValoresRubrica eliminados';

    -- 3. Eliminar DetallesEvaluacion (evaluaciones que referencian niveles)
    DELETE FROM DetallesEvaluacion;
    PRINT '✅ DetallesEvaluacion eliminados';

    -- 4. Finalmente eliminar NivelesCalificacion
    DELETE FROM NivelesCalificacion;
    PRINT '✅ NivelesCalificacion eliminados';

    -- Mostrar conteo final
    SELECT 
        (SELECT COUNT(*) FROM NivelesCalificacion) as NivelesRestantes,
        (SELECT COUNT(*) FROM ValoresRubrica) as ValoresRestantes,
        (SELECT COUNT(*) FROM RubricaNiveles) as RubricaNivelesRestantes,
        (SELECT COUNT(*) FROM DetallesEvaluacion) as DetallesRestantes;

    COMMIT TRANSACTION;
    PRINT '🎉 Todos los niveles de calificación han sido eliminados exitosamente';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    PRINT '❌ Error al eliminar niveles de calificación:';
    PRINT @ErrorMessage;
    
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
