using CarPark.Data;
using CarPark.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using CarPark.Attributes;
using CarPark.Models.Models;
using CarPark.Models.Vehicles;
using CarPark.Models.Enterprises;
using CarPark.Shared.CQ;
using Microsoft.OpenApi.Models;
using FluentResults;

namespace CarPark;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AppIdentityConst.ManagerPolicy, policy => policy.RequireClaim(AppIdentityConst.ManagerIdClaim));

        builder.Services.AddSimpleIdentity<IdentityUser>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddProblemDetails();

        builder.Services.AddAntiforgery();
        builder.Services.AddSingleton<ValidateAntiforgeryTokenAuthorizationFilter>();

        builder.Services.AddCommandQueriesServices();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi(static options =>
            {
                options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;

                options.AddDocumentTransformer((document, context, token) =>
                {
                    document.Components ??= new OpenApiComponents();

                    document.Components.SecuritySchemes.Add("csrf", new OpenApiSecurityScheme
                    {
                        Name = "RequestVerificationToken",
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Description = "CSRF Token",
                    });

                    foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
                    {
                        operation.Value.Security.Add(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "csrf" }
                                },
                                Array.Empty<string>()
                            }
                        });
                    }

                    return Task.CompletedTask;
                });
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
    public static IServiceCollection AddCommandQueriesServices(this IServiceCollection services)
    {
        services.AddScoped<IVehiclesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IModelsDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IEnterprisesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ICommandHandler<CreateModelCommand, Result<int>>, CreateModelCommand.Handler>();
        services.AddScoped<ICommandHandler<UpdateModelCommand, Result<int>>, UpdateModelCommand.Handler>();
        services.AddScoped<ICommandHandler<DeleteModelCommand, Result>, DeleteModelCommand.Handler>();

        services.AddScoped<ICommandHandler<CreateVehicleCommand, Result<int>>, CreateVehicleCommand.Handler>();
        services.AddScoped<ICommandHandler<UpdateVehicleCommand, Result<int>>, UpdateVehicleCommand.Handler>();
        services.AddScoped<ICommandHandler<DeleteVehicleCommand, Result>, DeleteVehicleCommand.Handler>();

        services.AddScoped<ICommandHandler<DeleteEnterpriseCommand, Result>, DeleteEnterpriseCommand.Handler>();

        return services;
    }

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
                    OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync,
                    OnRedirectToLogin = context =>
                    {
                        if (IsApiRequest(context.Request))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                        
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        if (IsApiRequest(context.Request))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }
                        
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });

        return services.AddIdentityCore<TUser>(_ => { })
            .AddApiEndpoints();
    }
    
    private static bool IsApiRequest(HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api");
    }
}