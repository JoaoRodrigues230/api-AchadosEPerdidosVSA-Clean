using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace API_AchadosEPerdidos.Shared.Infrastructure.Email;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await Task.Run(() => client.Send(mailMessage));
            Console.WriteLine("--- E-mail enviado com sucesso ao servidor SMTP! ---");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--- ERRO AO ENVIAR EMAIL: {ex.Message} ---");
            throw; 
        }
    }
}