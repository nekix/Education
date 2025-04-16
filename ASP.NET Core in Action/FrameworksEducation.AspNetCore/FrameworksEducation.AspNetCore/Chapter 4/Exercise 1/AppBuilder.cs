using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FrameworksEducation.AspNetCore.Chapter_4.Exercise_1
{
    public class AppBuilder
    {
        public static WebApplication Configure(string[] args)
        {
            WebApplicationOptions options = new WebApplicationOptions()
            {
                Args = args
            };

            WebApplicationBuilder builder = WebApplication.CreateEmptyBuilder(options);

            builder.WebHost.UseKestrelCore();
            builder.WebHost.UseUrls("http://localhost:5005");

            builder.Services.AddRoutingCore();
            builder.Services.AddProblemDetails();

            WebApplication app = builder.Build();

            app.UseRouting();

            app.UseExceptionHandler(err => err.Run(async context => 
                await context.Response.WriteAsync("Total error!")));

            app.UseExceptionHandler("/error");

            app.Map("/throw", () =>
            {
                throw new NotImplementedException("Something wrong!");
            });

            app.Map("/error", ConfigureErrorEndpoint());

            app.Map("{*catch-all}",  () => "Hello Empty World!");

            return app;
        }

        private static Action<IApplicationBuilder> ConfigureErrorEndpoint()
        {
            return (errBuilder) =>
            {
                errBuilder.Run(async (context) =>
                {
                    if (DateTime.Now.Ticks % 2 == 0)
                        throw new Exception("Total exception");

                    context.Response.ContentType = MediaTypeNames.Text.Plain;

                    IProblemDetailsService? problemDetailsService =
                        context.RequestServices.GetService<IProblemDetailsService>();

                    if (problemDetailsService == null)
                        return;

                    IExceptionHandlerFeature? exceptionHandlerFeature =
                        context.Features.Get<IExceptionHandlerFeature>();

                    Exception? error = exceptionHandlerFeature?.Error;

                    context.Response.StatusCode = error != null && error.GetType().IsSubclassOf(typeof(NotImplementedException))
                        ? StatusCodes.Status501NotImplemented
                        : StatusCodes.Status500InternalServerError;

                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ProblemDetails
                        {
                            Title = "Some exception handled in endpoint!",
                            Detail = $"Exception type: {error?.Message}"
                        }
                    });
                });
            };
        }
    }
}
