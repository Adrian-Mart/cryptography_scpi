using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

/// <summary>
/// This class is the model for the communication page
/// </summary>
public class ComunicationModel : PageModel
{
    /// <summary>
    /// User name of the receiver of the message
    /// </summary>
    [BindProperty]
    public string user { get; set; } = string.Empty;

    /// <summary>
    /// Message received from the other user
    /// </summary>
    [BindProperty]
    public string message { get; set; } = string.Empty;

    /// <summary>
    /// Message to send to the other user
    /// </summary>
    [BindProperty]
    public string mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Action to perform on submit
    /// </summary>
    [BindProperty]
    public string action { get; set; } = string.Empty;

    /// <summary>
    /// Whether another user is available
    /// </summary>
    [BindProperty]
    public bool otherAvailable { get; set; } = false;

    /// <summary>
    /// Session ID of the current session
    /// </summary>
    private int sessionId = 0;

    /// <summary>
    /// Constructor for the communication model
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public ComunicationModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        // Initialize the database controller
        DatabaseController.Initialize(context);

        // Get the session ID from the query
        var id = httpContextAccessor.HttpContext?.Request.Query["sessionId"];
        sessionId = int.Parse(id?.First() ?? throw new InvalidOperationException("No session found"));

        // Check if the session is null
        if (Sessions.SessionsList[sessionId].CurrentSession != null)
        {
            // Set the other user available
            otherAvailable = Sessions.SessionsList[sessionId].SetOtherUser();

            // If the other user is available
            if (otherAvailable)
            {
                // Set the user name
                user = Sessions.SessionsList[sessionId].CurrentSession!.Other?.user ?? "";
                // Share the symmetric key
                Sessions.SessionsList[sessionId].ShareSymmetricKey();

                // Try to read the message
                try
                {
                    // Get the message from the query
                    var u = httpContextAccessor.HttpContext?.Request.Query["u"].First() ?? "";

                    // If the message is not empty
                    if (!string.IsNullOrEmpty(u) && u == "True")
                    {
                        // Share the symmetric key
                        Sessions.SessionsList[sessionId].ShareSymmetricKey();
                        // Read the message
                        message = Sessions.SessionsList[sessionId].ReadMessage();
                    }
                }
                catch (Exception e)
                {
                    // Log the exception
                    Console.WriteLine(e.Message);
                }

                // Try to write the message
                try
                {
                    // Get the message from the query
                    var t = httpContextAccessor.HttpContext?.Request.Query["t"].First() ?? "";
                    // Get the signature from the query
                    var s = httpContextAccessor.HttpContext?.Request.Query["s"].First() ?? "";

                    // If the message and signature are not empty
                    if (!string.IsNullOrEmpty(t) && !string.IsNullOrEmpty(s))
                    {
                        // Replace the characters in the message and signature
                        // from non-URL characters to URL characters
                        t = t.Replace(")", "+").Replace("(", "/").Replace("-", "=");
                        s = s.Replace(")", "+").Replace("(", "/").Replace("-", "=");

                        // Write the message
                        MessageManager.WriteMessage(t, s, Sessions.SessionsList[sessionId].CurrentSession!);
                    }
                }
                catch (Exception e)
                {
                    // Log the exception
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    /// <summary>
    /// Method to handle the post request
    /// </summary>
    /// <returns>Redirect to the appropriate page</returns>
    public IActionResult OnPost()
    {
        // Switch on the action
        switch (action)
        {
            // On update action
            case "Actualizar":
                // Redirect to the communication page
                return Redirect($"/comunicacion?sessionId={sessionId}&u={otherAvailable}");
            // On logout action
            case "Salir":
                // Redirect to the index page
                return Redirect($"/Index?sessionId={sessionId}");
            // On send action
            default:
                // Get the message from the query
                var m = Sessions.SessionsList[sessionId].GetCipher(mensaje);
                // Replace the characters in the message and signature from URL characters
                // to non-URL characters to avoid errors
                m.Text = m.Text.Replace("+", ")").Replace("/", "(").Replace("=", "-");
                m.Signature = m.Signature.Replace("+", ")").Replace("/", "(").Replace("=", "-");

                // Redirect to the communication page
                return Redirect($"/comunicacion?sessionId={sessionId}&t={m.Text}&s={m.Signature}");
        }
    }
}