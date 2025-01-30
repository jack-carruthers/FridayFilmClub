using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using FridayFilmClub.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace FridayFilmClub.Pages
{
    public class MovieDetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _connectionString = "Server=localhost;Database=FridayFilmClub;User Id=sa;Password=saroot;TrustServerCertificate=True;";

        public MovieDetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Movie Movie { get; set; }
        public bool IsUserLoggedIn { get; private set; }
        public string ErrorMessage { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var apiKey = "b3f20ef4"; 
            var url = $"http://www.omdbapi.com/?i={id}&apikey={apiKey}";

            var response = await _httpClient.GetStringAsync(url);
            var movieData = JsonConvert.DeserializeObject<Movie>(response);

            // Check if the user is logged in based on session
            IsUserLoggedIn = HttpContext.Session.GetInt32("UserID") != null;

            if (movieData != null)
            {
                Movie = movieData;
                await LoadUserRatings(id);
            }

            return Page();
        }

        private async Task InitializeMovie(string movieId)
        {
            var apiKey = "b3f20ef4"; // Replace with your OMDB API key
            var url = $"http://www.omdbapi.com/?i={movieId}&apikey={apiKey}";

            var response = await _httpClient.GetStringAsync(url);
            Movie = JsonConvert.DeserializeObject<Movie>(response);
        }

        public async Task<IActionResult> OnPostAsync(string id, int rating)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                // If the user is not logged in, display an error message
                ErrorMessage = "Only logged-in users can rate movies.";
                IsUserLoggedIn = false;

                // Initialize the movie object to avoid null reference
                await InitializeMovie(id);
                await LoadUserRatings(id); // Reload movie details and ratings to keep the page consistent
                return Page();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    @"IF EXISTS (SELECT 1 FROM MovieRatings WHERE MovieID = @MovieID AND UserID = @UserID)
                    UPDATE MovieRatings SET Rating = @Rating WHERE MovieID = @MovieID AND UserID = @UserID
                    ELSE
                    INSERT INTO MovieRatings (MovieID, UserID, Rating) VALUES (@MovieID, @UserID, @Rating)", 
                    connection);

                command.Parameters.AddWithValue("@MovieID", id);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@Rating", rating);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }

            // Reload movie details and ratings after the update
            await InitializeMovie(id);
            await LoadUserRatings(id);

            return RedirectToPage(new { id });
        }

        private async Task LoadUserRatings(string movieId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Rating FROM MovieRatings WHERE MovieID = @MovieID", connection);
                command.Parameters.AddWithValue("@MovieID", movieId);

                connection.Open();
                var reader = await command.ExecuteReaderAsync();

                int totalStars = 0;
                int ratingCount = 0;

                while (await reader.ReadAsync())
                {
                    totalStars += reader.GetInt32(0);
                    ratingCount++;
                }

                Movie.AverageRating = ratingCount > 0 ? (double)totalStars / ratingCount : 0.0;
                Movie.RatingCount = ratingCount;
            }
        }
    }
}
