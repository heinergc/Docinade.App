# Despliegue Rápido a Azure - RubricasApp.Web

## 🚀 Opción 1: Despliegue Completo (Primera vez)

Crea toda la infraestructura en Azure y despliega la aplicación.

### Pre-requisitos
1. **Azure CLI** instalado: https://aka.ms/installazurecliwindows
2. **Cuenta de Azure** activa
3. **Contraseña segura** para SQL Server (mínimo 8 caracteres, mayúsculas, minúsculas, números, símbolos)

### Comando

```powershell
.\deploy-azure.ps1 -SqlPassword "TuContraseñaSegura123!"
```

**Esto creará:**
- ✅ Resource Group: `RubricasAppRG`
- ✅ SQL Server: `serverrubricasapp.database.windows.net`
- ✅ SQL Database: `RubricasDb` (Tier S0)
- ✅ App Service Plan: B1 (Basic)
- ✅ Web App: `rubricasapp.azurewebsites.net`
- ✅ Firewall configurado
- ✅ Connection strings configurados
- ✅ Aplicación desplegada

**Tiempo estimado:** 10-15 minutos

---

## ⚡ Opción 2: Publicación Rápida (Actualizaciones)

Cuando la infraestructura ya existe y solo quieres actualizar el código.

### Comando

```powershell
.\publish-azure.ps1
```

**Esto hará:**
- ✅ Compilar en modo Release
- ✅ Generar paquete de publicación
- ✅ Desplegar a Azure
- ✅ Limpiar archivos temporales

**Tiempo estimado:** 2-3 minutos

---

## 📋 Parámetros Personalizables

### deploy-azure.ps1

```powershell
.\deploy-azure.ps1 `
    -ResourceGroup "MiGrupo" `
    -Location "eastus" `
    -AppName "miapp" `
    -SqlServer "miservidor" `
    -SqlDatabase "mibd" `
    -SqlUser "admin" `
    -SqlPassword "MiPassword123!"
```

### publish-azure.ps1

```powershell
.\publish-azure.ps1 `
    -ResourceGroup "MiGrupo" `
    -AppName "miapp"
```

---

## 🔐 Credenciales de Acceso Inicial

Una vez desplegada la aplicación:

**URL:** https://rubricasapp.azurewebsites.net

**Usuario administrador:**
- Email: `admin@rubricas.edu`
- Password: `Admin@2025!`

⚠️ **IMPORTANTE:** Cambiar la contraseña inmediatamente después del primer login.

---

## 📊 Verificar Despliegue

### Ver logs en tiempo real

```powershell
az webapp log tail --resource-group RubricasAppRG --name rubricasapp
```

### Abrir aplicación en navegador

```powershell
az webapp browse --resource-group RubricasAppRG --name rubricasapp
```

### Ver estado del App Service

```powershell
az webapp show --resource-group RubricasAppRG --name rubricasapp --query state
```

### Reiniciar aplicación

```powershell
az webapp restart --resource-group RubricasAppRG --name rubricasapp
```

---

## 🔧 Configuración Post-Despliegue

### 1. Actualizar Contraseña de SQL en App Service

Si necesitas cambiar la contraseña de SQL Server:

```powershell
$NewConnectionString = "Server=tcp:serverrubricasapp.database.windows.net,1433;Initial Catalog=RubricasDb;User ID=RubricasUser;Password=NUEVA_CONTRASEÑA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

az webapp config connection-string set `
    --resource-group RubricasAppRG `
    --name rubricasapp `
    --settings DefaultConnection=$NewConnectionString `
    --connection-string-type SQLAzure
```

### 2. Habilitar HTTPS Only

```powershell
az webapp update `
    --resource-group RubricasAppRG `
    --name rubricasapp `
    --https-only true
```

### 3. Configurar Dominio Personalizado

```powershell
az webapp config hostname add `
    --resource-group RubricasAppRG `
    --webapp-name rubricasapp `
    --hostname www.tudominio.com
```

---

## 💰 Costos Mensuales Estimados

| Componente | Tier | Costo USD/mes |
|------------|------|---------------|
| App Service | B1 (Basic) | ~$13 |
| SQL Database | S0 (10 DTU) | ~$15 |
| **Total** | | **~$28** |

### Escalar para Producción

```powershell
# Escalar App Service a P1V2 (mejor rendimiento)
az appservice plan update `
    --name rubricasapp-plan `
    --resource-group RubricasAppRG `
    --sku P1V2

# Escalar SQL Database a S1 (20 DTU)
az sql db update `
    --resource-group RubricasAppRG `
    --server serverrubricasapp `
    --name RubricasDb `
    --service-objective S1
```

**Costo con escalado:** ~$103/mes

---

## 🐛 Troubleshooting

### Error: "Connection timeout"

Agregar IP del App Service al firewall de SQL Server:

```powershell
# Obtener IPs del App Service
az webapp show --resource-group RubricasAppRG --name rubricasapp --query outboundIpAddresses

# Agregar cada IP al firewall
az sql server firewall-rule create `
    --resource-group RubricasAppRG `
    --server serverrubricasapp `
    --name AllowAppService `
    --start-ip-address X.X.X.X `
    --end-ip-address X.X.X.X
```

### Error: "500 Internal Server Error"

Ver logs detallados:

```powershell
# Habilitar logs
az webapp config set `
    --resource-group RubricasAppRG `
    --name rubricasapp `
    --detailed-error-logging-enabled true

# Ver logs
az webapp log tail --resource-group RubricasAppRG --name rubricasapp
```

### La aplicación no arranca

```powershell
# Verificar estado
az webapp show --resource-group RubricasAppRG --name rubricasapp --query state

# Reiniciar
az webapp restart --resource-group RubricasAppRG --name rubricasapp

# Ver logs de inicio
az webapp log tail --resource-group RubricasAppRG --name rubricasapp
```

---

## 🗑️ Eliminar Todo (CUIDADO)

Para eliminar completamente todos los recursos de Azure:

```powershell
az group delete --name RubricasAppRG --yes --no-wait
```

⚠️ **ADVERTENCIA:** Esto eliminará PERMANENTEMENTE:
- App Service
- SQL Server y todas sus bases de datos
- Todos los datos almacenados
- Configuraciones

---

## 📚 Documentación Completa

Para instrucciones detalladas, ver: **[DEPLOY_AZURE.md](DEPLOY_AZURE.md)**

---

## ✅ Checklist de Despliegue

- [ ] Azure CLI instalado
- [ ] Login en Azure completado
- [ ] Script `deploy-azure.ps1` ejecutado exitosamente
- [ ] SQL Server creado y accesible
- [ ] Aplicación desplegada en App Service
- [ ] URL funcional: https://rubricasapp.azurewebsites.net
- [ ] Login con admin@rubricas.edu exitoso
- [ ] Slider visible en página pública
- [ ] HTTPS habilitado
- [ ] Contraseña de admin cambiada
- [ ] Backup configurado (opcional)

---

## 🆘 Soporte

**Logs en tiempo real:**
```powershell
az webapp log tail --resource-group RubricasAppRG --name rubricasapp
```

**Documentación Azure:**
- App Service: https://docs.microsoft.com/azure/app-service/
- SQL Database: https://docs.microsoft.com/azure/azure-sql/
