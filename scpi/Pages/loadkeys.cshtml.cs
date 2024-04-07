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
    /// Creates a method called OnPost that receives the form the loadkeys page
    /// </summary>
    public IActionResult OnPost()
    {
        ///<summary>
        ///Redirects to the communication page
        return RedirectToPage("comunicacion");
    }
}