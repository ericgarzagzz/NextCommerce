using NextCommerce.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NextCommerce.Data.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Pendiente de pago")]
        [Description("El pedido se ha realizado pero aún no se ha recibido el pago.")]
        [Color(ClassName = "bg-warning text-dark")]
        Pending,

        [Display(Name = "Pago recibido")]
        [Description("Pago del pedido recibido.")]
        [Color(ClassName = "bg-success text-white")]
        PaymentReceived,

        [Display(Name = "Procesando")]
        [Description("El pedido está siendo procesado y preparado para su envío.")]
        [Color(ClassName = "bg-info text-white")]
        Processing,

        [Display(Name = "Surtido")]
        [Description("Todos los artículos del pedido están en stock y listos para ser enviados.")]
        [Color(ClassName = "bg-success text-white")]
        StockFulfilled,

        [Display(Name = "Enviado")]
        [Description("El pedido ha sido enviado pero aún no ha sido entregado.")]
        [Color(ClassName = "bg-primary text-white")]
        Shipped,

        [Display(Name = "Entregado")]
        [Description("El pedido ha sido entregado al cliente.")]
        [Color(ClassName = "bg-success text-white")]
        Delivered,

        [Display(Name = "Cancelado")]
        [Description("El pedido ha sido cancelado y no será cumplido.")]
        [Color(ClassName = "bg-danger text-white")]
        Cancelled
    }
}
