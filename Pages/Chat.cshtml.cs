using FridayFilmClub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using System.IO;

namespace FridayFilmClub.Pages
{
    public class ChatModel : PageModel
    {
        private readonly string _connectionString;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public ChatModel(IConfiguration configuration, ICompositeViewEngine viewEngine, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _viewEngine = viewEngine;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        public List<ChatMessage> ChatMessages { get; set; } = new();

        [BindProperty]
        public string ChatMessage { get; set; }

        private bool IsUserAdmin()
        {
            return HttpContext.Session.GetInt32("IsAdmin") == 1;
        }

        public IActionResult OnGet()
        {
            if (!HttpContext.Session.TryGetValue("UserID", out _))
            {
                return RedirectToPage("/Login");
            }

            LoadMessages();
            return Page();
        }
        public async Task<IActionResult> OnGetRefresh()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT MessageID, Username, Message, Timestamp FROM ChatMessages ORDER BY Timestamp DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            List<string> messages = new List<string>();
                            while (await reader.ReadAsync())
                            {
                                string username = reader["Username"].ToString();
                                string message = reader["Message"].ToString();
                                string timestamp = Convert.ToDateTime(reader["Timestamp"]).ToString("g");
                                int messageId = Convert.ToInt32(reader["MessageID"]);

                                // Add the message HTML and delete button if the user is an admin
                                messages.Add($@"
                                    <p id='message-{messageId}'>
                                        <strong>{username}</strong> ({timestamp}): {message}
                                        { (HttpContext.Session.GetInt32("IsAdmin") == 1 ? 
                                            $"<form method='post' asp-page-handler='DeleteMessage' asp-route-messageId='{messageId}' style='display:inline;'>" +
                                            "<button type='submit' class='btn btn-danger btn-sm ml-2'>Delete</button></form>" : "") }
                                    </p>");
                            }
                            return Content(string.Join("", messages), "text/html");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content($"<p style='color: red;'>Error loading messages: {ex.Message}</p>", "text/html");
            }
        }
        public IActionResult OnPost()
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (!userID.HasValue)
            {
                return RedirectToPage("/Login");
            }

            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "Session error: Username is missing.");
                return Page();
            }

            // Check if the user is banned before allowing them to post a message
            using (var connection = new SqlConnection(_connectionString))
            {
                var checkBanCommand = new SqlCommand("SELECT IsBanned FROM Users WHERE Username = @Username", connection);
                checkBanCommand.Parameters.AddWithValue("@Username", username);
                connection.Open();

                var isBanned = (bool)checkBanCommand.ExecuteScalar();
                if (isBanned)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been banned. You cannot send messages.");
                    return Page();
                }
            }

            // If not banned, proceed with posting the message
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "INSERT INTO ChatMessages (UserID, Username, Message) VALUES (@UserID, @Username, @Message)",
                    connection);
                command.Parameters.AddWithValue("@UserID", userID.Value);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Message", ChatMessage);

                connection.Open();
                command.ExecuteNonQuery();
            }

            LoadMessages();
            return Page();
        }


        public IActionResult OnPostDeleteMessage(int messageId)
        {
            if (!IsUserAdmin())
            {
                return Forbid();
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("DELETE FROM ChatMessages WHERE MessageID = @MessageID", connection);
                    command.Parameters.AddWithValue("@MessageID", messageId);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        ModelState.AddModelError("", "No message found with the provided ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting message: " + ex.Message);
                return Page();
            }

            return RedirectToPage(); // Reload the page after deletion
        }

        public async Task<IActionResult> OnPostDeleteAll()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string query = "DELETE FROM ChatMessages"; // Ensure this is a DELETE query
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting messages: " + ex.Message);
                return Page();
            }
        }
        private void LoadMessages()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM ChatMessages ORDER BY Timestamp ASC", connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ChatMessages.Add(new ChatMessage
                        {
                            MessageID = (int)reader["MessageID"],  // Ensure this is set properly
                            Username = reader["Username"].ToString(),
                            Message = reader["Message"].ToString(),
                            Timestamp = (DateTime)reader["Timestamp"]
                        });
                    }
                }
            }
        }


        public IActionResult OnPostBanUser(string username)
        {
            if (!IsUserAdmin())
            {
                return Forbid();
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("UPDATE Users SET IsBanned = 1 WHERE Username = @Username", connection);
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        ModelState.AddModelError("", "No user found with the provided username.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error banning user: " + ex.Message);
                return Page();
            }

            return RedirectToPage(); // Reload the page after banning the user
        }

    }

    public class ChatMessage
    {
        public int MessageID { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
