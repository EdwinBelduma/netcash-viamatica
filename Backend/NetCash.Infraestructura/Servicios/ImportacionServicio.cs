using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.FileIO;
using NetCash.Aplicacion.DTOs.Clientes;
using NetCash.Aplicacion.DTOs.Usuarios;
using NetCash.Aplicacion.DTOs.Importaciones;
using NetCash.Aplicacion.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace NetCash.Infraestructura.Servicios;

public class ImportacionServicio : IImportacionServicio
{
    private readonly IUsuarioServicio _usuarioServicio;
    private readonly IClienteServicio _clienteServicio;
    private readonly ILogger<ImportacionServicio> _logger;

    public ImportacionServicio(
        IUsuarioServicio usuarioServicio,
        IClienteServicio clienteServicio,
        ILogger<ImportacionServicio> logger)
    {
        _usuarioServicio = usuarioServicio;
        _clienteServicio = clienteServicio;
        _logger = logger;
    }

    // ==========================================
    // IMPORTACION DE USUARIOS
    // ==========================================

    public async Task<ImportacionRespuesta> ImportarUsuariosAsync(
        Stream archivo,
        int usuarioResponsableId)
    {
        return await ProcesarArchivoAsync(
            archivo,
            new[]
            {
                "NombreUsuario",
                "Nombres",
                "Apellidos",
                "Identificacion",
                "Correo",
                "Contrasena",
                "RolId"
            },
            usuarioResponsableId,
            async campos =>
            {
                int rolId;

                if (campos[6].Equals(
                    "Gestor",
                    StringComparison.OrdinalIgnoreCase))
                {
                    rolId = 2;
                }
                else if (campos[6].Equals(
                    "Cajero",
                    StringComparison.OrdinalIgnoreCase))
                {
                    rolId = 3;
                }
                else if (!int.TryParse(campos[6], out rolId))
                {
                    throw new ValidationException(
                        "El rol debe ser Gestor (2) o Cajero (3).");
                }

                if (rolId != 2 && rolId != 3)
                {
                    throw new ValidationException(
                        "El rol indicado no está permitido.");
                }

                var solicitud = new UsuarioSolicitud
                {
                    NombreUsuario = campos[0],
                    Nombres = campos[1],
                    Apellidos = campos[2],
                    Identificacion = campos[3],
                    Correo = campos[4],
                    Contrasena = campos[5],
                    RolId = rolId
                };

                ValidarSolicitud(solicitud);

                await _usuarioServicio.CrearAsync(
                    solicitud,
                    usuarioResponsableId);
            });
    }

    // ==========================================
    // IMPORTACION DE CLIENTES
    // ==========================================

    public async Task<ImportacionRespuesta> ImportarClientesAsync(
        Stream archivo,
        int usuarioResponsableId)
    {
        return await ProcesarArchivoAsync(
            archivo,
            new[]
            {
                "Nombres",
                "Apellidos",
                "Identificacion",
                "Correo",
                "Telefono",
                "Direccion",
                "ReferenciaDireccion"
            },
            usuarioResponsableId,
            async campos =>
            {
                var solicitud = new ClienteSolicitud
                {
                    Nombres = campos[0],
                    Apellidos = campos[1],
                    Identificacion = campos[2],
                    Correo = campos[3],
                    Telefono = campos[4],
                    Direccion = campos[5],
                    ReferenciaDireccion = campos[6]
                };

                ValidarSolicitud(solicitud);

                await _clienteServicio.CrearAsync(solicitud);
            });
    }

    // ==========================================
    // PROCESAMIENTO DE ARCHIVOS CSV
    // ==========================================

