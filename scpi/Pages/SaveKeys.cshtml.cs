using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

public class SaveKeysModel : PageModel
{
    public IFormFile PublicKeyFile { get; set; } = null!;
    public IFormFile PrivateKeyFile { get; set; } = null!;
    private readonly ApplicationDbContext _context;
    [BindProperty]
    public int sessionId { get; private set; } = 0;
    private IHttpContextAccessor _httpContextAccessor;

    public SaveKeysModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        var id = _httpContextAccessor.HttpContext?.Request.Query["sessionId"];
        // Print the current url
        Console.WriteLine(_httpContextAccessor.HttpContext?.Request.GetDisplayUrl());
        sessionId = int.Parse(id?.First() ?? throw new InvalidOperationException("No session found"));
        DatabaseController.Initialize(_context);

        // Generate the keys
        if (!Sessions.SessionsList[sessionId].CurrentSession!.Generated)
            Sessions.SessionsList[sessionId].GenerateKeys();
    }

    public FileResult OnGetDownloadFile(string fileName, bool privateKey)
    {
        (string pub, string priv) = Sessions.SessionsList[sessionId].SaveKeys();

        string content = privateKey ? priv : pub;
        var byteArray = Encoding.UTF8.GetBytes(content);
        return File(byteArray, "application/octet-stream", fileName);
    }

    public IActionResult OnPost()
    {
        return Redirect($"/comunicacion?sessionId={sessionId}");
    }

}