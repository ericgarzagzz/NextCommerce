using NextCommerce.Models.Emails;

namespace NextCommerce.Services.Interfaces
{
    public interface IEmailViewModelService
    {
        OrderReceivedViewModel GetOrderReceivedViewModel(int orderId);
        OrderReceivedAndPaidViewModel GetOrderReceivedAndPaidViewModel(int orderId);
    }
}
