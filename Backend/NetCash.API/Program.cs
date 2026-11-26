using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetCash.Aplicacion.Interfaces;
using NetCash.Aplicacion.Servicios;
using NetCash.Infraestructura.Persistencia;
using NetCash.Infraestructura.Repositorios;
using NetCash.Infraestructura.Seguridad;
using NetCash.Infraestructura.Servicios;
using Serilog;
using System.Security.Claims;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURACION DE SERILOG
// ==========================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/netcash-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Iniciando API de NetCash");

    // Configuracion de controladores
    builder.Services.AddControllers();

    // ==========================================
    // CONEXION A POSTGRESQL
    // ==========================================

    var cadenaConexion = builder.Configuration
        .GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "No se ha configurado la conexion a PostgreSQL.");

    builder.Services.AddDbContext<NetCashDbContext>(
        options => options.UseNpgsql(cadenaConexion));

    // ==========================================
    // CLAVES CRIPTOGRAFICAS
    // ==========================================

    var claveFirma = builder.Configuration["Jwt:ClaveFirma"]
        ?? throw new InvalidOperationException(
            "Falta la clave de firma JWT.");

    var claveCifradoJwt =
        builder.Configuration["Jwt:ClaveCifrado"]
        ?? throw new InvalidOperationException(
            "Falta la clave de cifrado JWT.");

    var claveCifradoDatos =
        builder.Configuration["Seguridad:ClaveCifrado"]
        ?? throw new InvalidOperationException(
            "Falta la clave de cifrado de datos.");

    var emisor = builder.Configuration["Jwt:Emisor"]
        ?? "NetCash.API";

    var audiencia = builder.Configuration["Jwt:Audiencia"]
        ?? "NetCash.Frontend";

    // ==========================================
    // INYECCION DE DEPENDENCIAS - SEGURIDAD
    // ==========================================

    builder.Services.AddSingleton<ICifradoServicio>(
        new CifradoServicio(claveCifradoDatos));

    builder.Services.AddSingleton<ITokenServicio>(
        new TokenServicio(
            claveFirma,
            claveCifradoJwt,
            emisor,
            audiencia));

    builder.Services.AddScoped<
        IAutenticacionServicio,
        AutenticacionServicio>();

    builder.Services.AddScoped<InicializadorUsuarios>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - CLIENTES
    // ==========================================

    builder.Services.AddScoped<
        IClienteRepositorio,
        ClienteRepositorio>();

    builder.Services.AddScoped<
        IClienteServicio,
        ClienteServicio>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - USUARIOS
    // ==========================================

    builder.Services.AddScoped<
        IUsuarioServicio,
        UsuarioServicio>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - CAJAS
    // ==========================================

    builder.Services.AddScoped<
        ICajaServicio,
        CajaServicio>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - TURNOS
    // ==========================================

    builder.Services.AddScoped<
        ITurnoServicio,
        TurnoServicio>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - CATALOGO
    // ==========================================

    builder.Services.AddScoped<
        ICatalogoServicio,
        CatalogoServicio>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - OPERACIONES
    // ==========================================

    builder.Services.AddScoped<
        IOperacionServicio,
        OperacionServicio>();

    // ==========================================
    // DASHBOARD Y MENU DINAMICO
    // ==========================================

    builder.Services.AddScoped<
        IDashboardServicio,
        DashboardServicio>();

    builder.Services.AddScoped<InicializadorMenus>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - IMPORTACIONES
    // ==========================================

    builder.Services.AddScoped<
        IImportacionServicio,
        ImportacionServicio>();

    // ==========================================
    // INYECCION DE DEPENDENCIAS - RECUPERACION
    // ==========================================

    builder.Services.AddScoped<
        IRecuperacionServicio,
        RecuperacionServicio>();

    builder.Services.AddScoped<
        ICorreoServicio,
        CorreoServicio>();

    // ==========================================
    // LIMITACION DE SOLICITUDES - RATE LIMITING
    // ==========================================

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode =
            StatusCodes.Status429TooManyRequests;

        options.AddPolicy("Recuperacion", contexto =>
        {
            var ip = contexto.Connection
                .RemoteIpAddress?.ToString()
                ?? "desconocida";

            return RateLimitPartition
                .GetFixedWindowLimiter(
                    partitionKey: ip,
                    factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,

                            Window = TimeSpan.FromMinutes(1),

                            QueueLimit = 0,

                            AutoReplenishment = true
                        });
        });

        options.OnRejected = async (
            contexto,
            cancellationToken) =>
        {
            contexto.HttpContext.Response.StatusCode =
                StatusCodes.Status429TooManyRequests;

            await contexto.HttpContext.Response
                .WriteAsJsonAsync(
                    new
                    {
                        mensaje =
                            "Demasiadas solicitudes. " +
                            "Intenta nuevamente en un minuto."
                    },
                    cancellationToken: cancellationToken);
        };
    });

    // ==========================================
    // AUTENTICACION JWT Y JWE
    // ==========================================

    builder.Services.AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = true;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = emisor,

                    ValidateAudience = true,
                    ValidAudience = audiencia,

                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(claveFirma)),

                    TokenDecryptionKey =
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(claveCifradoJwt)),

                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    ClockSkew = TimeSpan.FromSeconds(30),

                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };
        });

    builder.Services.AddAuthorization();

    // ==========================================
    // DOCUMENTACION SWAGGER
    // ==========================================

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ==========================================
    // CONFIGURACION CORS PARA ANGULAR
    // ==========================================

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Angular", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    // Registro de solicitudes HTTP
    app.UseSerilogRequestLogging();

    // ==========================================
    // MIGRACIONES E INICIALIZACION DEL SISTEMA
    // ==========================================

    using (var scope = app.Services.CreateScope())
    {
        // ======================================
        // MIGRACIONES AUTOMATICAS DE POSTGRESQL
        // ======================================

        var dbContext = scope.ServiceProvider
            .GetRequiredService<NetCashDbContext>();

        Log.Information(
            "Verificando migraciones de PostgreSQL.");

        await dbContext.Database.MigrateAsync();

        Log.Information(
            "Migraciones de PostgreSQL verificadas correctamente.");

        // ======================================
        // INICIALIZACION DE MENUS DINAMICOS
        // ======================================

        var inicializadorMenus = scope.ServiceProvider
            .GetRequiredService<InicializadorMenus>();

        await inicializadorMenus.InicializarAsync();

        Log.Information(
            "Menus y permisos inicializados correctamente.");

        // ======================================
        // CREACION DEL ADMINISTRADOR INICIAL
        // ======================================

        var contrasena = builder.Configuration[
            "Administrador:Contrasena"];

        if (!string.IsNullOrWhiteSpace(contrasena))
        {
            var inicializador = scope.ServiceProvider
                .GetRequiredService<InicializadorUsuarios>();

            var creado = await inicializador
                .InicializarAsync(contrasena);

            if (creado)
            {
                Log.Information(
                    "Usuario administrador inicial creado.");
            }
        }

        Log.Information(
            "Inicializacion de NetCash completada.");
    }

    // ==========================================
    // DOCUMENTACION DE LA API
    // ==========================================

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "NetCash API v1");
        });
    }

    // ==========================================
    // CONFIGURACION DEL MIDDLEWARE
    // ==========================================

    app.UseRouting();

    app.UseCors("Angular");

    app.UseAuthentication();

    app.UseRateLimiter();

    app.UseAuthorization();

    app.MapControllers();

    // ==========================================
    // ENDPOINT DE ESTADO DEL SISTEMA
    // ==========================================

    app.MapGet("/api/estado", () => Results.Ok(new
    {
        sistema = "NetCash",
        estado = "Operativo",
        fecha = DateTime.UtcNow
    }));

    // ==========================================
    // INICIO DE LA APLICACION
    // ==========================================

    Log.Information(
        "NetCash API configurada correctamente.");

    app.Run();
}
catch (Exception ex) when (
    ex is not Microsoft.Extensions.Hosting.HostAbortedException)
{
    Log.Fatal(ex, "Error al iniciar la API NetCash");
    throw;
}
finally
{
    Log.CloseAndFlush();
}