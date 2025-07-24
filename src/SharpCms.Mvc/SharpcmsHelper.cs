using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharpCms.Core.DataObjects;

namespace SharpCms.Mvc
{
    public class SharpcmsHelper
    {
        private readonly IHtmlHelper _htmlhelper;

        public SharpcmsHelper(IHtmlHelper htmlhelper)
        {
            _htmlhelper = htmlhelper;
        }

        public ICollection<Element> GetElementsForContainer(Func<Container, bool> expression)
        {
            var model = _htmlhelper.ViewData.Model as PageModel;
            if (model?.Page?.Containers == null)
                return new Collection<Element>();

            var container = model.Page.Containers.Where(expression).FirstOrDefault();

            if (container != null)
            {
                return container.Elements;
            }
            return new Collection<Element>();
        }

        public PageInfo? GetCurrentPage()
        {
            var model = _htmlhelper.ViewData.Model as PageModel;
            return model?.Page?.PageInfo;
        }
        
        public ICollection<PageInfo> GetSubPages()
        {
            var model = _htmlhelper.ViewData.Model as PageModel;
            return model?.Page?.PageInfo?.Children ?? new Collection<PageInfo>();
        }

    }
}