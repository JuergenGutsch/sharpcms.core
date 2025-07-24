using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using SharpCms.Core.Contracts.Data;
using SharpCms.Core.DataObjects;

namespace SharpCms.Data.FileSystem
{
    public class PageProvider : IPageProvider
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;

        public PageProvider(IWebHostEnvironment webHostEnvironment, string connectionString)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = connectionString;
        }

        private string MapPath(string virtualPath)
        {
            return Path.Combine(_webHostEnvironment.ContentRootPath, virtualPath.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar));
        }

        public ICollection<Container> GetCurrentPageContainers(PageInfo currentpage)
        {
            var pagefile = String.Format("{0}.xml", currentpage.PageIdentifier);
            var path = MapPath(Path.Combine(_connectionString, pagefile));

            var result = new Collection<Container>();
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var document = XDocument.Load(fileStream);

                var tree = document.Element("page")?.Elements("containers");
                if (tree == null) return result;

                foreach (var xcontainer in tree.Elements("container"))
                {
                    var nameAttribute = xcontainer.Attribute("name");
                    if (nameAttribute == null) continue;

                    var container = new Container
                        {
                            Id = Guid.NewGuid(),
                            Name = nameAttribute.Value
                        };

                    AddElements(container, xcontainer);

                    result.Add(container);
                }
            }

            return result;
        }

        private void AddElements(Container container, XElement xcontainer)
        {
            var elementsContainer = xcontainer.Element("elements");
            if (elementsContainer == null) return;

            foreach (var xelement in elementsContainer.Elements("element"))
            {
                var typeAttribute = xelement.Attribute("type");
                if (typeAttribute == null) continue;

                var element = new Element
                    {
                        Id = Guid.NewGuid(),
                        ElementTypeName = typeAttribute.Value,
                        Published = true
                    };

                AddParameters(element, xelement);

                container.Elements.Add(element);
            }
        }

        private void AddParameters(Element element, XContainer xelement)
        {
            foreach (var xparameter in xelement.Elements())
            {
                var parameter = new Parameter()
                    {
                        Id = Guid.NewGuid(),
                        Name = xparameter.Name.LocalName,
                        Value = xparameter.Value,
                        Type = typeof (string)
                    };
                element.Parameters.Add(parameter);
            }
        }
    }
}