using CarPark.Attributes;
using CarPark.Data;
using CarPark.Identity;
using CarPark.Rides.Events;
using CarPark.Telegram;
using CarPark.Telegram.Events;
using CarPark.Telegram.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO.Converters;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace CarPark;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews()
            .AddJsonOptions(options =>
            {
                GeoJsonConverterFactory geoJsonConverterFactory = new GeoJsonConverterFactory();
                options.JsonSerializerOptions.Converters.Add(geoJsonConverterFactory);
            });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        builder.Services
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("Database");

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(AppIdentityConst.ManagerPolicy, policy => policy.RequireClaim(AppIdentityConst.ManagerIdClaim));

        builder.Services.AddSimpleIdentity<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, AppUserClaimsPrincipalFactory>();

        builder.Services.AddProblemDetails();

        builder.Services.AddAntiforgery();
        builder.Services.AddSingleton<ValidateAntiforgeryTokenAuthorizationFilter>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }
        
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

                foreach (KeyValuePair<OperationType, OpenApiOperation> operation in document.Paths.Values.SelectMany(path => path.Operations))
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


        builder.AddDataProvidersServices();
        builder.AddApplicationServices();
        builder.AddTelegramServices();

        WebApplication app = builder.Build();

        app.UseForwardedHeaders();

        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.InjectStylesheet("/swagger-ui/custom.css");
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
        });

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

        app.MapHealthChecks("/health");

        app.Run();
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddTelegramServices(this WebApplicationBuilder builder)
    {
        // Telegram Bot Configuration
        builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection(BotConfiguration.Key));

        builder.Services.AddHttpClient("Telegram", client =>
        {
            client.BaseAddress = new Uri("https://api.telegram.org/bot");
        });

        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<BotConfiguration>>().Value;
            return new TelegramBotClient(config.BotToken);
        });

        builder.Services.AddSingleton<ITelegramAuthenticationService, TelegramAuthenticationService>();
        builder.Services.AddSingleton<ITelegramReportService, TelegramReportService>();
        builder.Services.AddSingleton<IUpdateHandler, TelegramUpdateHandler>();
        builder.Services.AddHostedService<TelegramBotService>();

        builder.Services.AddTransient< ICreateRideEventHandler, TelegramCreateRideEventHandler>();
    }

    public static void AddDataProvidersServices(this WebApplicationBuilder builder)
    {
        InfrastractureModuleConfigurator infrastractureModule = new InfrastractureModuleConfigurator();

        infrastractureModule.ConfigureModule(builder.Services, builder.Configuration, null);
    }

    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        ApplicationModuleConfigurator applicationModule = new ApplicationModuleConfigurator();

        applicationModule.ConfigureModule(builder.Services, builder.Configuration);
    }

    public static IdentityBuilder AddSimpleIdentity<TUser>(this IServiceCollection services)
        where TUser : class
    {
        services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies(builder =>
            {
                builder!.ApplicationCookie!.Configure(o =>
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
            });

        return services.AddIdentityCore<TUser>(_ => { })
            .AddApiEndpoints();
    }
    
    private static bool IsApiRequest(HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api");
    }
}