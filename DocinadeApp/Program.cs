using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;
using DocinadeApp.Models.Identity;
using DocinadeApp.Models.Permissions;
using DocinadeApp.Services;
using DocinadeApp.Services.Identity;
using DocinadeApp.Services.Permissions;
using DocinadeApp.Services.Audit;
using DocinadeApp.Services.CuadernoCalificador;
using DocinadeApp.Services.Calificador;
using DocinadeApp.Services.Grupos;
using DocinadeApp.Scripts;
using DocinadeApp.Configuration;
using DocinadeApp.Middleware;
using DocinadeApp.Authorization;
using DocinadeApp.Utils;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using System.Text;
using ApplicationRoles = DocinadeApp.Models.Permissions.ApplicationRoles;

// Configurar EPPlus
EPPlusConfig.ConfigureLicense();

// Configurar encoding UTF-8 globalmente
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// MODO ESPECIAL: Resetear contraseña de administrador
if (args.Length > 0 && args[0] == "reset-password")
{
    await ResetAdminPasswordMode(args);
    return;
}

try
{
    var builder = WebApplication.CreateBuilder(args);

// Configurar puerto según ambiente
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    // En Render.com o producción con variable PORT
    Console.WriteLine($"[INFO] Usando puerto de entorno: {port}");
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}
else if (args.Length == 0)
{
    // Sin argumentos, usar configuración por defecto
    Console.WriteLine("[INFO] Usando configuración de launchSettings.json");
}
// Si hay argumentos --urls, .NET los manejará automáticamente

// Configurar Kestrel para UTF-8
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

// Configurar encoding para respuestas HTTP
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Configurar para usar UTF-8 en todas las respuestas
    options.RespectBrowserAcceptHeader = false;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

// Add HttpContextAccessor for services that need access to HTTP context
builder.Services.AddHttpContextAccessor();

// Configure session for period academic management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Add Entity Framework - SQL Server únicamente (sin compatibilidad con SQLite)
builder.Services.AddDbContext<RubricasDbContext>(options =>
{
    var environmentName = builder.Environment.EnvironmentName;
    var isProduction = builder.Environment.IsProduction();
    var connectionStringName = isProduction ? "ProductionConnection" : "DefaultConnection";
    
    Console.WriteLine($"[INFO] Ambiente detectado: {environmentName}");
    Console.WriteLine($"[INFO] IsProduction: {isProduction}");
    Console.WriteLine($"[INFO] Buscando ConnectionString: {connectionStringName}");
    
    var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
    
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("[ERROR] No se encontró ConnectionString para el ambiente actual");
        Console.WriteLine($"   Ambiente: {environmentName}");
        Console.WriteLine($"   ConnectionString buscado: {connectionStringName}");
        Console.WriteLine("   Verifica las variables de entorno en Render.com");
        throw new InvalidOperationException($"ConnectionString '{connectionStringName}' no configurado. Configura 'ConnectionStrings__{connectionStringName}' en las variables de entorno.");
    }
    
    // Configuración exclusiva para SQL Server
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(60);
    });
    
    // Ocultar contraseña en logs
    var safeConnectionString = connectionString.Contains("Password=") 
        ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***;..." 
        : connectionString;
    Console.WriteLine($"[INFO] Configurado para SQL Server: {safeConnectionString}");
    
    // Configuración adicional para desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedEmail = false; // Cambiar a true en producción
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<RubricasDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<SpanishIdentityErrorDescriber>();

