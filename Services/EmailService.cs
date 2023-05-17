using FluentEmail.Core;
using FluentEmail.Core.Models;
using Hangfire;
using NextCommerce.Extensions;
using NextCommerce.Models.Emails;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _email;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IFluentEmail email, ILogger<EmailService> logger)
        {
            _email = email;
            _logger = logger;
        }

        public void SendEmailTemplate(string templateFileName, string subject, object? viewModel, Address[] recipients)
        {
            var response = _email
                .To(recipients)
                .Subject(subject)
                .UsingTemplateFromFile(templateFileName, viewModel)
                .Send();

            if (!response.Successful)
            {
                _logger.LogError(templateFileName, response);
                throw new Exception($"Failed to send email {templateFileName}. Error Messages: {string.Join(", ", response.ErrorMessages)}");
            }
        }

        public void SendOrderReceived(OrderReceivedViewModel viewModel, Address[] recipients) => 
            SendEmailTemplate(Path.Combine("EmailTemplates", "order_received.liquid"), "Orden Recibida", viewModel, recipients);

        public void SendOrderReceivedAndPaid(OrderReceivedAndPaidViewModel viewModel, Address[] recipients) => 
            SendEmailTemplate(Path.Combine("EmailTemplates", "order_received_and_paid.liquid"), "Orden Recibida y Pagada", viewModel, recipients);

        public void SendTestEmail(string toEmail)
        {
            var response = _email
                .To(toEmail)
                .Subject("Test Email")
                .Body("This is a test email. Please, ignore it.")
                .Tag("Testing")
                .Send();

            if (!response.Successful)
            {
                throw new Exception($"Failed to send test email. Error Messages: {string.Join(", ", response.ErrorMessages)}");
            }
        }
    }
}
