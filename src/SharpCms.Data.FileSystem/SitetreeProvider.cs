using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using SharpCms.Core;
using SharpCms.Core.Contracts.Data;
using SharpCms.Core.DataObjects;

namespace SharpCms.Data.FileSystem
{
    public class SitetreeProvider : ISitetreeProvider
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;

        public SitetreeProvider(IWebHostEnvironment webHostEnvironment, string connectionString)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = connectionString;
        }

        private string MapPath(string virtualPath)
        {
            return Path.Combine(_webHostEnvironment.ContentRootPath, virtualPath.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar));
        }

        public PageInfo GetSiteTree()
        {
            var siteTree = new PageInfo
                {
                    Id = Guid.NewGuid(),
                    PageIdentifier = String.Empty,
                    Menuname = string.Empty,
                    InPath = true,
                    PageName = "root",
                };

            var path = MapPath(Path.Combine(_connectionString, "tree.xml"));
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var document = XDocument.Load(fileStream);
                var tree = document.Element("tree");

                if (tree != null)
                {
                    AddSubPages(siteTree, tree);
                }
            }

            return siteTree;
        }

        private void AddSubPages(PageInfo parentPage, XContainer parentElemet)
        {
            foreach (var element in parentElemet.Elements())
            {
                var page = new PageInfo
                    {
                        Id = Guid.NewGuid(),
                        PageIdentifier = element.Attribute("pageidentifier", String.Empty).Value,
                        Menuname = element.Attribute("menuname", String.Empty).Value,
                        PageName = element.Attribute("pagename", String.Empty).Value,
                        UrlName = element.Name.LocalName,
                        InPath = false,
                        NavState = (NavState)Enum.Parse(typeof(NavState), element.Attribute("status", "open").Value.ToUpper()),
                        LastEdited = DateTime.Now,
                        Tags = element.Attribute("metakeywords", String.Empty).Value.Split(new[] { ',' }),
                        Description = element.Attribute("metadescription", String.Empty).Value,
                        Template = "Show"
                    };
                parentPage.Children.Add(page);
                AddSubPages(page, element);

            }
        }
    }
}