// Configure application cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    
    // Evitar redirección automática para rutas públicas
    options.Events.OnRedirectToLogin = context =>
    {
        // Permitir acceso anónimo a Home y Account
        var publicPaths = new[] { "/", "/Home", "/Account/Register", "/Account/Login" };
        var isPublicPath = publicPaths.Any(path => 
            context.Request.Path.StartsWithSegments(path) || 
            context.Request.Path.Value?.Equals(path, StringComparison.OrdinalIgnoreCase) == true);
            
        if (isPublicPath)
        {
            // No redirigir, devolver 401 pero continuar procesando
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
        
        // Para rutas protegidas, hacer la redirección normal
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Add custom services
builder.Services.AddScoped<IEstudianteImportService, EstudianteImportService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IdentitySeederService>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();

// Add HttpClient for external APIs
builder.Services.AddHttpClient();

// Add Cedula Costa Rica service
builder.Services.AddHttpClient<ICedulaCostaRicaService, CedulaCostaRicaService>();

// Add permission and audit services
builder.Services.AddScoped<IAuditService, AuditService>();

// Add AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<DocinadeApp.Mapping.AcademicMappingProfile>());

// Add academic module services
builder.Services.AddScoped<DocinadeApp.Services.Academic.IPeriodosAcademicosService, DocinadeApp.Services.Academic.PeriodosAcademicosService>();
builder.Services.AddScoped<DocinadeApp.Services.Academic.IMateriasService, DocinadeApp.Services.Academic.MateriasService>();
builder.Services.AddScoped<DocinadeApp.Services.Academic.IPeriodoAcademicoRepository, DocinadeApp.Services.Academic.PeriodoAcademicoRepositoryImpl>();
builder.Services.AddScoped<DocinadeApp.Services.Academic.IMateriaRepository, DocinadeApp.Services.Academic.MateriaRepositoryImpl>();
builder.Services.AddScoped<DocinadeApp.Services.Academic.IRubricaRepository, DocinadeApp.Services.Academic.RubricaRepositoryImpl>();
builder.Services.AddScoped<DocinadeApp.Services.Academic.IMateriaPeriodoRepository, DocinadeApp.Services.Academic.MateriaPeriodoRepositoryImpl>();
builder.Services.AddScoped<DocinadeApp.Services.Academic.IInstrumentoRepository, DocinadeApp.Services.Academic.InstrumentoRepositoryImpl>();

// Add global period academic filter services
builder.Services.AddScoped<IPeriodoAcademicoService, PeriodoAcademicoService>();

// Add Cuaderno Calificador services
builder.Services.AddScoped<ICuadernoCalificadorService, CuadernoCalificadorService>();
builder.Services.AddScoped<ICuadernoCalificadorDinamicoService, CuadernoCalificadorDinamicoService>();

// Add SEA (Sistema Evaluacion MEP) services
builder.Services.AddScoped<DocinadeApp.Services.SEA.ISEAService, DocinadeApp.Services.SEA.SEAService>();

// Add Reportes services
builder.Services.AddScoped<DocinadeApp.Services.Reportes.IReporteService, DocinadeApp.Services.Reportes.ReporteService>();

// Add Calificador PQ2025 services (no intrusivo)
builder.Services.AddScoped<ICalificadorService, CalificadorService>();

// Add Grupos services
builder.Services.AddScoped<IGrupoEstudianteService, GrupoEstudianteService>();

// 🆕 MÓDULO ACS - Add Adecuación Curricular Significativa services
builder.Services.AddScoped<DocinadeApp.Services.AdecuacionCurricular.IACSService, DocinadeApp.Services.AdecuacionCurricular.ACSService>();

// 🔧 NUEVA: Add Auditoria services
builder.Services.AddScoped<DocinadeApp.Services.Auditoria.IAuditoriaService, DocinadeApp.Services.Auditoria.AuditoriaService>();

// 📄 Add PDF generation services using Rotativa
builder.Services.AddScoped<DocinadeApp.Services.IPdfService, DocinadeApp.Services.PdfService>();

// NUEVA: Add User Context services for access control
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

// MÓDULO DE EXCEPCIONES - Add Exception tracking service
builder.Services.AddScoped<IExcepcionService, ExcepcionService>();
builder.Services.AddScoped<IErrorLogService, ErrorLogService>();

// MÓDULO DE CONDUCTA - Add Conducta services
builder.Services.AddScoped<IConductaService, ConductaService>();

// MÓDULO DE ASISTENCIA MEP - Add Horario and AsistenciaLeccion services
builder.Services.AddScoped<IHorarioService, HorarioService>();
builder.Services.AddScoped<IAsistenciaLeccionService, AsistenciaLeccionService>();

// MÓDULO DE SLIDER DINÁMICO
builder.Services.AddScoped<ISliderService, SliderService>();

// Add custom authorization using extension method
builder.Services.AddCustomAuthorization();

// Add Razor runtime compilation for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
}

var app = builder.Build();

// Initialize database and Identity
IServiceScope? scope = null;
try
{
    scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<RubricasDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("🏢 Iniciando inicialización de SQL Server...");
    
    // Inicializar base de datos SQL Server desde cero
    try
    {
        var dbInitialized = await SqlServerDatabaseInitializer.InitializeDatabaseAsync(services);
        if (!dbInitialized)
        {
            logger.LogWarning("⚠️ La inicialización de la base de datos reportó falso, pero continuaremos");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error en la inicialización de la base de datos, pero continuaremos");
    }

    // 🔧 Corregir columnas Observaciones faltantes
    // await DocinadeApp.Utils.DatabaseFixer.FixMissingObservacionesColumns(context);

    // Las tablas de grupos ya están creadas, solo verificamos
    logger.LogInformation("[SUCCESS] Tablas del sistema de grupos verificadas");

    // Inicializar tablas de grupos si no existen
    try
    {
        var groupsInitialized = await DocinadeApp.Utils.DatabaseGroupsInitializer.CreateGroupsTablesAsync(context);
        if (groupsInitialized)
        {
            logger.LogInformation("[SUCCESS] Tablas del sistema de grupos inicializadas correctamente");
            // Inicializar catálogo de tipos de grupo
            var tiposGrupoInitialized = await DocinadeApp.Utils.TiposGrupoInitializer.InitializeCompleteAsync(context);
            if (tiposGrupoInitialized)
            {
                logger.LogInformation("[SUCCESS] Catálogo de tipos de grupo inicializado correctamente");
            }
            else
            {
                logger.LogWarning("[WARNING] No se pudo inicializar el catálogo de tipos de grupo");
            }
            // Insertar datos de ejemplo si es necesario
            try
            {
                await DocinadeApp.Utils.DatabaseGroupsInitializer.InsertSampleDataAsync(context);
                logger.LogInformation("[SUCCESS] Datos de ejemplo de grupos insertados");
            }
            catch (Exception exSample)
            {
                logger.LogWarning(exSample, "[WARNING] No se pudieron insertar datos de ejemplo de grupos, pero el sistema continuará");
            }
        }
        else
        {
            logger.LogWarning("[WARNING] No se pudieron crear las tablas de grupos, pero el sistema continuará");
        }
    }
    catch (Exception exGroups)
    {
        logger.LogError(exGroups, "[ERROR] Error inicializando grupos, pero el sistema continuará");
    }

    // 🔧 NUEVA: Inicializar datos geográficos de Costa Rica
    logger.LogInformation("[INFO] Inicializando datos geográficos de Costa Rica...");
    try
    {
        var geographicInitialized = await DocinadeApp.Utils.CostaRicaGeographicInitializer.InitializeAsync(context);
        if (geographicInitialized)
        {
            logger.LogInformation("[SUCCESS] Datos geográficos inicializados correctamente");
        }
        else
        {
            logger.LogWarning("[WARNING] No se pudieron inicializar los datos geográficos");
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "[WARNING] No se pudieron inicializar los datos geográficos, pero el sistema continuará");
    }

    // 🔧 NUEVA: Inicializar tabla de auditoría
    logger.LogInformation("[INFO] Inicializando tabla de auditoría...");
    try
    {
        var auditoriaScript = new CrearTablaAuditoria(context);
        await auditoriaScript.EjecutarAsync();
        logger.LogInformation("[SUCCESS] Tabla de auditoría inicializada correctamente");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "[WARNING] No se pudo inicializar la tabla de auditoría, pero el sistema continuará");
    }

    // 📋 MÓDULO DE CONDUCTA: Inicializar tipos de falta y parámetros
    logger.LogInformation("[INFO] Inicializando módulo de conducta (REA 40862-V21)...");
    try
    {
        var conductaInitialized = await DocinadeApp.Utils.ConductaSeedData.InitializeAsync(context);
        if (conductaInitialized)
        {
            var stats = await DocinadeApp.Utils.ConductaSeedData.GetEstadisticasAsync(context);
            logger.LogInformation("[SUCCESS] Módulo de conducta inicializado: {Total} tipos de falta ({Activos} activos, {Inactivos} inactivos)", 
                stats.TotalTipos, stats.Activos, stats.Inactivos);
            
            // Verificar integridad
            await DocinadeApp.Utils.ConductaSeedData.VerificarIntegridadAsync(context);
        }
        else
        {
            logger.LogWarning("[WARNING] No se pudo inicializar el módulo de conducta");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[ERROR] Error inicializando módulo de conducta");
    }

    // Seed Identity data (roles and default users)
    logger.LogInformation("[INFO] Inicializando usuarios y roles del sistema...");
    try
    {
        var identitySeeder = services.GetRequiredService<IdentitySeederService>();
        await identitySeeder.SeedAsync();
        logger.LogInformation("[SUCCESS] Usuarios de Identity creados/verificados");
        
        // Verificar que el admin esté correctamente creado
        var hasAdmin = await identitySeeder.HasAdminUserAsync();
        if (hasAdmin)
        {
            logger.LogInformation("[SUCCESS] SuperAdmin verificado - Sistema listo para usar");
            logger.LogInformation("[INFO] Login con: admin@rubricas.edu / Admin@2025!");
        }
        else
        {
            logger.LogWarning("[WARNING] No se pudo verificar el usuario SuperAdmin");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[ERROR] Error en el seeding de Identity, pero continuaremos");
    }
    
    // Initialize permissions and roles using the service
    logger.LogInformation("[INFO] Inicializando permisos del sistema...");
    try
    {
        var permissionService = services.GetRequiredService<IPermissionService>();
        await permissionService.InitializeDefaultRolesAndPermissionsAsync();
        logger.LogInformation("[SUCCESS] Permisos y roles del sistema configurados");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[ERROR] Error inicializando permisos, pero continuaremos");
    }

    // Sembrar configuración SMTP inicial desde appsettings si no está en BD
    logger.LogInformation("[INFO] Verificando configuración SMTP en base de datos...");
    try
    {
        var configuracionService = services.GetRequiredService<DocinadeApp.Services.IConfiguracionService>();
        var config = services.GetRequiredService<IConfiguration>();

        var smtpDefaults = new[]
        {
            (DocinadeApp.Models.ConfiguracionClaves.EmailSmtpServer,   config["Email:SmtpServer"] ?? string.Empty,    "Servidor SMTP"),
            (DocinadeApp.Models.ConfiguracionClaves.EmailSmtpPort,     config["Email:SmtpPort"] ?? "587",             "Puerto SMTP"),
            (DocinadeApp.Models.ConfiguracionClaves.EmailEnableSsl,    "true",                                         "Habilitar SSL"),
            (DocinadeApp.Models.ConfiguracionClaves.EmailSmtpUsername, config["Email:From"] ?? string.Empty,          "Usuario SMTP"),
            (DocinadeApp.Models.ConfiguracionClaves.EmailSmtpPassword, config["Email:Password"] ?? string.Empty,      "Contraseña SMTP"),
            (DocinadeApp.Models.ConfiguracionClaves.EmailFromEmail,    config["Email:From"] ?? string.Empty,          "Email remitente"),
            (DocinadeApp.Models.ConfiguracionClaves.EmailFromName,     config["Email:DisplayName"] ?? "Sistema de Rúbricas", "Nombre remitente"),
            (DocinadeApp.Models.ConfiguracionClaves.EmailHabilitado,   "true",                                         "Habilitar envío de emails"),
        };

        foreach (var (clave, valor, descripcion) in smtpDefaults)
        {
            if (!await configuracionService.ExisteConfiguracionAsync(clave))
                await configuracionService.EstablecerConfiguracionAsync(clave, valor, descripcion, "Sistema");
        }

        logger.LogInformation("[SUCCESS] Configuración SMTP verificada en base de datos");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[ERROR] Error sembrando configuración SMTP, pero continuaremos");
    }
    
    logger.LogInformation("🎉 Sistema completamente inicializado en SQL Server");
    logger.LogInformation("[SUCCESS] Sistema completamente inicializado en SQL Server");
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR FATAL] Error initializing database: {ex.Message}");
    Console.WriteLine($"[ERROR FATAL] Stack Trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"[ERROR FATAL] Inner Exception: {ex.InnerException.Message}");
        Console.WriteLine($"[ERROR FATAL] Inner Stack Trace: {ex.InnerException.StackTrace}");
    }
    Console.WriteLine("[WARNING] Application will continue but database may not be available.");
}
finally
{
    // Dispose scope with error handling
    if (scope != null)
    {
        try
        {
            scope.Dispose();
            Console.WriteLine("[INFO] Service scope disposed successfully");
        }
        catch (Exception disposeEx)
        {
            Console.WriteLine($"[ERROR] Error disposing service scope: {disposeEx.Message}");
            Console.WriteLine($"[ERROR] Dispose Stack Trace: {disposeEx.StackTrace}");
            if (disposeEx.InnerException != null)
            {
                Console.WriteLine($"[ERROR] Dispose Inner Exception: {disposeEx.InnerException.Message}");
            }
        }
    }
}

Console.WriteLine("[INFO] Starting HTTP pipeline configuration...");

// Configure the HTTP request pipeline.
try
{
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Agregar middleware de UTF-8 al principio del pipeline
app.UseUtf8Encoding();

// Configurar PathBase para IIS (cuando se publica en subcarpeta)
var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
    app.Logger.LogInformation("[INFO] PathBase configurado: {PathBase}", pathBase);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add session middleware for period academic management
app.UseSession();

app.UseRouting();

// Add custom middleware for access logging
app.UseRegistroAcceso();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Add global period academic filter middleware
app.UsePeriodoAcademicoMiddleware();

Console.WriteLine("[INFO] Configuring Rotativa...");
// Configurar Rotativa para generación de PDFs
var rotativaPath = System.IO.Path.Combine(app.Environment.WebRootPath, "Rotativa");
if (!System.IO.Directory.Exists(rotativaPath))
{
    System.IO.Directory.CreateDirectory(rotativaPath);
    app.Logger.LogWarning("[WARNING] Directorio Rotativa creado. Debe descargar wkhtmltopdf desde: https://wkhtmltopdf.org/downloads.html");
}
Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

Console.WriteLine("[INFO] Mapping controller routes...");
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    Console.WriteLine("[INFO] Starting Kestrel web server (app.RunAsync())...");
    Console.WriteLine("[INFO] If you see this message, the app should be listening now.");
    await app.RunAsync();
    Console.WriteLine("[INFO] Application stopped gracefully");
}
catch (Exception pipelineEx)
{
    Console.WriteLine($"[FATAL ERROR] Error configuring HTTP pipeline or starting application:");
    Console.WriteLine($"Message: {pipelineEx.Message}");
    Console.WriteLine($"Type: {pipelineEx.GetType().FullName}");
    Console.WriteLine($"Stack Trace: {pipelineEx.StackTrace}");
    if (pipelineEx.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {pipelineEx.InnerException.Message}");
        Console.WriteLine($"Inner Type: {pipelineEx.InnerException.GetType().FullName}");
    }
    Environment.ExitCode = 1;
}
}
catch (Exception globalEx)
{
    Console.WriteLine($"[FATAL ERROR] Unhandled exception in Program.cs:");
    Console.WriteLine($"Message: {globalEx.Message}");
    Console.WriteLine($"Type: {globalEx.GetType().FullName}");
    Console.WriteLine($"Stack Trace: {globalEx.StackTrace}");
    if (globalEx.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {globalEx.InnerException.Message}");
        Console.WriteLine($"Inner Type: {globalEx.InnerException.GetType().FullName}");
        Console.WriteLine($"Inner Stack Trace: {globalEx.InnerException.StackTrace}");
    }
    Environment.ExitCode = 1;
    throw;
}

// ============================================================================
// FUNCIONES AUXILIARES: RESETEAR CONTRASEÑA DE ADMINISTRADOR
// ============================================================================

static async Task ResetAdminPasswordMode(string[] args)
{
    Console.WriteLine("==============================================");
    Console.WriteLine("  RESETEAR CONTRASEÑA DE ADMINISTRADOR");
    Console.WriteLine("  Sistema de Rúbricas MEP");
    Console.WriteLine("==============================================");
    Console.WriteLine();

    try
    {
        // Construir servicios mínimos
        var builder = WebApplication.CreateBuilder(args);
        
        // Configurar conexión a base de datos
        var isProduction = builder.Environment.IsProduction();
        var connectionStringName = isProduction ? "ProductionConnection" : "DefaultConnection";
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
        
        Console.WriteLine($"[INFO] Ambiente: {(isProduction ? "Production" : "Development")}");
        Console.WriteLine($"[INFO] Conectando a base de datos...");

        builder.Services.AddDbContext<RubricasDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<RubricasDbContext>()
        .AddDefaultTokenProviders();

        var app = builder.Build();

        // Crear scope para servicios
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Determinar acción
            string nuevaPassword = "Admin@2025!"; // Default

            if (args.Length > 1)
            {
                if (args[1] == "Admin1234")
                {
                    nuevaPassword = "Admin1234";
                }
                else if (args[1].StartsWith("--password="))
                {
                    nuevaPassword = args[1].Substring("--password=".Length);
                }
                else if (args[1] == "--verify")
                {
                    await VerificarUsuarioAdmin(userManager);
                    return;
                }
                else if (args[1] == "--unlock")
                {
                    await DesbloquearUsuarioAdmin(userManager);
                    return;
                }
                else
                {
                    nuevaPassword = args[1];
                }
            }

            await ResetearPasswordAdmin(userManager, nuevaPassword);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"[ERROR] {ex.Message}");
        Console.WriteLine($"[STACK] {ex.StackTrace}");
        Environment.ExitCode = 1;
    }
}

