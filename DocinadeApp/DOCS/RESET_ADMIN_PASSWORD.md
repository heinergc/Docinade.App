# Resetear Contraseña del Administrador

## Descripción
Utilidad para resetear o cambiar la contraseña del usuario administrador del Sistema de Rúbricas MEP.

## Credenciales por Defecto
- **Usuario:** admin@rubricas.edu
- **Contraseña:** Admin@2025!

## Uso

### 1. Verificar usuario admin
```powershell
dotnet run -- reset-password --verify
```

Muestra información del usuario:
- ID, Email, UserName
- Estado (Activo, Bloqueado, Email Confirmado)
- Roles asignados
- Último acceso

### 2. Resetear a contraseña por defecto (Admin@2025!)
```powershell
dotnet run -- reset-password
```

### 3. Cambiar a Admin1234
```powershell
dotnet run -- reset-password Admin1234
```

### 4. Cambiar a contraseña personalizada
```powershell
dotnet run -- reset-password --password=MiNuevaPassword123!
```

O simplemente:
```powershell
dotnet run -- reset-password MiNuevaPassword123!
```

### 5. Desbloquear usuario
Si el usuario está bloqueado por múltiples intentos fallidos:
```powershell
dotnet run -- reset-password --unlock
```

## Ejemplos Comunes

### Problema: "Email o contraseña incorrectos"
```powershell
# Resetear a contraseña por defecto
dotnet run -- reset-password

# Luego usar:
# Usuario: admin@rubricas.edu
# Contraseña: Admin@2025!
```

### Problema: "Cuenta bloqueada temporalmente"
```powershell
# Desbloquear usuario
dotnet run -- reset-password --unlock

# Luego resetear contraseña
dotnet run -- reset-password
```

### Cambiar a contraseña más simple para desarrollo
```powershell
dotnet run -- reset-password Admin1234
```

## Requisitos de Contraseña
- Mínimo 6 caracteres
- Al menos 1 dígito
- Al menos 1 mayúscula
- Sin caracteres especiales requeridos (opcional)

## Ejemplos de Contraseñas Válidas
- `Admin@2025!` (por defecto)
- `Admin1234`
- `Password123`
- `MiClave456`
- `SuperAdmin99`

## Solución de Problemas

### Error: "Usuario admin@rubricas.edu NO encontrado"
**Solución:** El usuario se crea automáticamente al iniciar la aplicación por primera vez.

1. Inicie la aplicación normalmente:
   ```powershell
   dotnet run
   ```

2. Espere a que termine la inicialización (verá logs como "SuperAdmin verificado")

3. Detenga la aplicación (Ctrl+C)

4. Ahora ejecute el reseteo de contraseña:
   ```powershell
   dotnet run -- reset-password
   ```

### Error: "No se pudo cambiar la contraseña: Password is too weak"
**Solución:** Use una contraseña que cumpla los requisitos (mínimo 6 caracteres, 1 dígito, 1 mayúscula).

Ejemplos correctos:
```powershell
dotnet run -- reset-password Admin123
dotnet run -- reset-password Password1
dotnet run -- reset-password MiClave456
```

### La contraseña no funciona después de cambiarla
**Solución:** Verifique que el cambio se aplicó correctamente:

1. Verificar usuario:
   ```powershell
   dotnet run -- reset-password --verify
   ```

2. Si aparece como bloqueado, desbloquear:
   ```powershell
   dotnet run -- reset-password --unlock
   ```

3. Resetear contraseña nuevamente:
   ```powershell
   dotnet run -- reset-password Admin1234
   ```

## Scripts SQL Alternativos

Si prefiere usar SQL directamente, ejecute en SQL Server Management Studio:

### Desbloquear usuario
```sql
UPDATE AspNetUsers
SET LockoutEnd = NULL,
    AccessFailedCount = 0,
    EmailConfirmed = 1,
    Activo = 1
WHERE Email = 'admin@rubricas.edu';
```

### Verificar usuario
```sql
SELECT Email, UserName, EmailConfirmed, Activo, LockoutEnd, AccessFailedCount
FROM AspNetUsers
WHERE Email = 'admin@rubricas.edu';
```

**NOTA:** No es posible cambiar la contraseña con SQL directo porque está hasheada. Debe usar esta herramienta.

## Archivo de Script PowerShell

También puede ejecutar:
```powershell
.\reset-admin-password.ps1
```

Este script proporciona un menú interactivo con todas las opciones.

## Notas Importantes

1. **Ambiente de Producción:** La herramienta detecta automáticamente si está en Production o Development y usa la conexión correspondiente.

2. **Seguridad:** Después de resetear la contraseña, se recomienda cambiarla desde la interfaz web en "Mi Perfil > Cambiar Contraseña".

3. **Base de Datos:** Asegúrese de que el `appsettings.json` tenga configurada correctamente la cadena de conexión:
   - `DefaultConnection` para Development
   - `ProductionConnection` para Production

4. **Usuario No Existe:** Si el usuario no existe, debe iniciar la aplicación web normalmente. El sistema creará automáticamente el usuario admin en el primer inicio.

## Más Ayuda

Para más información sobre el sistema de autenticación, consulte:
- [INSTRUCCIONES_ADMIN_SEEDING.md](INSTRUCCIONES_ADMIN_SEEDING.md)
- [copilot-instructions.md](.github/copilot-instructions.md)
