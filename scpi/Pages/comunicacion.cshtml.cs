using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;
/// <summary>
/// Creates a class called ComunicationModel that is a PageModel
/// </summary>
public class ComunicationModel : PageModel
{
    [BindProperty]
    public string user { get; set; } = String.Empty;

    [BindProperty]
    public string message { get; set; } = String.Empty;

    [BindProperty]
    public string mensaje { get; set; } = String.Empty;

    [BindProperty]
    public string action { get; set; } = String.Empty;
    [BindProperty]
    public bool otherAvailable { get; set; } = false;
    private int sessionId = 0;

    public ComunicationModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        DatabaseController.Initialize(context);

        var id = httpContextAccessor.HttpContext?.Request.Query["sessionId"];
        sessionId = int.Parse(id?.First() ?? throw new InvalidOperationException("No session found"));

        // Check if the session is null
        if (Sessions.SessionsList[sessionId].CurrentSession == null)
        {
            // Redirect to the index page
            RedirectToPage("Index");
        }

        otherAvailable = Sessions.SessionsList[sessionId].SetOtherUser();
        user = Sessions.SessionsList[sessionId].CurrentSession!.Other?.user ?? "[Sin usuario]";
        Sessions.SessionsList[sessionId].ShareSymmetricKey();


    }

    public IActionResult OnPost()
    {
        switch (action)
        {
            case "Enviar":
                if (otherAvailable)
                {
                    Sessions.SessionsList[sessionId].WriteMessage(mensaje);
                }
                return Redirect($"/cominicacion?sessionId={sessionId}");
            case "Actualizar":
                if (otherAvailable)
                {
                    message = Sessions.SessionsList[sessionId].CurrentSession?.ReadMessage() ?? "[Sin mensajes]";
                }
                else
                {
                    otherAvailable = Sessions.SessionsList[sessionId].SetOtherUser();
                    user = Sessions.SessionsList[sessionId].CurrentSession!.Other?.user ?? "[Sin usuario]";
                    Sessions.SessionsList[sessionId].ShareSymmetricKey();
                }
                return Redirect($"/cominicacion?sessionId={sessionId}");
            case "Salir":
                return Redirect($"/Index?sessionId={sessionId}");
            default:
                return Redirect($"/cominicacion?sessionId={sessionId}");
        }
    }
}