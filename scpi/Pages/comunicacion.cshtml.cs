using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;
/// <summary>
/// Creates a class called ComunicationModel that is a PageModel
/// </summary>
public class ComunicationModel : PageModel
{
    
    [BindProperty]
    public string mensaje { get; set; } = String.Empty;

    [BindProperty]
    public string action { get; set; } = String.Empty;

    public IActionResult OnPost()
    {
        switch (action)
        {
            case "Salir":
                // Código para salir
                return RedirectToPage("Index");
        }
        return Page();
    }
}