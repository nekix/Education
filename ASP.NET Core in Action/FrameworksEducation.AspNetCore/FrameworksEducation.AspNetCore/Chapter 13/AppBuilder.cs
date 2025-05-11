using FrameworksEducation.AspNetCore.Chapter_13.Core.Products;
using FrameworksEducation.AspNetCore.Chapter_13.Services;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace FrameworksEducation.AspNetCore.Chapter_13;

public class AppBuilder
{
    public static WebApplication Configure(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        IMvcBuilder mvcBuilder = builder.Services.AddControllersWithViews()
            .Services.AddRazorPages()
            .Services.AddControllers()
            .AddRazorPagesOptions(opts =>
            {
                opts.Conventions.Add(new PageRouteTransformerConvention(
                    new KebabCaseParameterTransformer()));

                opts.Conventions.Add(new PrefixingRazorPagesRouteModelConvention("razor-pages"));
            })
            .WithRazorPagesRoot("/Chapter 13/RazorPages/Pages")
            .AddRazorOptions(opts =>
            {
                opts.PageViewLocationFormats.Clear();
                opts.PageViewLocationFormats.Add("/Chapter 13/RazorPages/Pages/{1}/{0}.cshtml");
                opts.PageViewLocationFormats.Add("/Chapter 13/RazorPages/Pages/Shared/{0}.cshtml");

                opts.ViewLocationFormats.Clear();
                opts.ViewLocationFormats.Add("Chapter 13/Mvc/Views/{1}/{0}.cshtml");
                opts.ViewLocationFormats.Add("Chapter 13/Mvc/Views/Shared/{0}.cshtml");
            });

        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen();

        if (builder.Environment.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        builder.Services.Configure<RouteOptions>(o =>
        {
            o.LowercaseUrls = true;
            o.LowercaseQueryStrings = true;
            o.AppendTrailingSlash = true;
        });

        builder.Services.AddSingleton<ProductAppService>();

        WebApplication app = builder.Build();

        app.UseSwagger(o =>
        {
            o.RouteTemplate = "/web-api/swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/web-api/swagger/v1/swagger.json", "v1");
            o.RoutePrefix = "web-api/swagger";
        });

        app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "Chapter 13", "wwwroot")),
            RequestPath = ""
        });

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "mvc/{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}