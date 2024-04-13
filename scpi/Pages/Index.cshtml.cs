using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

/// <summary>
/// This class is the model for the index page
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// Database context
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Session ID of the current session
    /// </summary>
    private int sessionId = 0;

    /// <summary>
    /// Constructor for the index model
    /// </summary>
    /// <param name="context">Database context</param>
    public IndexModel(ApplicationDbContext context)
    {
        // Initialize the database controller
        _context = context;
        DatabaseController.Initialize(_context);
    }

    /// <summary>
    /// Creates a property of type string called username 
    /// and initializes it with an empty string
    /// </summary>
    [BindProperty]
    public string username { get; set; } = string.Empty;

    /// <summary>
    /// Creates a property of type string called password and initializes it with an empty string
    /// </summary>
    [BindProperty]
    public string password { get; set; } = string.Empty;

    /// <summary>
    /// Creates a property of type string called generatekeys and initializes it with an empty string
    /// </summary>
    [BindProperty]
    public string generatekeys { get; set; } = string.Empty;

    /// <summary>
    /// Creates a method called OnPost that receives the form the index page
    /// </summary>
    public IActionResult OnPost()
    {
        ///<summary>
        ///Compares if the username and password are not empty
        ///</summary>
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            SessionManager manager;
            if (!SessionManager.CreateSession(username, password, out manager))
            {
                return RedirectToPage("Index");
            }
            ///<summary>
            ///Checks if the generatekeys is on or off, if it is on it redirects to the saveKeys page, if it is off it redirects to the loadKeys page
            ///</summary>
            if (generatekeys == "on")
            {
                sessionId = Sessions.AddSession(manager);
                return Redirect($"/SaveKeys?sessionId={sessionId}");
            }
            else
            {
                sessionId = Sessions.AddSession(manager);
                ///Redirects to the loadKeys page
                return Redirect($"/loadKeys?sessionId={sessionId}");
            }
        }
        else
        {
            ///Redirects to the index page
            return RedirectToPage("Index");
        }
    }
}