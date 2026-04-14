# 🔒 Reporte de Vulnerabilidades NuGet - RubricasApp.Web

**Fecha de análisis:** 28 de enero de 2025  
**Proyecto:** RubricasApp.Web (.NET 8.0)  
**Generado por:** `dotnet list package --vulnerable --include-transitive`

---

## ⚠️ RESUMEN EJECUTIVO

Se encontraron **9 paquetes vulnerables** (todos transientes/indirectos):

| Gravedad | Cantidad |
|----------|----------|
| 🔴 **CRÍTICA** | 1 |
| 🟠 **ALTA** | 7 |
| 🟡 **MODERADA** | 1 |

**RECOMENDACIÓN:** Actualizar inmediatamente los paquetes principales para resolver vulnerabilidades transientes.

---

## 🔴 VULNERABILIDADES CRÍTICAS

### 1. NuGet.Packaging 6.3.1
- **Gravedad:** CRÍTICA (Critical)
- **Versión actual:** 6.3.1
- **Asesoría:** [GHSA-68w7-72jg-6qpp](https://github.com/advisories/GHSA-68w7-72jg-6qpp)
- **Impacto:** Potencial ejecución de código remoto durante restauración de paquetes
- **Paquete padre:** `NuGet.Common 7.3.0`

**ACCIÓN INMEDIATA:**
```powershell
dotnet add package NuGet.Common --version 8.0.1
```

---

## 🟠 VULNERABILIDADES ALTAS

### 2. Microsoft.AspNetCore.Http 2.0.1
- **Gravedad:** ALTA (High)
- **Versión actual:** 2.0.1
- **Asesoría:** [GHSA-hxrm-9w7p-39cc](https://github.com/advisories/GHSA-hxrm-9w7p-39cc)
- **Impacto:** Denegación de servicio (DoS) mediante headers HTTP malformados
- **Paquete padre:** Rotativa.AspNetCore 1.2.0

**ACCIÓN:**
```powershell
dotnet add package Rotativa.AspNetCore --version 1.4.0
```

---

### 3. Microsoft.Data.SqlClient 5.1.1
- **Gravedad:** ALTA (High)
- **Versión actual:** 5.1.1
- **Asesoría:** [GHSA-98g6-xh36-x2p7](https://github.com/advisories/GHSA-98g6-xh36-x2p7)
- **Impacto:** Divulgación de información a través de mensajes de error de SQL Server
- **Paquete padre:** Microsoft.EntityFrameworkCore.SqlServer 8.0.0

**ACCIÓN:**
```powershell
# Actualizar Entity Framework a versión 8.0.13 o superior
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.13
```

---

### 4. Microsoft.Extensions.Caching.Memory 8.0.0
- **Gravedad:** ALTA (High)
- **Versión actual:** 8.0.0
- **Asesoría:** [GHSA-qj66-m88j-hmgj](https://github.com/advisories/GHSA-qj66-m88j-hmgj)
- **Impacto:** Denegación de servicio por consumo excesivo de memoria
- **Paquete padre:** Microsoft.EntityFrameworkCore 8.0.0

**ACCIÓN:**
```powershell
# Se resuelve actualizando EF Core (ver punto 3)
```

---

### 5. NuGet.Protocol 6.3.1
- **Gravedad:** ALTA (High)
- **Versión actual:** 6.3.1
- **Asesoría:** [GHSA-6qmf-mmc7-6c2p](https://github.com/advisories/GHSA-6qmf-mmc7-6c2p)
- **Impacto:** Deserialización insegura durante restauración de paquetes
- **Paquete padre:** NuGet.Common 7.3.0

**ACCIÓN:**
```powershell
# Se resuelve actualizando NuGet.Common (ver punto 1)
```

---

### 6. System.Formats.Asn1 5.0.0
- **Gravedad:** ALTA (High)
- **Versión actual:** 5.0.0
- **Asesoría:** [GHSA-447r-wph3-92pm](https://github.com/advisories/GHSA-447r-wph3-92pm)
- **Impacto:** Denegación de servicio al procesar certificados X.509
- **Paquete padre:** Azure.Identity 1.19.0

**ACCIÓN:**
```powershell
dotnet add package Azure.Identity --version 1.14.1
```

---

### 7. System.Net.Http 4.3.0
- **Gravedad:** ALTA (High)
- **Versión actual:** 4.3.0
- **Asesoría:** [GHSA-7jgj-8wvc-jh57](https://github.com/advisories/GHSA-7jgj-8wvc-jh57)
- **Impacto:** Divulgación de información mediante cabeceras HTTP
- **Paquete padre:** Múltiples paquetes legacy

**ACCIÓN:**
```powershell
# Se resuelve actualizando todos los paquetes de Microsoft a versiones 8.0.13+
```

---

### 8. System.Text.RegularExpressions 4.3.0
- **Gravedad:** ALTA (High)
- **Versión actual:** 4.3.0
- **Asesoría:** [GHSA-cmhx-cq75-c4mj](https://github.com/advisories/GHSA-cmhx-cq75-c4mj)
- **Impacto:** Denegación de servicio mediante regex complejas (ReDoS)
- **Paquete padre:** Múltiples paquetes de Microsoft

**ACCIÓN:**
```powershell
# Se resuelve actualizando EF Core y otros paquetes de Microsoft
```

---

## 🟡 VULNERABILIDADES MODERADAS

### 9. System.IdentityModel.Tokens.Jwt 6.24.0
- **Gravedad:** MODERADA (Moderate)
- **Versión actual:** 6.24.0
- **Asesoría:** [GHSA-59j7-ghrg-fj52](https://github.com/advisories/GHSA-59j7-ghrg-fj52)
- **Impacto:** Validación incorrecta de tokens JWT
- **Paquete padre:** Microsoft.AspNetCore.Identity 8.0.0

**ACCIÓN:**
```powershell
# Se resuelve actualizando Identity a 8.0.13+
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.13
```

---

## 📦 PAQUETES DESACTUALIZADOS (Sin Vulnerabilidades)

| Paquete | Versión Actual | Última Disponible |
|---------|----------------|-------------------|
| Bootstrap | 5.3.0 | 5.3.8 |
| jQuery | 3.7.0 | 3.7.1 |
| PdfSharp | 6.2.2 | 6.2.4 |
| QuestPDF | 2025.7.4 | 2026.2.4 |
| Rotativa.AspNetCore | 1.2.0 | 1.4.0 |

---

## ⚠️ CONFLICTOS DE VERSIONES

### AutoMapper
```
WARNING NU1608: AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1 
requiere AutoMapper (= 12.0.1), pero se resolvió la versión 16.1.1
```

**ACCIÓN:**
```powershell
# Actualizar ambos paquetes a versiones compatibles
dotnet add package AutoMapper --version 13.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 13.0.1
```

---

## 🛠️ SCRIPT DE ACTUALIZACIÓN COMPLETO

```powershell
# 1. CRÍTICO: Actualizar NuGet.Common
dotnet add package NuGet.Common --version 8.0.1

# 2. ALTO: Actualizar Entity Framework Core (resuelve múltiples vulnerabilidades)
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.13

# 3. ALTO: Actualizar ASP.NET Core Identity
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.13
dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --version 8.0.13

# 4. ALTO: Actualizar Azure.Identity
dotnet add package Azure.Identity --version 1.14.1

# 5. ALTO: Actualizar Rotativa (resuelve Microsoft.AspNetCore.Http)
dotnet add package Rotativa.AspNetCore --version 1.4.0

# 6. Actualizar Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 8.0.10

# 7. Corregir conflicto de AutoMapper
dotnet add package AutoMapper --version 13.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 13.0.1

# 8. Actualizar paquetes UI (opcional pero recomendado)
dotnet add package Bootstrap --version 5.3.8
dotnet add package jQuery --version 3.7.1
dotnet add package PdfSharp --version 6.2.4
dotnet add package QuestPDF --version 2026.2.4

# 9. Restaurar y verificar
dotnet restore
dotnet list package --vulnerable
dotnet build
```

---

## 🚀 SCRIPT ALTERNATIVO (Modo Conservador)

Si prefieres actualizar solo las vulnerabilidades críticas y altas sin cambiar versiones mayores:

```powershell
# Solo vulnerabilidades críticas/altas manteniendo .NET 8
dotnet add package NuGet.Common --version 8.0.1
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.13
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.13
dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --version 8.0.13
dotnet add package Azure.Identity --version 1.14.1
dotnet add package Rotativa.AspNetCore --version 1.4.0
dotnet add package AutoMapper --version 13.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 13.0.1

dotnet restore
dotnet build
```

---

## 📋 CHECKLIST DE ACTUALIZACIÓN

### Antes de actualizar:
- [ ] Hacer backup de la base de datos
- [ ] Crear branch de actualización: `git checkout -b fix/update-vulnerable-packages`
- [ ] Commit de cambios actuales
- [ ] Revisar documentación de breaking changes de cada paquete

### Durante la actualización:
- [ ] Ejecutar script de actualización
- [ ] Ejecutar `dotnet restore`
- [ ] Ejecutar `dotnet build`
- [ ] Revisar warnings de compilación
- [ ] Ejecutar todas las pruebas unitarias

### Después de actualizar:
- [ ] Probar funcionalidades críticas:
  - [ ] Login/Logout
  - [ ] Crear evaluación
  - [ ] Enviar correos
  - [ ] Exportar Excel/PDF
  - [ ] CRUD de estudiantes
  - [ ] Módulo de conducta
- [ ] Verificar logs de errores
- [ ] Commit de cambios
- [ ] Merge a rama principal

---

## 🔍 VERIFICACIÓN POST-ACTUALIZACIÓN

```powershell
# Verificar que no hay vulnerabilidades
dotnet list package --vulnerable --include-transitive

# Debería mostrar:
# "no known vulnerabilities"

# Verificar conflictos de versiones
dotnet list package

# No debería haber warnings NU1608
```

---

## 📊 DETALLE DE PAQUETES VULNERABLES

### Paquetes Transientes (Indirectos)

| Paquete | Versión | Gravedad | Padre Directo | Solución |
|---------|---------|----------|---------------|----------|
| NuGet.Packaging | 6.3.1 | 🔴 CRÍTICA | NuGet.Common 7.3.0 | Actualizar a 8.0.1 |
| Microsoft.Data.SqlClient | 5.1.1 | 🟠 ALTA | EF Core 8.0.0 | Actualizar a 8.0.13 |
| Microsoft.Extensions.Caching.Memory | 8.0.0 | 🟠 ALTA | EF Core 8.0.0 | Actualizar a 8.0.13 |
| Microsoft.AspNetCore.Http | 2.0.1 | 🟠 ALTA | Rotativa 1.2.0 | Actualizar a 1.4.0 |
| NuGet.Protocol | 6.3.1 | 🟠 ALTA | NuGet.Common 7.3.0 | Actualizar a 8.0.1 |
| System.Formats.Asn1 | 5.0.0 | 🟠 ALTA | Azure.Identity 1.19.0 | Actualizar a 1.14.1 |
| System.Net.Http | 4.3.0 | 🟠 ALTA | Legacy packages | Actualizar padres |
| System.Text.RegularExpressions | 4.3.0 | 🟠 ALTA | Legacy packages | Actualizar padres |
| System.IdentityModel.Tokens.Jwt | 6.24.0 | 🟡 MODERADA | Identity 8.0.0 | Actualizar a 8.0.13 |

---

## 🎯 PLAN DE ACCIÓN RECOMENDADO

### Fase 1: Actualizaciones Críticas (URGENTE)
**Tiempo estimado:** 15 minutos  
**Riesgo:** Bajo

```powershell
# Paso 1: Actualizar NuGet.Common (CRÍTICO)
dotnet add package NuGet.Common --version 8.0.1

# Paso 2: Verificar compilación
dotnet build

# Paso 3: Commit
git add RubricasApp.Web.csproj
git commit -m "fix: Actualizar NuGet.Common para resolver vulnerabilidad crítica GHSA-68w7-72jg-6qpp"
```

---

### Fase 2: Actualizaciones Altas (PRIORITARIO)
**Tiempo estimado:** 30 minutos  
**Riesgo:** Medio

```powershell
# Actualizar Entity Framework Core (resuelve 5 vulnerabilidades)
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.13
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.13

# Actualizar Identity (resuelve 1 vulnerabilidad)
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.13
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.13

# Actualizar Azure.Identity (resuelve 1 vulnerabilidad)
dotnet add package Azure.Identity --version 1.14.1

# Actualizar Rotativa (resuelve 1 vulnerabilidad)
dotnet add package Rotativa.AspNetCore --version 1.4.0

# Verificar
dotnet restore
dotnet build

# Commit
git add RubricasApp.Web.csproj
git commit -m "fix: Actualizar paquetes para resolver 7 vulnerabilidades altas en EF Core, Identity, Azure y Rotativa"
```

---

### Fase 3: Corrección de Conflictos
**Tiempo estimado:** 10 minutos  
**Riesgo:** Bajo

```powershell
# Corregir AutoMapper
dotnet add package AutoMapper --version 13.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 13.0.1

dotnet build

git add RubricasApp.Web.csproj
git commit -m "fix: Resolver conflicto de versiones de AutoMapper"
```

---

### Fase 4: Actualizaciones Opcionales (RECOMENDADO)
**Tiempo estimado:** 15 minutos  
**Riesgo:** Bajo

```powershell
# UI y PDF
dotnet add package Bootstrap --version 5.3.8
dotnet add package jQuery --version 3.7.1
dotnet add package PdfSharp --version 6.2.4
dotnet add package QuestPDF --version 2026.2.4
dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation --version 8.0.13
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 8.0.10

dotnet build

git add RubricasApp.Web.csproj
git commit -m "chore: Actualizar paquetes UI y PDF a versiones más recientes"
```

---

## 🧪 PRUEBAS REQUERIDAS DESPUÉS DE ACTUALIZAR

### Pruebas Funcionales Críticas

#### 1. Sistema de Identity
```
✓ Login con admin@rubricas.edu
✓ Logout
✓ Crear nuevo usuario
✓ Asignar roles
✓ Verificar permisos
```

#### 2. Entity Framework Core
```
✓ CRUD de estudiantes
✓ CRUD de evaluaciones
✓ CRUD de rúbricas
✓ Consultas complejas en reportes
✓ Transacciones
```

#### 3. Envío de Correos
```
✓ Enviar evaluación individual
✓ Enviar evaluaciones masivas
✓ Verificar formato HTML
```

#### 4. Generación de PDFs
```
✓ Exportar evaluación a PDF (Rotativa)
✓ Exportar reportes a PDF (QuestPDF)
```

#### 5. Exportación Excel
```
✓ Exportar estudiantes (EPPlus)
✓ Exportar calificaciones (ClosedXML)
```

---

## 🚨 BREAKING CHANGES CONOCIDOS

### Entity Framework Core 8.0.0 → 8.0.13
- ✅ Sin breaking changes (solo fixes de seguridad)

### Identity 8.0.0 → 8.0.13
- ✅ Sin breaking changes

### AutoMapper 12.0.1 → 13.0.1
- ⚠️ Cambios menores en configuración de mapeos
- **Verificar:** Perfiles de AutoMapper en `Program.cs`

### Rotativa 1.2.0 → 1.4.0
- ✅ Sin breaking changes reportados
- **Verificar:** Generación de PDFs en `PdfService`

### QuestPDF 2025.7.4 → 2026.2.4
- ⚠️ Posibles cambios en API de generación
- **Verificar:** Todas las plantillas de PDF

---

## 📝 NOTAS IMPORTANTES

### Sobre .NET 10.0.5
Los paquetes de Microsoft tienen versiones 10.0.5 disponibles, **PERO** estás en **.NET 8.0**:

- ✅ **USAR:** Versiones 8.0.13 (compatibles con .NET 8)
- ❌ **NO USAR:** Versiones 10.x (requieren .NET 9/10)

### Sobre Azure.Identity
La versión recomendada es **1.14.1** (no 1.19.0):
- Versión 1.19.0 tiene vulnerabilidad en System.Formats.Asn1
- Versión 1.14.1 es la última estable sin vulnerabilidades para .NET 8

---

## 🔄 ESTRATEGIA DE ROLLBACK

Si algo falla después de actualizar:

```powershell
# Opción 1: Rollback con Git
git reset --hard HEAD~1
dotnet restore
dotnet build

# Opción 2: Rollback selectivo
# Editar RubricasApp.Web.csproj y restaurar versiones anteriores
dotnet restore
dotnet build
```

---

## 📈 IMPACTO ESPERADO

### Antes de Actualizar
- 🔴 9 vulnerabilidades (1 crítica, 7 altas, 1 moderada)
- ⚠️ 1 conflicto de versiones

### Después de Actualizar
- ✅ 0 vulnerabilidades
- ✅ 0 conflictos de versiones
- ✅ Mejoras de rendimiento y estabilidad
- ✅ Compatibilidad con parches de seguridad futuros

---

## 📞 SOPORTE Y RECURSOS

### Documentación Oficial
- [Security advisories - GitHub](https://github.com/advisories)
- [.NET Security Updates](https://github.com/dotnet/announcements/labels/security)
- [Entity Framework Core Releases](https://learn.microsoft.com/ef/core/what-is-new/)

### Contacto
Para problemas después de actualizar:
- Revisar logs: `logs/rubricasapp-[fecha].log`
- Verificar compilación: `dotnet build -v detailed`
- Consultar GitHub Issues del proyecto

---

## ⏱️ TIEMPO TOTAL ESTIMADO

| Fase | Tiempo | Prioridad |
|------|--------|-----------|
| Fase 1: Críticas | 15 min | 🔴 URGENTE |
| Fase 2: Altas | 30 min | 🟠 PRIORITARIO |
| Fase 3: Conflictos | 10 min | 🟡 IMPORTANTE |
| Fase 4: Opcionales | 15 min | 🟢 RECOMENDADO |
| **TOTAL** | **70 min** | |

---

## ✅ CHECKLIST FINAL

- [ ] Ejecutar script de actualización
- [ ] Verificar compilación exitosa
- [ ] Ejecutar pruebas funcionales
- [ ] Verificar vulnerabilidades resueltas
- [ ] Commit de cambios
- [ ] Push al repositorio
- [ ] Desplegar en ambiente de pruebas
- [ ] Validar en producción
- [ ] Documentar cambios

---

**RECOMENDACIÓN FINAL:** Ejecutar **Fase 1 y Fase 2 inmediatamente**. Las vulnerabilidades críticas y altas representan riesgos de seguridad significativos para el sistema MEP.
