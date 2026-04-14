# INSTRUCCIONES: Agregar Seeding de Admin en Program.cs

## Paso 1: Abrir Program.cs

Abre el archivo `Program.cs` en tu editor.

## Paso 2: Buscar la línea

Busca la línea **357** que contiene:

```csharp
logger.LogInformation("✅ Permissions and roles initialized successfully.");
```

## Paso 3: Agregar el código

Justo **DESPUÉS** de esa línea (línea 357), agrega estas líneas:

```csharp
    
    // 👤 Crear usuario administrador por defecto
    await RubricasApp.Web.Utils.AdminSeeder.SeedDefaultAdminAsync(
        services.GetRequiredService<UserManager<ApplicationUser>>(),
        services.GetRequiredService<RoleManager<IdentityRole>>(),
        logger);
```

## Paso 4: Resultado Final

El código debería verse así (líneas 357-365):

```csharp
    logger.LogInformation("✅ Permissions and roles initialized successfully.");
    
    // 👤 Crear usuario administrador por defecto
    await RubricasApp.Web.Utils.AdminSeeder.SeedDefaultAdminAsync(
        services.GetRequiredService<UserManager<ApplicationUser>>(),
        services.GetRequiredService<RoleManager<IdentityRole>>(),
        logger);
    
    logger.LogInformation("🎉 Sistema completamente inicializado en SQL Server");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error initializing database: {ex.Message}");
    Console.WriteLine("Application will continue but database may not be available.");
}
```

## Paso 5: Guardar y Ejecutar

1. Guarda el archivo `Program.cs`
2. Ejecuta la aplicación con `dotnet run`
3. Verás en los logs: `✅ Usuario administrador creado: admin@rubricas.com con contraseña: Admin123!`

## Credenciales

- **Email**: admin@rubricas.com
- **Contraseña**: Admin123!

¡Listo! El usuario administrador se creará automáticamente al iniciar la aplicación.
