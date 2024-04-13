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

    private readonly ApplicationDbContext _context;
    private int sessionId = 0;

    public LoadKeysModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;

        var id = httpContextAccessor.HttpContext?.Request.Query["sessionId"];
        sessionId = int.Parse(id?.First() ?? throw new InvalidOperationException("No session found"));

        DatabaseController.Initialize(_context);
        // Check if the session is null
        if (Sessions.SessionsList[sessionId].CurrentSession == null)
        {
            // Redirect to the index page
            RedirectToPage("Index");
        }
    }

    public IActionResult OnPostLoadKeys()
    {
        return RedirectToPage("LoadKeys");
    }

    /// <summary>
    /// Creates a method called OnPost that receives the form the loadkeys page
    /// </summary>
    public IActionResult OnPost()
    {
        try
        {
            // Get the file content from the public and private key files
            var publicKey = new StreamReader(PublicKeyFile.OpenReadStream()).ReadToEnd();
            var privateKey = new StreamReader(PrivateKeyFile.OpenReadStream()).ReadToEnd();

            // Set the public and private keys to the session
            Sessions.SessionsList[sessionId].LoadKeys(publicKey, privateKey);


            return Redirect($"/comunicacion?sessionId={sessionId}");
        }
        catch (Exception)
        {
            return Redirect($"/loadKeys?sessionId={sessionId}");
        }
    }
}