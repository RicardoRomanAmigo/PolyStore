using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PolyStore.Application.Abstractions.Services;

namespace PolyStore.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(Guid orderId, string email)
    {
        try
        {
            var emailMessage = new MimeMessage();

            // Datos del remitente (leídos de appsettings)
            var senderName = _configuration["SmtpSettings:SenderName"] ?? "PolyStore 3D";
            var senderEmail = _configuration["SmtpSettings:SenderEmail"] ?? throw new InvalidOperationException("La configuración 'SmtpSettings:SenderEmail' es obligatoria.");

            emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = $"¡Confirmación de tu pedido en PolyStore! (Ref: {orderId})";

            // Cuerpo del correo (Aquí puedes meter una plantilla HTML mucho más bonita luego)
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h1>¡Gracias por tu compra!</h1>
                    <p>Hemos recibido el pago correctamente y tu pedido ya está en proceso.</p>
                    <p><strong>Identificador del pedido:</strong> {orderId}</p>
                    <br/>
                    <p>El equipo de PolyStore</p>"
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Conexión al servidor SMTP
            // VALIDACIÓN ESTRICTA: Nos aseguramos de que no sean nulos
            var server = _configuration["SmtpSettings:Server"]
                ?? throw new InvalidOperationException("La configuración 'SmtpSettings:Server' es obligatoria.");
            var portString = _configuration["SmtpSettings:Port"]
                ?? throw new InvalidOperationException("La configuración 'SmtpSettings:Port' es obligatoria.");
            var port = int.Parse(portString);

            // Ahora el compilador sabe que server no puede ser nulo
            await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);

            // --- VALIDACIONES ESTRICTAS DE AUTENTICACIÓN ---
            var user = _configuration["SmtpSettings:Username"] ?? throw new InvalidOperationException("La configuración 'SmtpSettings:Username' es obligatoria.");
            var pass = _configuration["SmtpSettings:Password"] ?? throw new InvalidOperationException("La configuración 'SmtpSettings:Password' es obligatoria.");
            await client.AuthenticateAsync(user, pass);

            // Envío
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email de confirmación enviado con éxito a {Email} para el pedido {OrderId}", email, orderId);
        }
        catch (Exception ex)
        {
            // Logueamos el error, pero Hangfire se encargará de reintentar la tarea si falla
            _logger.LogError(ex, "Fallo al enviar el email de confirmación para el pedido {OrderId}", orderId);
            throw; // Propagamos la excepción para que Hangfire sepa que falló y lo ponga en reintento
        }
    }
}