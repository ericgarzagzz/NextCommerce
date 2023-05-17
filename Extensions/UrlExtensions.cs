using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NextCommerce.Extensions
{
    public static class UrlExtensions
    {
        public static string AbsoluteContent(this IUrlHelper urlHelper, string contentPath, HttpContext context)
        {
            var url = new Uri(baseUri: new Uri(context.Request.GetDisplayUrl()), relativeUri: urlHelper.Content(contentPath));
            return url.AbsoluteUri;
        }
    }
}
