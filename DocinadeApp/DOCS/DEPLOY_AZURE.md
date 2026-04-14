# Guía de Despliegue a Azure - RubricasApp.Web

## Pre-requisitos

1. **Cuenta de Azure** activa
2. **Azure CLI** instalado: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli
3. **SQL Server Management Studio** (opcional, para gestionar la BD)
4. **Contraseña de producción** para la base de datos Azure SQL

---

## Paso 1: Preparar Base de Datos Azure SQL

### 1.1 Crear SQL Database en Azure Portal

```bash
# Login en Azure
az login

# Crear grupo de recursos (si no existe)
az group create --name RubricasAppRG --location eastus

# Crear servidor SQL
az sql server create \
  --name serverrubricasapp \
  --resource-group RubricasAppRG \
  --location eastus \
  --admin-user RubricasUser \
  --admin-password "TU_CONTRASEÑA_SEGURA"

# Crear base de datos
az sql db create \
  --resource-group RubricasAppRG \
  --server serverrubricasapp \
  --name RubricasDb \
  --service-objective S0 \
  --backup-storage-redundancy Local

# Permitir acceso desde Azure Services
az sql server firewall-rule create \
  --resource-group RubricasAppRG \
  --server serverrubricasapp \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Permitir tu IP local (para gestión)
az sql server firewall-rule create \
  --resource-group RubricasAppRG \
  --server serverrubricasapp \
  --name AllowMyIP \
  --start-ip-address TU_IP_PUBLICA \
  --end-ip-address TU_IP_PUBLICA
```

### 1.2 Obtener Connection String

```bash
az sql db show-connection-string \
  --client ado.net \
  --name RubricasDb \
  --server serverrubricasapp
```

**Connection String esperado:**
```
Server=tcp:serverrubricasapp.database.windows.net,1433;Initial Catalog=RubricasDb;Persist Security Info=False;User ID=RubricasUser;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

---

## Paso 2: Migrar Base de Datos Local a Azure

### Opción A: Backup/Restore (Recomendado)

```sql
-- En SQL Server Local
BACKUP DATABASE RubricasDb 
TO DISK = 'C:\Backups\RubricasDb.bak'
WITH COMPRESSION, INIT;
```

Luego importar en Azure usando **Azure Data Studio** o **SSMS**:
1. Conectar a `serverrubricasapp.database.windows.net`
2. Right-click en Databases → Import Data-tier Application
3. Seleccionar archivo `.bacpac` generado

### Opción B: Script de Schema + Datos

```bash
# Generar script completo de la BD local
# Usar SSMS: Tasks → Generate Scripts → Include Schema and Data
```

---

## Paso 3: Configurar App Service en Azure

### 3.1 Crear App Service

```bash
# Crear App Service Plan
az appservice plan create \
  --name RubricasAppPlan \
  --resource-group RubricasAppRG \
  --sku B1 \
  --is-linux

# Crear Web App
az webapp create \
  --resource-group RubricasAppRG \
  --plan RubricasAppPlan \
  --name rubricasapp \
  --runtime "DOTNETCORE:8.0"
```

### 3.2 Configurar Variables de Entorno

```bash
# Connection String de producción
az webapp config connection-string set \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --settings DefaultConnection="Server=tcp:serverrubricasapp.database.windows.net,1433;Initial Catalog=RubricasDb;Persist Security Info=False;User ID=RubricasUser;Password=TU_CONTRASEÑA_REAL;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  --connection-string-type SQLAzure

# Variables de aplicación
az webapp config appsettings set \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    WEBSITE_TIME_ZONE="Central Standard Time" \
    SCM_DO_BUILD_DURING_DEPLOYMENT="true"
```

---

## Paso 4: Publicar Aplicación

### 4.1 Preparar Publicación

```bash
# Navegar a la carpeta del proyecto
cd D:\Fuentes_gitHub\RubricasApp.Web

# Limpiar y compilar
dotnet clean
dotnet build --configuration Release

