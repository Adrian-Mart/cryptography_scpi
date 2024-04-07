using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace scpi.Pages;

/// <summary>
/// Creates a class called IndexModel that is a PageModel
/// </summary>
public class IndexModel : PageModel
{
    /// <summary>
    /// Creates a property of type string called username and initializes it with an empty string
    /// </summary>
    [BindProperty]
    public string username { get; set; } = string.Empty;

    /// <summary>
    /// Creates a property of type string called password and initializes it with an empty string
    /// </summary>
    [BindProperty]
    public string password { get; set; } = string.Empty;

    /// <summary>
    /// Creates a property of type string called generatekeys and initializes it with an empty string
    /// </summary>
    [BindProperty]
    public string generatekeys { get; set; } = string.Empty;

    /// <summary>
    /// Creates a method called OnPost that receives the form the index page
    /// </summary>
    public IActionResult OnPost()
    {
        ///<summary>
        ///Compares if the username and password are not empty
        ///</summary>
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            ///<summary>
            ///Checks if the generatekeys is on or off, if it is on it redirects to the saveKeys page, if it is off it redirects to the loadKeys page
            ///</summary>
            if (generatekeys == "on")
            {
                ///<summary>
                ///Redirects to the saveKeys page
                ///</summary>
                return RedirectToPage("saveKeys");
            }
            else
            {
                ///Redirects to the loadKeys page
                return RedirectToPage("loadKeys");
            }   
        }else{
            ///Redirects to the index page
            return RedirectToPage("Index");
        }
    }
}