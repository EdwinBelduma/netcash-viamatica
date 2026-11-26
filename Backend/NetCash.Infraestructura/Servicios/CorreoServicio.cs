using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using NetCash.Aplicacion.Interfaces;

namespace NetCash.Infraestructura.Servicios;

public class CorreoServicio : ICorreoServicio
{
    private readonly IConfiguration _configuracion;

    public CorreoServicio(
        IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public async Task EnviarRecuperacionAsync(
        string destinatario,
        string nombres,
        string enlaceRecuperacion)
    {
        // ==========================================
        // CONFIGURACION DEL SERVIDOR SMTP
        // ==========================================

        var servidor = _configuracion["Correo:SmtpHost"]
            ?? throw new InvalidOperationException(
                "No se ha configurado el servidor SMTP.");

        var puerto = _configuracion.GetValue<int>(
            "Correo:SmtpPort", 587);

        var usuario = _configuracion["Correo:Usuario"];

        var contrasena = _configuracion["Correo:Contrasena"];

        var remitente = _configuracion["Correo:Remitente"]
            ?? throw new InvalidOperationException(
                "No se ha configurado el correo remitente.");

        var habilitarSsl = _configuracion.GetValue<bool>(
            "Correo:HabilitarSsl", true);

        // ==========================================
        // CONFIGURACION DEL MENSAJE
        // ==========================================

        var nombreSeguro = WebUtility.HtmlEncode(nombres);

        var enlaceSeguro = WebUtility.HtmlEncode(
            enlaceRecuperacion);

        using var mensaje = new MailMessage();

        mensaje.From = new MailAddress(
            remitente,
            "NetCash - Seguridad");

        mensaje.To.Add(new MailAddress(destinatario));

        mensaje.Subject =
            "NetCash | Recuperacion de contrasena";

        mensaje.SubjectEncoding = Encoding.UTF8;
        mensaje.BodyEncoding = Encoding.UTF8;
        mensaje.IsBodyHtml = true;

        // ==========================================
        // PLANTILLA HTML DEL CORREO
        // ==========================================

        mensaje.Body = $"""
            <html>
            <body style="background:#f1f5f9;
                         font-family:Arial,sans-serif;
                         padding:30px;">

                <div style="max-width:600px;
                            margin:auto;
                            background:#ffffff;
                            border-radius:12px;
                            overflow:hidden;">

                    <div style="background:#14213d;
                                padding:25px;
                                color:#ffffff;
                                text-align:center;">

                        <h1>NetCash</h1>

                        <p>Sistema de gestión empresarial</p>

                    </div>

                    <div style="padding:30px;
                                color:#334155;">

                        <h2>Recuperación de contraseña</h2>

                        <p>Hola, {nombreSeguro}.</p>

                        <p>
                            Hemos recibido una solicitud
                            para restablecer tu contraseña
                            de acceso a NetCash.
                        </p>

                        <div style="text-align:center;
                                    margin:30px 0;">

                            <a href="{enlaceSeguro}"
                               style="background:#2563eb;
                                      color:#ffffff;
                                      padding:14px 24px;
                                      text-decoration:none;
                                      border-radius:8px;">

                                Restablecer contraseña

                            </a>

                        </div>

                        <p>
                            Este enlace tiene una vigencia
                            de 15 minutos y solo puede
                            utilizarse una vez.
                        </p>

                        <p>
                            Si no solicitaste este cambio,
                            puedes ignorar este mensaje.
                        </p>

                    </div>

                    <div style="background:#f8fafc;
                                text-align:center;
                                padding:20px;
                                color:#64748b;
                                font-size:12px;">

                        NetCash © 2026

                        <p>
                            Mensaje automático de seguridad.
                            No responder.
                        </p>

                    </div>

                </div>

            </body>
            </html>
            """;

        // ==========================================
        // ENVIO DEL CORREO ELECTRONICO
        // ==========================================

        using var cliente = new SmtpClient(
            servidor,
            puerto);

        cliente.EnableSsl = habilitarSsl;

        cliente.UseDefaultCredentials = false;

        if (!string.IsNullOrWhiteSpace(usuario))
        {
            cliente.Credentials = new NetworkCredential(
                usuario,
                contrasena);
        }

        await cliente.SendMailAsync(mensaje);
    }
}