namespace CarPark.TimeZones.Providers;

public class TzInfoServiceException : Exception
{
    /// <summary>
    /// Creates exception with the provided message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TzInfoServiceException(string message) : base(message)
    { }
}