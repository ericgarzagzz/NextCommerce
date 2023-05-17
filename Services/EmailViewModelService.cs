using HashidsNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Models.Emails;
using NextCommerce.Services.Interfaces;
using System.Collections.Immutable;

namespace NextCommerce.Services
{
    public class EmailViewModelService : IEmailViewModelService
    {
        private readonly ApplicationDbContext _context;
        private readonly Hashids _hashIds;
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;

        public EmailViewModelService(ApplicationDbContext context, Hashids hashIds, IHttpContextAccessor accessor, LinkGenerator generator)
        {
            _context = context;
            _hashIds = hashIds;
            _accessor = accessor;
            _generator = generator;
        }

        public OrderReceivedAndPaidViewModel GetOrderReceivedAndPaidViewModel(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Details).ThenInclude(d => d.ShippingAddress)
                .Include(o => o.Details).ThenInclude(d => d.BillingAddress)
                .Include(o => o.LineItems).ThenInclude(l => l.Product).ThenInclude(p => p.Images).ThenInclude(i => i.Image)
                .First(o => o.Id == orderId);

            return new OrderReceivedAndPaidViewModel
            {
                HeaderImageUrl = _generator.GetPathByPage(_accessor.HttpContext, "/email/images/order-image-2.jpg"),
                CustomerName = order.Details.ShippingAddress.FullName,
                OrderId = _hashIds.Encode(order.Id),
                ShippingAddress = order.Details.ShippingAddress.PlainAddress,
                BillingAddress = order.Details.BillingAddress?.PlainAddress ?? string.Empty,
                Items = order.LineItems.Select(l => new OrderReceivedViewModel.LineItem
                {
                    ImageUrl = l.Product.Images.Any() ? _generator.GetPathByPage(_accessor.HttpContext, "/front/images/product/" + l.Product.Images.First().Image.Path) : _generator.GetPathByPage(_accessor.HttpContext, "/email/images/10.jpg"),
                    Name = l.Product.Name,
                    Quantity = l.Quantity.ToString(),
                    Price = (l.UnitPrice - l.UnitTaxes) * l.Quantity
                }).ToList(),
                Summary = new OrderReceivedViewModel.ItemsSummary
                {
                    Products = order.LineItems.Sum(l => (l.UnitPrice - l.UnitTaxes) * l.Quantity),
                    Tax = order.LineItems.Sum(l => l.UnitTaxes * l.Quantity),
                    Fees = order.Fees,
                    Total = order.Total
                },
                HideMediaLinks = true
            };
        }

        public OrderReceivedViewModel GetOrderReceivedViewModel(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.Details).ThenInclude(d => d.ShippingAddress)
                .Include(o => o.Details).ThenInclude(d => d.BillingAddress)
                .Include(o => o.LineItems).ThenInclude(l => l.Product).ThenInclude(p => p.Images).ThenInclude(i => i.Image)
                .First(o => o.Id == orderId);

            return new OrderReceivedAndPaidViewModel
            {
                HeaderImageUrl = _generator.GetPathByPage(_accessor.HttpContext, "/email/images/order-image-2.jpg"),
                CustomerName = order.Details.ShippingAddress.FullName,
                OrderId = _hashIds.Encode(order.Id),
                ShippingAddress = order.Details.ShippingAddress.PlainAddress,
                BillingAddress = order.Details.BillingAddress?.PlainAddress ?? string.Empty,
                Items = order.LineItems.Select(l => new OrderReceivedViewModel.LineItem
                {
                    ImageUrl = l.Product.Images.Any() ? _generator.GetPathByPage(_accessor.HttpContext, "/front/images/product/" + l.Product.Images.First().Image.Path) : _generator.GetPathByPage(_accessor.HttpContext, "/email/images/10.jpg"),
                    Name = l.Product.Name,
                    Quantity = l.Quantity.ToString(),
                    Price = (l.UnitPrice - l.UnitTaxes) * l.Quantity
                }).ToList(),
                Summary = new OrderReceivedViewModel.ItemsSummary
                {
                    Products = order.LineItems.Sum(l => (l.UnitPrice - l.UnitTaxes) * l.Quantity),
                    Tax = order.LineItems.Sum(l => l.UnitTaxes * l.Quantity),
                    Fees = order.Fees,
                    Total = order.Total
                },
                HideMediaLinks = true
            };
        }
    }
}
