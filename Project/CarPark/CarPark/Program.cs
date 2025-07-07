using CarPark.Data;
using CarPark.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace CarPark;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AppIdentityConst.ManagerPolicy, policy => policy.RequireClaim(AppIdentityConst.ManagerIdClaim));
        });

        builder.Services.AddSimpleIdentity<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi(static options =>
            {
                options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
            });

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        builder.AddDataProvidersServices();

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.InjectStylesheet("/swagger-ui/custom.css");
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "error",
            pattern: "Error/Index");

        app.Run();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataProvidersServices(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Unable to find database connection string. 'ConnectionStrings:Default' must be defined in configuration.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        return builder.Services;
    }

    public static IdentityBuilder AddSimpleIdentity<TUser>(this IServiceCollection services)
        where TUser : class
    {
        services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = new PathString("/Auth/Login");
                o.LogoutPath = new PathString("/Auth/Logout");
                o.ExpireTimeSpan = TimeSpan.FromHours(24);
                o.Cookie.HttpOnly = true;
                o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                o.Cookie.SameSite = SameSiteMode.Strict;
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
                };
            });

        return services.AddIdentityCore<TUser>(_ => { })
            .AddApiEndpoints();
    }
}