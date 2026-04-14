# 🧩 Diseño del Módulo de Asistencia por Bloque/Lección -- Prompt en Markdown

## 🎯 Instrucción General

Actúa como un **arquitecto de bases de datos** y **desarrollador
senior** en **C#, ASP.NET Core y SQL Server**.\
Necesito diseñar **desde cero** un módulo de **registro de asistencia
por bloque/lección** para centros educativos del MEP en Costa Rica.

## 📘 Contexto General del Sistema

-   Cada estudiante pertenece a **uno o varios grupos**
    (secciones/curso).
-   Cada grupo tiene un **horario** con varias **lecciones (bloques)**
    por día, por materia.
-   La asistencia debe registrarse **por estudiante**, **por grupo**,
    **por fecha** y **por bloque/lección**.
-   Para cada bloque, el estudiante puede estar en: `Presente`,
    `Ausente`, `Tardanza`, `Justificada`.

## 🗄️ Requerimientos de Diseño de Base de Datos (SQL Server)

### Tablas existentes:

-   Estudiantes\
-   GruposEstudiantes\
-   Materias

### Tablas nuevas:

#### Lecciones o BloquesHorario

-   IdLeccion (PK)\
-   IdGrupo (FK)\
-   Materia\
-   NumeroBloque o BloqueHorario\
-   HoraInicio\
-   HoraFin

#### AsistenciaxLeccion

-   IdAsistencia (PK, IDENTITY)\
-   IdEstudiante (FK)\
-   IdGrupo (FK)\
-   IdLeccion (FK)\
-   Fecha\
-   Estado\
-   Observaciones\
-   RegistradoPorId\
-   FechaRegistro\
-   FechaModificacion

### Restricción de unicidad

`(IdEstudiante, IdGrupo, IdLeccion, Fecha)` debe ser **único**.

### Índices recomendados

-   Fecha\
-   IdGrupo\
-   IdEstudiante

## 🧱 Modelo en C# EF Core

Generar entidades: - Estudiante\
- Grupo\
- Leccion\
- Asistencia

Configurar: - Relaciones\
- Clave única\
- Reglas de longitud y tipos

## 📐 Reglas de Negocio

1.  No permitir duplicados de asistencia por bloque.\
2.  Si se registra de nuevo, **actualizar**, no insertar.\
3.  Validar transiciones de estado.

## 📊 Consultas SQL útiles

-   Porcentaje de asistencia\
-   Ausencias por grupo/materia\
-   Estudiantes con X ausencias
