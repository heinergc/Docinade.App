-- Script de prueba para verificar el estado después del recálculo
-- Este script simula lo que sucede cuando se llama CalcularNotaConductaAsync()

-- 1. Ver estado ANTES del recálculo
SELECT 'ANTES DEL RECALCULO:' AS Momento;
SELECT 
    IdNotaConducta, 
    IdEstudiante, 
    Estado, 
    NotaFinal, 
    TotalRebajos, 
    RequiereProgramaAcciones,
    FechaUltimaActualizacion
FROM NotasConducta 
WHERE IdEstudiante = 2 AND IdPeriodo = 1;

-- 2. Mostrar boletas activas (debería ser 0)
SELECT 'Boletas activas:' AS Informacion, COUNT(*) AS Cantidad
FROM BoletasConducta 
WHERE IdEstudiante = 2 AND IdPeriodo = 1 AND Estado = 'Activa';

-- 3. Sumar rebajos de boletas activas
SELECT 'Total rebajos calculado:' AS Informacion, 
    ISNULL(SUM(RebajoAplicado), 0) AS TotalRebajos
FROM BoletasConducta 
WHERE IdEstudiante = 2 AND IdPeriodo = 1 AND Estado = 'Activa';

-- 4. Simular el recálculo manual (lo que debería hacer el servicio automáticamente)
DECLARE @totalRebajos DECIMAL(5,2);
DECLARE @notaFinal DECIMAL(5,2);
DECLARE @estado NVARCHAR(50);
DECLARE @requierePrograma BIT;
DECLARE @notaMinima DECIMAL(5,2) = 80; -- Valor típico

-- Calcular total de rebajos
SELECT @totalRebajos = ISNULL(SUM(RebajoAplicado), 0)
FROM BoletasConducta 
WHERE IdEstudiante = 2 AND IdPeriodo = 1 AND Estado = 'Activa';

-- Calcular nota final
SET @notaFinal = 100 - @totalRebajos;
IF @notaFinal < 0 SET @notaFinal = 0;

-- Determinar estado (con la nueva lógica)
IF @totalRebajos = 0
BEGIN
    SET @estado = 'Aprobado';
    SET @requierePrograma = 0;
END
ELSE IF @notaFinal >= @notaMinima
BEGIN
    SET @estado = 'Aprobado';
    SET @requierePrograma = 0;
END
ELSE IF @notaFinal >= (@notaMinima - 5)
BEGIN
    SET @estado = 'Riesgo';
    SET @requierePrograma = 0;
END
ELSE
BEGIN
    SET @estado = 'Aplazado';
    SET @requierePrograma = 1;
END

-- Actualizar
UPDATE NotasConducta
SET TotalRebajos = @totalRebajos,
    NotaFinal = @notaFinal,
    Estado = @estado,
    RequiereProgramaAcciones = @requierePrograma,
    FechaUltimaActualizacion = GETDATE()
WHERE IdEstudiante = 2 AND IdPeriodo = 1;

-- 5. Ver estado DESPUÉS del recálculo
SELECT 'DESPUES DEL RECALCULO:' AS Momento;
SELECT 
    IdNotaConducta, 
    IdEstudiante, 
    Estado, 
    NotaFinal, 
    TotalRebajos, 
    RequiereProgramaAcciones,
    FechaUltimaActualizacion
FROM NotasConducta 
WHERE IdEstudiante = 2 AND IdPeriodo = 1;

-- Mostrar valores usados en el cálculo
SELECT 
    @totalRebajos AS TotalRebajosCalculado,
    @notaFinal AS NotaFinalCalculada,
    @estado AS EstadoCalculado,
    @requierePrograma AS RequiereProgramaCalculado;
