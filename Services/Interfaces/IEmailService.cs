using FluentEmail.Core.Models;
using NextCommerce.Models.Emails;

namespace NextCommerce.Services.Interfaces
{
    public interface IEmailService
    {
        void SendTestEmail(string toEmail);
        void SendOrderReceived(OrderReceivedViewModel viewModel, Address[] recipients);
        void SendOrderReceivedAndPaid(OrderReceivedAndPaidViewModel viewModel, Address[] recipients);
        void SendEmailTemplate(string templateFileName, string subject, object? viewModel, Address[] recipients);
    }
}
