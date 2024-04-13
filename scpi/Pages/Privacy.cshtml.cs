using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

/// <summary>
/// This class is the model for the privacy page
/// </summary>
public class PrivacyModel : PageModel
{
    /// <summary>
    /// Logger for the privacy page
    /// </summary>
    private readonly ILogger<PrivacyModel> _logger;

    /// <summary>
    /// Constructor for the privacy model
    /// </summary>
    /// <param name="logger">Logger for the privacy page</param>
    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        // Initialize the logger
        _logger = logger;
    }
    
    /// <summary>
    /// Creates a method called OnGet that is called when the page is requested
    /// </summary>
    public void OnGet()
    {
    }
}

