using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;
/// <summary>
/// Creates a class called LoadKeysModel that is a PageModel
/// </summary>
public class LoadKeysModel : PageModel
{
    /// <summary>
    /// The public key file used to encrypt the session.
    /// </summary>
    [BindProperty]
    public IFormFile PublicKeyFile { get; set; } = null!;

    /// <summary>
    /// The private key file used to decrypt the session.
    /// </summary>
    [BindProperty]
    public IFormFile PrivateKeyFile { get; set; } = null!;

    /// <summary>
    /// Database context
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Session ID of the current session
    /// </summary>
    private int sessionId = 0;

    /// <summary>
    /// Constructor for the load keys model
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public LoadKeysModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        // Initialize the database controller
        _context = context;

        // Get the session ID from the query and check if the session is null
        var id = httpContextAccessor.HttpContext?.Request.Query["sessionId"];
        // Parse the session ID from the query or throw an exception
        sessionId = int.Parse(id?.First() ?? throw new InvalidOperationException("No session found"));

        DatabaseController.Initialize(_context);
        // Check if the session is null
        if (Sessions.SessionsList[sessionId].CurrentSession == null)
        {
            // Redirect to the index page
            RedirectToPage("Index");
        }
    }

    /// <summary>
    /// Creates a method called OnPostLoadKeys that redirects to the loadkeys page
    /// </summary>
    public IActionResult OnPostLoadKeys()
    {
        // Redirect to the loadkeys page
        return RedirectToPage("LoadKeys");
    }

    /// <summary>
    /// Creates a method called OnPost that receives the form the loadkeys page
    /// </summary>
    /// <returns>Redirects to the communication page or the loadkeys page</returns>
    public IActionResult OnPost()
    {
        try
        {
            // Get the file content from the public and private key files
            var publicKey = new StreamReader(PublicKeyFile.OpenReadStream()).ReadToEnd();
            var privateKey = new StreamReader(PrivateKeyFile.OpenReadStream()).ReadToEnd();

            // Set the public and private keys to the session
            Sessions.SessionsList[sessionId].LoadKeys(publicKey, privateKey);

            // Redirect to the communication page with the session ID in the query
            return Redirect($"/comunicacion?sessionId={sessionId}");
        }
        catch (Exception)
        {
            // Redirect to the loadkeys page with the session ID in the query
            return Redirect($"/loadKeys?sessionId={sessionId}");
        }
    }
}