-- Created by GitHub Copilot in SSMS - review carefully before executing

-- Paso 1: Cambiar a modo SINGLE_USER para cerrar todas las conexiones
USE master;
GO

ALTER DATABASE [RubricasDb] 
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Paso 2: Restaurar la base de datos
RESTORE DATABASE [RubricasDb]
FROM DISK = N'D:\Fuentes_github\Bk.bak'
WITH REPLACE,
     RECOVERY;
GO

-- Paso 3: Volver a modo MULTI_USER
ALTER DATABASE [RubricasDb] 
SET MULTI_USER;
GO