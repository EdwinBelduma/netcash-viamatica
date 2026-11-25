
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    // INICIALIZACION DE MENUS Y ADMINISTRADOR
    // ==========================================

    using (var scope = app.Services.CreateScope())
    {
        // Inicializar menus dinamicos
        var inicializadorMenus = scope.ServiceProvider
            .GetRequiredService<InicializadorMenus>();

        await inicializadorMenus.InicializarAsync();

        // Crear administrador inicial
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

    app.UseCors("Angular");

    app.UseAuthentication();

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
