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

        if (user == "[Sin usuario]")
        {
            // Redirect to the index page
            RedirectToPage("Index");
        }
        else Sessions.SessionsList[sessionId].ShareSymmetricKey();

        try
        {
            var m = httpContextAccessor.HttpContext?.Request.Query["m"].First() ?? "";
            if (!string.IsNullOrEmpty(m))
            {
                message = MessageManager.Decipher(m, Sessions.SessionsList[sessionId].CurrentSession!);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            var t = httpContextAccessor.HttpContext?.Request.Query["t"].First() ?? "";
            var s = httpContextAccessor.HttpContext?.Request.Query["s"].First() ?? "";

            if (!string.IsNullOrEmpty(t) && !string.IsNullOrEmpty(s))
            {
                DatabaseController.AddMessage(Sessions.SessionsList[sessionId].CurrentSession!.User,
                    Sessions.SessionsList[sessionId].CurrentSession!.Other!, t, s);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    public IActionResult OnPost()
    {
        switch (action)
        {
            case "Actualizar":
                if (otherAvailable)
                {
                    message = Sessions.SessionsList[sessionId].CurrentSession?.ReadCipher() ?? "[Sin mensajes]";
                }
                return Redirect($"/cominicacion?sessionId={sessionId}&m={message}");
            case "Salir":
                return Redirect($"/Index?sessionId={sessionId}");
            default:
                var m = Sessions.SessionsList[sessionId].GetCipher(mensaje);
                return Redirect($"/cominicacion?sessionId={sessionId}&t={m.Text}&s={m.Signature}");
        }
    }
}