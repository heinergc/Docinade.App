-- =============================================
-- Script: Fix CalificacionObtenida Data Type
-- Descripcion: Corrige el tipo de dato de CalificacionObtenida de nvarchar a decimal
-- Fecha: 2025-10-27
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '========================================';
    PRINT 'Corrigiendo tipo de dato de CalificacionObtenida...';
    PRINT '========================================';

    -- Verificar si la columna existe
    IF EXISTS (
        SELECT 1 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'ProfesorCapacitacion' 
        AND COLUMN_NAME = 'CalificacionObtenida'
    )
    BEGIN
        -- Actualizar valores que no sean numericos a NULL
        UPDATE [dbo].[ProfesorCapacitacion]
        SET CalificacionObtenida = NULL
        WHERE ISNUMERIC(CalificacionObtenida) = 0 AND CalificacionObtenida IS NOT NULL;
        
        PRINT 'Valores no numericos actualizados a NULL';

        -- Cambiar el tipo de dato de la columna
        ALTER TABLE [dbo].[ProfesorCapacitacion]
        ALTER COLUMN CalificacionObtenida DECIMAL(4,2) NULL;
        
        PRINT 'Tipo de dato cambiado exitosamente a DECIMAL(4,2)';
        
        -- Verificar el cambio
        SELECT 
            COLUMN_NAME, 
            DATA_TYPE, 
            NUMERIC_PRECISION, 
            NUMERIC_SCALE
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'ProfesorCapacitacion' 
        AND COLUMN_NAME = 'CalificacionObtenida';
        
        PRINT '========================================';
        PRINT 'Correccion completada exitosamente!';
        PRINT '========================================';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: La columna CalificacionObtenida no existe en la tabla ProfesorCapacitacion';
        THROW 50001, 'Columna no encontrada', 1;
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT 'ERROR EN LA EJECUCION DEL SCRIPT';
    PRINT '========================================';
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
    
    THROW;
END CATCH;
