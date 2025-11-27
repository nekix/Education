using FluentResults;

namespace CarPark.Errors;

public class WebApiError : Error
{
    public int StatusCode { get; }

    public string UserMessage { get; }

    public WebApiError(int statusCode, string userMessage)
    {
        StatusCode = statusCode;
        UserMessage = userMessage;
        Message = userMessage; // Set the base Message for FluentResults compatibility
    }
}