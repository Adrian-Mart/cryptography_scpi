using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

public class SaveKeysModel : PageModel
{
    public IFormFile PublicKeyFile { get; set; } = null!;
    public IFormFile PrivateKeyFile { get; set; } = null!;
    private readonly ApplicationDbContext _context;
    private string privateKey, publicKey;
    private int sessionId = 0;

    public SaveKeysModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
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

        // Generate the keys
        Sessions.SessionsList[sessionId].GenerateKeys();
        (publicKey, privateKey) = Sessions.SessionsList[sessionId].SaveKeys();
    }

    public FileResult OnGetDownloadFile(string fileName, bool privateKey)
    {
        string content = privateKey ? this.privateKey : publicKey;
        var byteArray = Encoding.UTF8.GetBytes(content);
        return File(byteArray, "application/octet-stream", fileName);
    }

    public IActionResult OnPost()
    {
        return Redirect($"/comunicacion?sessionId={sessionId}");
    }

}