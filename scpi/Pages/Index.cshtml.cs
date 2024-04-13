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
    /// <returns>Redirects to the saveKeys page or the loadKeys page or the index page</returns>
    public IActionResult OnPost()
    {
        ///<summary>
        ///Compares if the username and password are not empty
        ///</summary>
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            // Creates a session manager
            SessionManager manager;
            if (!SessionManager.CreateSession(username, password, out manager))
            {
                ///Redirects to the index page if the session manager is not created
                return RedirectToPage("Index");
            }
            ///<summary>
            ///Checks if the generatekeys is on or off, if it is on it redirects to the saveKeys page, if it is off it redirects to the loadKeys page
            ///</summary>
            if (generatekeys == "on")
            {
                // And adds the session manager to the sessions list
                sessionId = Sessions.AddSession(manager);
                // Redirects to the saveKeys page with the session ID as a query parameter if the generatekeys is on
                return Redirect($"/SaveKeys?sessionId={sessionId}");
            }
            else
            {
                // Adds the session manager to the sessions list
                sessionId = Sessions.AddSession(manager);
                ///Redirects to the loadKeys page with the session ID as a query parameter if the generatekeys is off
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