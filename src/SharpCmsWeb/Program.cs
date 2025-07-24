using Autofac;
using Autofac.Extensions.DependencyInjection;
using SharpCms.Core.Contracts.Data;
using SharpCms.Core.DataObjects;
using SharpCms.Data.FileSystem;

var builder = WebApplication.CreateBuilder(args);

// Configure Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register dependencies with constructor parameters
    containerBuilder.Register(c => new PageProvider(
        c.Resolve<IWebHostEnvironment>(), 
        "~/App_Data/database")).As<IPageProvider>();
        
    containerBuilder.Register(c => new SitetreeProvider(
        c.Resolve<IWebHostEnvironment>(), 
        "~/App_Data/database")).As<ISitetreeProvider>();
});

// Add minimal services
builder.Services.AddRazorPages();
builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthorization();

// Minimal API endpoints
app.MapGet("/", () => Results.Ok("Welcome to SharpCMS"));

app.MapGet("/error", () => Results.Problem("An error occurred"));

app.MapGet("/admin", () => Results.Ok("Admin Area"));

// Dynamic page routing with simplified logic
app.MapGet("/{page1?}/{page2?}/{page3?}/{page4?}/{page5?}", 
    (HttpContext context, IPageProvider pageProvider, ISitetreeProvider sitetreeProvider,
     string? page1 = null, string? page2 = null, string? page3 = null, 
     string? page4 = null, string? page5 = null) =>
{
    // Set defaults using null-coalescing
    page1 ??= "german";
    page2 ??= "about";

    // Build page path inline
    var pagePath = string.Join("/", new[] { page1, page2, page3, page4, page5 }
        .Where(p => !string.IsNullOrEmpty(p)));
    
    try
    {
        var siteTree = sitetreeProvider.GetSiteTree();
        var currentPage = FindPageInTree(siteTree, pagePath);
        
        if (currentPage == null)
            return Results.NotFound("Page not found");

        var containers = pageProvider.GetCurrentPageContainers(currentPage);
        
        // Inline response model
        return Results.Ok(new
        {
            Page = new { PageInfo = currentPage, Containers = containers },
            SiteTree = siteTree,
            Basepath = context.Request.PathBase.ToString(),
            Domain = context.Request.Host.ToString(),
            Referrer = context.Request.Headers.Referer.ToString(),
            Useragent = context.Request.Headers.UserAgent.ToString(),
            Query = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString())
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error loading page: {ex.Message}");
    }
});

// Simplified API endpoints
app.MapGet("/api/pages", (IPageProvider pageProvider) => 
    Results.Ok("Pages API endpoint"));

app.MapGet("/api/sitetree", (ISitetreeProvider sitetreeProvider) =>
{
    try
    {
        return Results.Ok(sitetreeProvider.GetSiteTree());
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error getting site tree: {ex.Message}");
    }
});

app.Run();

// Local function for page tree navigation
static PageInfo? FindPageInTree(PageInfo rootPage, string pagePath)
{
    // Check if current page matches
    if (rootPage.PageName?.Equals(pagePath, StringComparison.OrdinalIgnoreCase) == true ||
        rootPage.UrlName?.Equals(pagePath, StringComparison.OrdinalIgnoreCase) == true)
        return rootPage;

    // Search children recursively
    if (rootPage.Children != null)
    {
        foreach (var child in rootPage.Children)
        {
            var result = FindPageInTree(child, pagePath);
            if (result != null) return result;
        }
    }

    return null;
}
