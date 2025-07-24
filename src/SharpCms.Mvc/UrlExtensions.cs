using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SharpCms.Mvc
{
    public static class UrlExtensions
    {
        public static string ActionUrl(this IHtmlHelper helper, string action, string controller, object? routeValues = null)
        {
            var urlHelper = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Mvc.IUrlHelper>();
            return urlHelper.Action(new UrlActionContext
            {
                Action = action,
                Controller = controller,
                Values = routeValues
            }) ?? string.Empty;
        }

        public static string ActionUrl(this IHtmlHelper helper, string action, string controller)
        {
            var urlHelper = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Mvc.IUrlHelper>();
            return urlHelper.Action(new UrlActionContext
            {
                Action = action,
                Controller = controller
            }) ?? string.Empty;
        }

        public static string ActionUrl(this IHtmlHelper helper, string action)
        {
            var urlHelper = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Mvc.IUrlHelper>();
            return urlHelper.Action(new UrlActionContext
            {
                Action = action
            }) ?? string.Empty;
        }
    }
}