    private async Task<ImportacionRespuesta> ProcesarArchivoAsync(
        Stream archivo,
        string[] encabezadosEsperados,
        int usuarioResponsableId,
        Func<string[], Task> procesarRegistro)
    {
        var resultado = new ImportacionRespuesta();

        using var memoria = new MemoryStream();

        await archivo.CopyToAsync(memoria);

        memoria.Position = 0;

        string contenido;

        using (var lectorTexto = new StreamReader(
            memoria,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 1024,
            leaveOpen: true))
        {
            contenido = await lectorTexto.ReadToEndAsync();
        }

        if (string.IsNullOrWhiteSpace(contenido))
        {
            resultado.Errores.Add(
                "El archivo CSV está vacío.");

            return resultado;
        }

        using var lectorEncabezado =
            new StringReader(contenido);

        var primeraLinea =
            lectorEncabezado.ReadLine() ?? string.Empty;

        // Detectar automáticamente el separador CSV.

        string separador = primeraLinea.Contains(';')
            ? ";"
            : ",";

        // Compatibilidad con archivos que incluyen sep=;

        if (primeraLinea.StartsWith(
            "sep=",
            StringComparison.OrdinalIgnoreCase))
        {
            separador = primeraLinea[4..].Trim();

            contenido = lectorEncabezado.ReadToEnd();
        }

        if (separador != ";" && separador != ",")
        {
            resultado.Errores.Add(
                "El separador del archivo no es válido.");

            return resultado;
        }

        using var lector = new TextFieldParser(
            new StringReader(contenido));

        lector.TextFieldType = FieldType.Delimited;
        lector.SetDelimiters(separador);
        lector.HasFieldsEnclosedInQuotes = true;
        lector.TrimWhiteSpace = true;

        if (lector.EndOfData)
        {
            resultado.Errores.Add(
                "El archivo no contiene información.");

            return resultado;
        }

        string[]? encabezados;

        try
        {
            encabezados = lector.ReadFields();
        }
        catch (MalformedLineException)
        {
            resultado.Errores.Add(
                "Los encabezados del archivo no son válidos.");

            return resultado;
        }

        if (encabezados == null ||
            !encabezados.SequenceEqual(
                encabezadosEsperados,
                StringComparer.OrdinalIgnoreCase))
        {
            resultado.Errores.Add(
                "Las columnas no coinciden con la plantilla. " +
                "Verifica los encabezados y su orden.");

            return resultado;
        }

        // ======================================
        // PROCESAR REGISTROS
        // ======================================

        while (!lector.EndOfData)
        {
            long numeroLinea = lector.LineNumber;

            try
            {
                var campos = lector.ReadFields();

                if (campos == null ||
                    campos.All(string.IsNullOrWhiteSpace))
                {
                    continue;
                }

                resultado.TotalRegistros++;

                if (campos.Length != encabezadosEsperados.Length)
                {
                    throw new ValidationException(
                        "La cantidad de columnas es incorrecta.");
                }

                await procesarRegistro(campos);

                resultado.RegistrosImportados++;
            }
            catch (MalformedLineException)
            {
                resultado.TotalRegistros++;
                resultado.RegistrosRechazados++;

                resultado.Errores.Add(
                    $"Línea {numeroLinea}: formato CSV incorrecto.");
            }
            catch (ValidationException)
            {
                resultado.RegistrosRechazados++;

                resultado.Errores.Add(
                    $"Línea {numeroLinea}: datos inválidos. " +
                    "Verifica los campos obligatorios y sus formatos.");
            }
            catch (InvalidOperationException)
            {
                resultado.RegistrosRechazados++;

                resultado.Errores.Add(
                    $"Línea {numeroLinea}: el registro incumple " +
                    "una regla de negocio o puede estar duplicado.");
            }
            catch (Exception ex)
            {
                resultado.RegistrosRechazados++;

                resultado.Errores.Add(
                    $"Línea {numeroLinea}: no se pudo guardar el registro.");

                _logger.LogWarning(
                    "Error de importación. Usuario: {UsuarioId}. " +
                    "Línea: {Linea}. Tipo: {TipoError}",
                    usuarioResponsableId,
                    numeroLinea,
                    ex.GetType().Name);
            }
        }

        if (resultado.TotalRegistros == 0)
        {
            resultado.Errores.Add(
                "La plantilla no contiene registros para importar.");
        }

        _logger.LogInformation(
            "Importación finalizada. Usuario: {UsuarioId}. " +
            "Total: {Total}. Importados: {Importados}. " +
            "Rechazados: {Rechazados}",
            usuarioResponsableId,
            resultado.TotalRegistros,
            resultado.RegistrosImportados,
            resultado.RegistrosRechazados);

        return resultado;
    }

    // ==========================================
    // VALIDACION DE DATOS
    // ==========================================

    private static void ValidarSolicitud(object solicitud)
    {
        var contexto = new ValidationContext(solicitud);

        Validator.ValidateObject(
            solicitud,
            contexto,
            validateAllProperties: true);
    }
}