using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

/// <summary>
/// This class is the model for the save keys page
/// </summary>
public class SaveKeysModel : PageModel
{
    /// <summary>
    /// The public key file used to encrypt the session.
    /// </summary>
    public IFormFile PublicKeyFile { get; set; } = null!;

    /// <summary>
    /// The private key file used to decrypt the session.
    /// </summary>
    public IFormFile PrivateKeyFile { get; set; } = null!;

    /// <summary>
    /// Database context
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Session ID of the current session
    /// </summary>
    [BindProperty]
    public int sessionId { get; private set; } = 0;

    /// <summary>
    /// HTTP context accessor for the current session
    /// </summary>    
    private IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Constructor for the save keys model
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public SaveKeysModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        // Initialize the database controller
        _context = context;
        // Initialize the HTTP context accessor
        _httpContextAccessor = httpContextAccessor;
        // Get the session ID from the query
        var id = _httpContextAccessor.HttpContext?.Request.Query["sessionId"];
        // Print the current url
        Console.WriteLine(_httpContextAccessor.HttpContext?.Request.GetDisplayUrl());
        // Parse the session ID from the query or throw an exception
        sessionId = int.Parse(id?.First() ?? throw new InvalidOperationException("No session found"));
        // Initialize the database controller
        DatabaseController.Initialize(_context);

        // Generate the keys if they have not been generated
        if (!Sessions.SessionsList[sessionId].CurrentSession!.Generated)
            Sessions.SessionsList[sessionId].GenerateKeys();
    }

    /// <summary>
    /// Creates a method called OnGetDownloadFile that downloads the keys
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="privateKey">Whether the key is private</param>
    /// <returns>File to download</returns>
    public FileResult OnGetDownloadFile(string fileName, bool privateKey)
    {
        // Save the keys
        (string pub, string priv) = Sessions.SessionsList[sessionId].SaveKeys();
        // Get the content of the key
        string content = privateKey ? priv : pub;
        // Get the byte array of the content
        var byteArray = Encoding.UTF8.GetBytes(content);
        // Return the file to download
        return File(byteArray, "application/octet-stream", fileName);
    }
    
    /// <summary>
    /// Creates a method called OnPost that receives the form the saveKeys page
    /// </summary>
    public IActionResult OnPost()
    {
        // Redirect to the communication page with the session ID
        return Redirect($"/comunicacion?sessionId={sessionId}");
    }

}