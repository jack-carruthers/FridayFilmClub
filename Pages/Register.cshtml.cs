using FridayFilmClub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FridayFilmClub.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AccountService _accountService;
    
        public RegisterModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public string ErrorMessage { get; set; }

        public IActionResult OnPost(string username, string email, string password, string confirmPassword)
        {
            // Validate passwords match
            if (password != confirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            // Register the user
            bool success = _accountService.RegisterUser(username, email, password);

            if (success)
            {
                return RedirectToPage("/Login"); // Redirect to login page after successful registration
            }

            // If registration fails, show an error message
            ErrorMessage = "The username is already taken. Please choose a different one.";
            return Page();
        }
    }
}
