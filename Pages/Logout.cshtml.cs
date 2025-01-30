using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FridayFilmClub.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear(); // Clear session
            return RedirectToPage("/Index"); // Redirect to home page
        }
    }
}
