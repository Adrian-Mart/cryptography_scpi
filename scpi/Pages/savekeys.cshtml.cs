using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

public class SaveKeysModel : PageModel
{
    public IFormFile PublicKeyFile { get; set; } = null!;
    public IFormFile PrivateKeyFile { get; set; } = null!;
    public IActionResult OnPost()
    {
        return RedirectToPage("comunicacion");
    }
    
}