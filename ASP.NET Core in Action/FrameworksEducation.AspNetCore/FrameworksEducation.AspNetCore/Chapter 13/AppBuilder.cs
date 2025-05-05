using FrameworksEducation.AspNetCore.Chapter_13.Core.Products;
using FrameworksEducation.AspNetCore.Chapter_13.Services;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.FileProviders;

namespace FrameworksEducation.AspNetCore.Chapter_13;

public class AppBuilder
{
    public static WebApplication Configure(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        IMvcBuilder mvcBuilder = builder.Services.AddRazorPages()
            .AddRazorPagesOptions(opts =>
            {
                opts.Conventions.Add(new PageRouteTransformerConvention(
                    new KebabCaseParameterTransformer()));
            })
            .WithRazorPagesRoot("/Chapter 13/Pages");

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

        app.UseAuthorization();

        app.MapRazorPages();

        return app;
    }
}