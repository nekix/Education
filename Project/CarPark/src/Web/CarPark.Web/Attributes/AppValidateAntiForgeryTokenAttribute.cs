using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CarPark.Attributes;

/// <summary>
/// Specifies that the class or method that this attribute is applied validates the anti-forgery token.
/// If the anti-forgery token is not available, or if the token is invalid, the validation will fail
/// and the action method will not execute.
/// </summary>
/// <remarks>
/// This attribute helps defend against cross-site request forgery. It won't prevent other forgery or tampering
/// attacks.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AppValidateAntiForgeryTokenAttribute : Attribute, IFilterFactory, IOrderedFilter
{
    /// <summary>
    /// Gets the order value for determining the order of execution of filters. Filters execute in
    /// ascending numeric value of the <see cref="Order"/> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Filters are executed in an ordering determined by an ascending sort of the <see cref="Order"/> property.
    /// </para>
    /// <para>
    /// The default Order for this attribute is 1000 because it must run after any filter which does authentication
    /// or login in order to allow them to behave as expected (ie Unauthenticated or Redirect instead of 400).
    /// </para>
    /// <para>
    /// Look at <see cref="IOrderedFilter.Order"/> for more detailed info.
    /// </para>
    /// </remarks>
    public int Order { get; set; } = 1000;

    /// <inheritdoc />
    public bool IsReusable => true;

    /// <inheritdoc />
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<ValidateAntiforgeryTokenAuthorizationFilter>();
    }
}

internal partial class ValidateAntiforgeryTokenAuthorizationFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
{
    private readonly IAntiforgery _antiforgery;
    private readonly ILogger _logger;

    public ValidateAntiforgeryTokenAuthorizationFilter(IAntiforgery antiforgery, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(antiforgery);

        _antiforgery = antiforgery;
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
        {
            Log.NotMostEffectiveFilter(_logger, typeof(IAntiforgeryPolicy));
            return;
        }

        if (ShouldValidate(context))
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException exception)
            {
                Log.AntiforgeryTokenInvalid(_logger, exception.Message, exception);
                context.Result = new AntiforgeryValidationFailedResult();
            }
        }
    }

    protected virtual bool ShouldValidate(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return true;
    }

    private static partial class Log
    {
        [LoggerMessage(1, LogLevel.Information, "Antiforgery token validation failed. {Message}", EventName = "AntiforgeryTokenInvalid")]
        public static partial void AntiforgeryTokenInvalid(ILogger logger, string message, Exception exception);

        [LoggerMessage(2, LogLevel.Trace, "Skipping the execution of current filter as its not the most effective filter implementing the policy {FilterPolicy}.", EventName = "NotMostEffectiveFilter")]
        public static partial void NotMostEffectiveFilter(ILogger logger, Type filterPolicy);
    }
}

/// <summary>
/// A <see cref="StatusCodes.Status403Forbidden"/> used for antiforgery validation
/// failures. Use <see cref="IAntiforgeryValidationFailedResult"/> to
/// match for validation failures inside MVC result filters.
/// </summary>
public class AntiforgeryValidationFailedResult : StatusCodeResult, IAntiforgeryValidationFailedResult
{
    private const int DefaultStatusCode = StatusCodes.Status403Forbidden;

    /// <summary>
    /// Creates a new <see cref="AntiforgeryValidationFailedResult"/> instance.
    /// </summary>
    public AntiforgeryValidationFailedResult()
        : base(DefaultStatusCode)
    {
    }
}