using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

// This is the response cache for the error page
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
// This is the antiforgery token for the error page
[IgnoreAntiforgeryToken]

/// <summary>
/// This class is the model for the error page
/// </summary>
public class ErrorModel : PageModel
{
    /// <summary>
    /// Request ID for the error
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Whether to show the request ID
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <summary>
    /// Logger for the error page
    /// </summary>
    private readonly ILogger<ErrorModel> _logger;

    /// <summary>
    /// Constructor for the error model
    /// </summary>
    /// <param name="logger">Logger for the error page</param>
    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Action to perform on get request for the error page model class
    /// </summary>
    public void OnGet()
    {
        // Get the request ID
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

