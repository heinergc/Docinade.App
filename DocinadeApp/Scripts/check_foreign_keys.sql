-- Verificar las foreign keys de las tablas relacionadas con Profesores
-- y si tienen configurado ON DELETE CASCADE

SELECT 
    OBJECT_NAME(fk.parent_object_id) AS TablaHija,
    fk.name AS NombreConstraint,
    OBJECT_NAME(fk.referenced_object_id) AS TablaPadre,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnaFK,
    fk.delete_referential_action_desc AS AccionEliminar
FROM 
    sys.foreign_keys AS fk
INNER JOIN 
    sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
WHERE 
    OBJECT_NAME(fk.referenced_object_id) = 'Profesores'
ORDER BY 
    OBJECT_NAME(fk.parent_object_id);