static async Task VerificarUsuarioAdmin(UserManager<ApplicationUser> userManager)
{
    Console.WriteLine("[INFO] Verificando usuario admin@rubricas.edu...");
    Console.WriteLine();

    var user = await userManager.FindByEmailAsync("admin@rubricas.edu");
    if (user == null)
    {
        Console.WriteLine("[ERROR] Usuario admin@rubricas.edu NO encontrado");
        Console.WriteLine("[INFO] El usuario debe ser creado automáticamente al iniciar la aplicación");
        return;
    }

    Console.WriteLine("[SUCCESS] Usuario encontrado:");
    Console.WriteLine($"  ID: {user.Id}");
    Console.WriteLine($"  Email: {user.Email}");
    Console.WriteLine($"  UserName: {user.UserName}");
    Console.WriteLine($"  Nombre: {user.Nombre} {user.Apellidos}");
    Console.WriteLine($"  Email Confirmado: {user.EmailConfirmed}");
    Console.WriteLine($"  Activo: {user.Activo}");
    Console.WriteLine($"  Bloqueado hasta: {user.LockoutEnd?.ToString() ?? "No bloqueado"}");
    Console.WriteLine($"  Intentos fallidos: {user.AccessFailedCount}");

    var roles = await userManager.GetRolesAsync(user);
    Console.WriteLine($"  Roles: {string.Join(", ", roles)}");
}

