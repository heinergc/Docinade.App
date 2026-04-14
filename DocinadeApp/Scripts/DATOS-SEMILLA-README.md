# Datos Semilla de Conducta - Resumen

## 📊 Datos Insertados Exitosamente

Se han creado **6 estudiantes de prueba** con diferentes escenarios de conducta para visualizar todas las funcionalidades del sistema.

---

## 👨‍🎓 Estudiantes Creados

### 1. **María González Riesgo** (TEST-001)
- **Nota Final:** 60 pts
- **Estado:** ⚠️ **EN RIESGO** (60-64 puntos)
- **Total Rebajos:** 40 puntos
- **Boletas:** 3 activas
  - Grave (15 pts): Uso de celular en clase durante evaluación
  - Leve (10 pts): Llegada tardía reiterada (3 veces en una semana)
  - Grave (15 pts): Falta de respeto verbal a compañero
- **Programa de Acciones:** No
- **Decisión Profesional:** No

### 2. **Carlos Ramírez Aplazado** (TEST-002)
- **Nota Final:** 50 pts
- **Estado:** ❌ **APLAZADO** (< 60 puntos)
- **Total Rebajos:** 50 puntos
- **Boletas:** 3 activas
  - Gravísima (25 pts): Agresión física a compañero en el recreo
  - Grave (15 pts): Sustracción de material didáctico
  - Leve (10 pts): No traer material requerido repetidamente
- **Programa de Acciones:** ⚠️ **Sí, requiere** (no creado aún)
- **Decisión Profesional:** No

### 3. **Ana Martínez Programa** (TEST-003)
- **Nota Final:** 55 pts
- **Estado:** ❌ **APLAZADO** (< 60 puntos)
- **Total Rebajos:** 45 puntos
- **Boletas:** 3 activas
  - Muy grave (20 pts): Copia en examen final de matemáticas
  - Grave (15 pts): Falsificación de firma en documento de permiso
  - Leve (10 pts): Interrupciones constantes en clase
- **Programa de Acciones:** ✅ **Sí, CREADO**
  - **Título:** Programa de Mejoramiento Conductual - Ana Martínez
  - **Estado:** En Proceso
  - **Duración:** 60 días
  - **Actividades:** Sesiones con orientador, diario reflexivo, servicio comunitario, taller de valores
- **Decisión Profesional:** No

### 4. **Luis Pérez Decisión** (TEST-004)
- **Nota Calculada:** 40 pts → **Ajustada a:** 70 pts
- **Estado:** ✅ **APROBADO** (por decisión profesional)
- **Total Rebajos:** 60 puntos
- **Boletas:** 3 activas
  - Gravísima (25 pts): Daño intencional a propiedad institucional
  - Muy grave (20 pts): Acoso verbal sistemático a compañero
  - Grave (15 pts): Salida no autorizada de la institución
- **Programa de Acciones:** No
- **Decisión Profesional:** ✅ **Sí, APLICADA**
  - **Acta:** ACTA-2025-001
  - **Decisión:** Ajustar nota a 70 pts con compromiso de mejora
  - **Justificación:** Arrepentimiento genuino, circunstancias familiares difíciles
  - **Condiciones:** Terapia psicológica, mayor supervisión parental, seguimiento mensual

### 5. **Elena Torres Buena** (TEST-005)
- **Nota Final:** 95 pts
- **Estado:** ✅ **APROBADO** (≥ 70 puntos)
- **Total Rebajos:** 5 puntos
- **Boletas:** 1 activa
  - Muy leve (5 pts): Olvido menor - no traer cuaderno en una ocasión
- **Programa de Acciones:** No
- **Decisión Profesional:** No

### 6. **Pedro Sánchez Múltiple** (TEST-006)
- **Nota Final:** 62 pts
- **Estado:** ⚠️ **EN RIESGO** (60-64 puntos)
- **Total Rebajos:** 38 puntos
- **Boletas:** 5 activas
  - Leve (8 pts): Incumplimiento de tareas 4 veces consecutivas
  - Grave (15 pts): Lenguaje inapropiado en clase
  - Leve (8 pts): Uniforme incompleto repetidamente
  - Muy leve (2 pts): Masticar chicle en clase
  - Muy leve (5 pts): Conversaciones durante explicación del profesor
- **Programa de Acciones:** No
- **Decisión Profesional:** No

---

## 🎯 Escenarios para Visualizar

### 1. **Dashboard de Conducta**
Navega a: `/NotaConducta/Dashboard`

