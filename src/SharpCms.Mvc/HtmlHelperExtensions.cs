using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;

namespace SharpCms.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static SharpcmsHelper Sharpcms(this IHtmlHelper htmlhelper)
        {
            return new SharpcmsHelper(htmlhelper);
        }

        // Note: RenderSection functionality would need to be implemented differently in ASP.NET Core
        // This is just a placeholder - the actual implementation would depend on your specific needs
        public static IHtmlContent RenderSection(this IHtmlHelper htmlHelper, string name, Func<dynamic, IHtmlContent>? defaultContents)
        {
            // This would need to be implemented based on your specific section rendering logic
            return defaultContents?.Invoke(null!) ?? HtmlString.Empty;
        }
    }
}
