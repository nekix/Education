namespace FrameworksEducation.AspNetCore.Chapter_13.Mvc.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}