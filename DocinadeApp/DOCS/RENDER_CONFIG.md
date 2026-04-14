# Configuración de Variables de Entorno en Render.com

## ⚠️ VARIABLES REQUERIDAS (OBLIGATORIAS):

### 1. ASPNETCORE_ENVIRONMENT
```
Clave: ASPNETCORE_ENVIRONMENT
Valor: Production
```

### 2. ConnectionStrings__DefaultConnection
```
Clave: ConnectionStrings__DefaultConnection
Valor: Server=RubricaDB.mssql.somee.com;Database=RubricaDB;User ID=lksa2014_SQLLogin_1;Password=46isuufjtc;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=true;
```

**NOTA**: El doble guión bajo `__` reemplaza los `:` en la jerarquía JSON de configuración.

### 3. PORT (Render lo asigna automáticamente)
⚠️ **NO configures PORT manualmente**. Render lo asigna dinámicamente.

---

## 📋 PASOS PARA CONFIGURAR EN RENDER.COM:

1. **Accede a tu servicio en Render**:
   - Ve a https://dashboard.render.com/
   - Selecciona tu servicio: `rubricasapp-web`

2. **Configura las variables de entorno**:
   - Click en la pestaña **Environment** (lado izquierdo)
   - Click en **Add Environment Variable**
   
3. **Agrega cada variable**:
   ```
   Variable 1:
   Key: ASPNETCORE_ENVIRONMENT
   Value: Production
   
   Variable 2:
   Key: ConnectionStrings__DefaultConnection
   Value: Server=RubricaDB.mssql.somee.com;Database=RubricaDB;User ID=lksa2014_SQLLogin_1;Password=46isuufjtc;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=true;
   ```

4. **Guardar y redesplegar**:
   - Click en **Save Changes**
   - Render automáticamente redespliegará el servicio (3-5 minutos)

---

## 🔍 VERIFICAR LOGS DESPUÉS DEL DESPLIEGUE:

1. En Render Dashboard → Tu servicio → **Logs** (pestaña superior)

2. **Busca estos mensajes de éxito**:
   ```
   ✅ "🏢 Configurado para SQL Server: Server=RubricaDB.mssql.somee.com..."
   ✅ "🎉 Sistema completamente inicializado en SQL Server"
   ✅ "Now listening on: http://0.0.0.0:10000"
   ✅ "Application started. Press Ctrl+C to shut down."
   ```

3. **Errores comunes a buscar**:
   ```
   ❌ "ConnectionString no configurado" → Falta variable ConnectionStrings__DefaultConnection
   ❌ "A network-related or instance-specific error" → Somee.com no permite conexión desde Render
   ❌ "Login failed for user" → Credenciales incorrectas
   ❌ "Invalid object name" → Tablas no existen (ejecutar scripts SQL)
   ```

---

## 🚨 SOLUCIÓN SI ERROR 500 PERSISTE:

### Problema 1: Somee.com bloquea conexiones desde Render
**Síntoma**: Logs muestran "A network-related or instance-specific error"

**Soluciones**:
1. Verifica en Somee.com que la base de datos esté **activa** (no suspendida)
2. Confirma que Somee.com permite conexiones remotas
3. Considera migrar a base de datos con mejor conectividad (Azure SQL, AWS RDS, Railway Postgres)

### Problema 2: Tablas de Identity no existen
**Síntoma**: Logs muestran "Invalid object name 'AspNetUsers'"

**Solución**:
1. Conecta a Somee.com con SQL Server Management Studio
2. Ejecuta: `recrear_admin.sql` (crea usuario admin)
3. Ejecuta: `DATOS_INICIALES_PRODUCCION.sql` (crea catálogos)

### Problema 3: Variables de entorno no se aplican
**Síntoma**: Logs muestran "❌ ERROR: No se encontró ConnectionStrings:DefaultConnection"

**Solución**:
1. Verifica que escribiste **EXACTAMENTE**: `ConnectionStrings__DefaultConnection` (doble guión bajo)
2. Guarda los cambios y espera el redespliegue completo
3. Si persiste, elimina y vuelve a crear la variable

---

## 🧪 TESTING DESPUÉS DE DESPLIEGUE:

1. **Accede a la URL**: https://rubricasapp-web.onrender.com/

2. **Verifica el endpoint de login**: https://rubricasapp-web.onrender.com/Account/Login

3. **Prueba login con**:
   ```
   Email: admin@rubricas.edu
   Password: Admin@2025
   ```
   (Si ejecutaste `recrear_admin.sql`)

---

## 📞 SOPORTE ADICIONAL:

Si el problema persiste después de estos pasos:

1. **Revisa logs completos** en Render y copia el error específico
2. **Verifica conectividad** a Somee.com desde otro servidor externo
3. **Considera alternativas** de base de datos (Railway, Supabase, Azure SQL)
```