static async Task ResetearPasswordAdmin(UserManager<ApplicationUser> userManager, string nuevaPassword)
{
    Console.WriteLine($"[INFO] Reseteando contraseña a: {nuevaPassword}");
    Console.WriteLine();

    var user = await userManager.FindByEmailAsync("admin@rubricas.edu");
    if (user == null)
    {
        Console.WriteLine("[ERROR] Usuario admin@rubricas.edu NO encontrado");
        Console.WriteLine("[INFO] Inicie la aplicación normalmente primero para crear el usuario");
        return;
    }

    // Remover contraseña actual y agregar nueva
    await userManager.RemovePasswordAsync(user);
    var result = await userManager.AddPasswordAsync(user, nuevaPassword);

    if (result.Succeeded)
    {
        Console.WriteLine("[SUCCESS] Contraseña cambiada exitosamente");
        Console.WriteLine();
        Console.WriteLine("Credenciales de acceso:");
        Console.WriteLine($"  Usuario: admin@rubricas.edu");
        Console.WriteLine($"  Contraseña: {nuevaPassword}");
    }
    else
    {
        Console.WriteLine("[ERROR] No se pudo cambiar la contraseña:");
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"  - {error.Description}");
        }
    }
}

static async Task DesbloquearUsuarioAdmin(UserManager<ApplicationUser> userManager)
{
    Console.WriteLine("[INFO] Desbloqueando usuario admin@rubricas.edu...");
    Console.WriteLine();

    var user = await userManager.FindByEmailAsync("admin@rubricas.edu");
    if (user == null)
    {
        Console.WriteLine("[ERROR] Usuario admin@rubricas.edu NO encontrado");
        return;
    }

    await userManager.SetLockoutEndDateAsync(user, null);
    await userManager.ResetAccessFailedCountAsync(user);
    
    user.Activo = true;
    user.EmailConfirmed = true;
    await userManager.UpdateAsync(user);

    Console.WriteLine("[SUCCESS] Usuario desbloqueado exitosamente");
}
