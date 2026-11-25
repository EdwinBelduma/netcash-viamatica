
using Microsoft.EntityFrameworkCore;
using NetCash.Infraestructura.Persistencia;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de Serilog
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

    // Conexion a PostgreSQL
    var cadenaConexion = builder.Configuration
        .GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "No se ha configurado la conexion a PostgreSQL.");

    builder.Services.AddDbContext<NetCashDbContext>(
        options => options.UseNpgsql(cadenaConexion));

    // Configuracion de Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configuracion CORS para Angular
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Angular", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Autorizacion
    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Registro de solicitudes HTTP
    app.UseSerilogRequestLogging();

    // Documentacion Swagger
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

    app.UseCors("Angular");

    app.UseAuthorization();

    app.MapControllers();

    // Endpoint de verificacion del sistema
    app.MapGet("/api/estado", () => Results.Ok(new
    {
        sistema = "NetCash",
        estado = "Operativo",
        fecha = DateTime.UtcNow
    }));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error al iniciar la API NetCash");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

