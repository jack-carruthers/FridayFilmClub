using FridayFilmClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FridayFilmClub.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AccountService _accountService;

        public LoginModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        public bool LoginFailed { get; set; }

        public IActionResult OnPost(string username, string password)
        {
            var user = _accountService.LoginUser(username, password);

            if (user == null)
            {
                // If user is null, it means either username or password is incorrect
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }

            if (user.IsBanned)
            {
                // If the user is found but banned, show banned message
                ModelState.AddModelError(string.Empty, "Your account has been banned.");
                return Page();
            }

            // If the user is not banned, proceed with successful login
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("IsAdmin", user.IsAdmin ? 1 : 0);

            return RedirectToPage("/Index"); // Redirect to the home page
        }

    }
}