# Publicar
dotnet publish --configuration Release --output ./publish
```

### 4.2 Desplegar a Azure

```bash
# Comprimir carpeta publish
Compress-Archive -Path ./publish/* -DestinationPath ./rubricasapp.zip -Force

# Desplegar usando Azure CLI
az webapp deployment source config-zip \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --src ./rubricasapp.zip
```

### 4.3 Alternativa: Desplegar desde Visual Studio

1. **Click derecho** en el proyecto → **Publish**
2. **Target**: Azure
3. **Specific target**: Azure App Service (Linux)
4. **Subscription**: Seleccionar suscripción
5. **App Service**: `rubricasapp`
6. **Publish**

---

## Paso 5: Verificar Despliegue

### 5.1 Verificar Logs

```bash
# Ver logs en tiempo real
az webapp log tail \
  --resource-group RubricasAppRG \
  --name rubricasapp

# Descargar logs
az webapp log download \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --log-file logs.zip
```

### 5.2 Verificar Aplicación

```bash
# Abrir en navegador
az webapp browse \
  --resource-group RubricasAppRG \
  --name rubricasapp
```

**URL de producción**: https://rubricasapp.azurewebsites.net

### 5.3 Verificar Inicialización

La aplicación debe ejecutar automáticamente:
1. ✅ SqlServerDatabaseInitializer (crear tablas)
2. ✅ DatabaseGroupsInitializer (tipos de grupo)
3. ✅ CostaRicaGeographicInitializer (provincias/cantones/distritos)
4. ✅ CrearTablaAuditoria (auditoría)
5. ✅ ConductaSeedData (tipos de falta MEP)
6. ✅ IdentitySeederService (crear admin@rubricas.edu)
7. ✅ PermissionService (168 permisos)
8. ✅ Verificación SuperAdmin

**Credenciales de acceso inicial:**
- Email: `admin@rubricas.edu`
- Password: `Admin@2025!`

---

## Paso 6: Configuración Post-Despliegue

### 6.1 Configurar SSL/HTTPS

```bash
# Azure proporciona HTTPS automáticamente con certificado *.azurewebsites.net
# Para dominio personalizado:
az webapp config hostname add \
  --resource-group RubricasAppRG \
  --webapp-name rubricasapp \
  --hostname www.tudominio.com
```

### 6.2 Configurar Escalado

```bash
# Escalar verticalmente (más CPU/RAM)
az appservice plan update \
  --name RubricasAppPlan \
  --resource-group RubricasAppRG \
  --sku P1V2

# Escalar horizontalmente (más instancias)
az appservice plan update \
  --name RubricasAppPlan \
  --resource-group RubricasAppRG \
  --number-of-workers 2
```

### 6.3 Habilitar Application Insights (Monitoreo)

```bash
az monitor app-insights component create \
  --app rubricasapp-insights \
  --location eastus \
  --resource-group RubricasAppRG \
  --application-type web

# Obtener Instrumentation Key
az monitor app-insights component show \
  --app rubricasapp-insights \
  --resource-group RubricasAppRG \
  --query instrumentationKey

# Configurar en App Service
az webapp config appsettings set \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --settings APPINSIGHTS_INSTRUMENTATIONKEY="TU_INSTRUMENTATION_KEY"
```

---

## Paso 7: Respaldo y Mantenimiento

### 7.1 Backup Automático de Base de Datos

```bash
az sql db ltr-policy set \
  --resource-group RubricasAppRG \
  --server serverrubricasapp \
  --database RubricasDb \
  --weekly-retention P4W \
  --monthly-retention P12M \
  --yearly-retention P5Y \
  --week-of-year 1
```

### 7.2 Backup Manual

```sql
-- Ejecutar en Azure SQL Database
-- No soporta BACKUP TO DISK, usar Export Data-tier Application (.bacpac)
```

### 7.3 Monitoreo

Portal Azure → App Service → Monitoring:
- **Metrics**: CPU, Memory, Response Time
- **Logs**: Application Logs, HTTP Logs
- **Alerts**: Configurar alertas de disponibilidad

---

## Troubleshooting

### Error: "Connection timeout"
```bash
# Verificar firewall de SQL Server
az sql server firewall-rule list \
  --resource-group RubricasAppRG \
  --server serverrubricasapp

# Agregar IP de App Service
az webapp show \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --query outboundIpAddresses
```

### Error: "Unable to open database"
- Verificar connection string en App Service settings
- Verificar credenciales de SQL Server
- Revisar logs: `az webapp log tail`

### Error: "500 Internal Server Error"
```bash
# Habilitar logs detallados
az webapp config set \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --detailed-error-logging-enabled true

# Ver logs
az webapp log tail \
  --resource-group RubricasAppRG \
  --name rubricasapp
```

### Performance lento
- Escalar App Service Plan a tier superior (P1V2, P2V2)
- Revisar índices de base de datos
- Habilitar Application Insights para diagnóstico

---

## Costos Estimados (USD/mes)

| Servicio | Tier | Costo Aprox. |
|----------|------|-------------|
| App Service | B1 (Basic) | $13.14 |
| SQL Database | S0 (10 DTU) | $15.00 |
| Application Insights | Basic | Gratis (5GB) |
| **Total** | | **~$28.14/mes** |

**Producción recomendada:**
- App Service: P1V2 (~$73/mes)
- SQL Database: S1 (20 DTU, ~$30/mes)
- **Total: ~$103/mes**

---

## Comandos Rápidos de Gestión

```bash
# Reiniciar App Service
az webapp restart --resource-group RubricasAppRG --name rubricasapp

# Ver estado
az webapp show --resource-group RubricasAppRG --name rubricasapp --query state

# Detener App Service
az webapp stop --resource-group RubricasAppRG --name rubricasapp

# Iniciar App Service
az webapp start --resource-group RubricasAppRG --name rubricasapp

# Eliminar todo (CUIDADO!)
az group delete --name RubricasAppRG --yes --no-wait
```

---

## Checklist de Despliegue

- [ ] Base de datos Azure SQL creada
- [ ] Firewall SQL Server configurado
- [ ] Connection string actualizado en App Service
- [ ] Aplicación publicada y desplegada
- [ ] Inicialización automática completada (8 pasos)
- [ ] Login con admin@rubricas.edu funcional
- [ ] Slider visible en página pública (pre-login)
- [ ] Módulos principales probados
- [ ] SSL/HTTPS activo
- [ ] Logs de Application Insights configurados
- [ ] Backup automático configurado

---

## Soporte

**Documentación oficial:**
- Azure App Service: https://docs.microsoft.com/azure/app-service/
- Azure SQL Database: https://docs.microsoft.com/azure/azure-sql/
- ASP.NET Core en Azure: https://docs.microsoft.com/aspnet/core/host-and-deploy/azure-apps/

**Logs en tiempo real:**
```bash
az webapp log tail --resource-group RubricasAppRG --name rubricasapp
```