Podrás visualizar:
- 📊 Total de estudiantes: 6
- ⚠️ Estudiantes en riesgo: 2 (María, Pedro)
- ❌ Estudiantes aplazados: 2 (Carlos, Ana - antes del ajuste de Luis)
- ✅ Estudiantes aprobados: 2 (Elena, Luis)
- 📈 Distribución por tipo de falta
- 📋 Últimas boletas emitidas
- 👥 Lista de estudiantes en riesgo/aplazados

### 2. **Boletas por Estudiante**
Navega a: `/NotaConducta/EstudianteNota?idEstudiante=X&idPeriodo=1`

Reemplaza X con el ID del estudiante:
- María (TEST-001): ID 3
- Carlos (TEST-002): ID 4
- Ana (TEST-003): ID 5
- Luis (TEST-004): ID 6
- Elena (TEST-005): ID 7
- Pedro (TEST-006): ID 8

### 3. **Programas de Acciones**
Navega a: `/NotaConducta/ProgramasAcciones`

Verás:
- 1 programa activo para Ana Martínez
- Estado: En Proceso
- Detalles completos del programa

### 4. **Decisiones Profesionales**
Navega a: `/NotaConducta/DecisionesProfesionales`

Verás:
- 1 decisión profesional para Luis Pérez
- Acta ACTA-2025-001
- Ajuste de nota de 40 a 70 puntos

### 5. **Reporte General de Conducta**
Navega a: `/NotaConducta/ReporteGeneralConducta?idPeriodo=1`

Verás el reporte completo con todos los estudiantes y sus estadísticas.

### 6. **Exportar a Excel**
Desde el reporte general, puedes exportar los datos a Excel para análisis offline.

---

## 📁 Archivos del Script

Los scripts SQL se encuentran en:
- `Scripts/seed-data-conducta.sql` - Script completo con comentarios
- `Scripts/insertar-datos-semilla.sql` - Script ejecutable optimizado
- `Scripts/seed-data-output.txt` - Log de ejecución

---

## 🔄 Reejecutar el Script

Si necesitas restablecer los datos de prueba:

```powershell
# Eliminar datos anteriores
sqlcmd -S localhost\SQLEXPRESS -U sa -P "LKSA2014.01.05.14" -d RubricasDb -Q "DELETE FROM NotasConducta WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%'); DELETE FROM BoletasConducta WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%'); DELETE FROM EstudianteGrupos WHERE EstudianteId IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%'); DELETE FROM Estudiantes WHERE NumeroId LIKE 'TEST-%';"

# Reinsertar datos
sqlcmd -S localhost\SQLEXPRESS -U sa -P "LKSA2014.01.05.14" -d RubricasDb -i ".\Scripts\insertar-datos-semilla.sql"
```

---

## ✅ Verificación

Para verificar que los datos se insertaron correctamente:

```sql
-- Resumen general
SELECT 
    e.NumeroId AS 'ID',
    e.Nombre + ' ' + e.Apellidos AS 'Estudiante',
    n.NotaFinal AS 'Nota',
    n.Estado AS 'Estado',
    n.TotalRebajos AS 'Rebajos',
    COUNT(b.IdBoleta) AS 'Boletas'
FROM Estudiantes e
INNER JOIN NotasConducta n ON e.IdEstudiante = n.IdEstudiante
LEFT JOIN BoletasConducta b ON e.IdEstudiante = b.IdEstudiante AND b.Estado = 'Activa'
WHERE e.NumeroId LIKE 'TEST-%'
GROUP BY e.NumeroId, e.Nombre, e.Apellidos, n.NotaFinal, n.Estado, n.TotalRebajos
ORDER BY n.NotaFinal;

-- Programas de acciones
SELECT * FROM ProgramasAccionesInstitucional WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%');

-- Decisiones profesionales
SELECT * FROM DecisionesProfesionalesConducta WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%');
```

---

## 🎓 Notas Importantes

1. **Todos los estudiantes están en el Grupo 60** (Décimo-A)
2. **Periodo Académico:** 1 (activo)
3. **Docente Emisor:** admin@rubricas.edu
4. **Las fechas de boletas están distribuidas** en los últimos 50 días para simular realismo
5. **Los rebajos cumplen con las reglas** de cada tipo de falta

---

## 🚀 Próximos Pasos

1. Navega al **Dashboard de Conducta** para ver el resumen general
2. Explora cada estudiante individualmente para ver sus boletas
3. Revisa el **Programa de Acciones** de Ana Martínez
4. Consulta la **Decisión Profesional** de Luis Pérez
5. Genera reportes y exporta a Excel
6. Prueba los filtros y búsquedas

---

**¡Los datos semilla están listos para usar!** 🎉
