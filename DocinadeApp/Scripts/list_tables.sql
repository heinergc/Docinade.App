SELECT 
    SCHEMA_NAME(t.schema_id) AS Esquema,
    t.name AS Tabla,
    (SELECT COUNT(*) FROM sys.columns c WHERE c.object_id = t.object_id) AS Columnas,
    CASE 
        WHEN t.temporal_type_desc = 'HISTORY_TABLE' THEN 'Temporal (Historial)'
        WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN 'Temporal (Sistema)'
        ELSE 'Regular'
    END AS Tipo
FROM sys.tables t
WHERE t.is_ms_shipped = 0
ORDER BY SCHEMA_NAME(t.schema_id), t.name;
