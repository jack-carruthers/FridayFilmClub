using BCrypt.Net;
using FridayFilmClub.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FridayFilmClub.Services
{
    public class AccountService
    {
        private readonly string _connectionString;
        
        public AccountService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool RegisterUser(string username, string email, string password)
        {
            // Generate salt and hashed password using bcrypt
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            try
            {
                // Store the user in the database
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("INSERT INTO Users (Username, Email, Hashed_password, Salt) VALUES (@Username, @Email, @HashedPassword, @Salt)", connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                    command.Parameters.AddWithValue("@Salt", salt);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (SqlException ex)
            {
                // Check for unique constraint violation (SQL Server error code for unique constraint violation is 2627)
                if (ex.Number == 2627)
                {
                    return false; // Username already exists
                }

                throw; // Re-throw the exception for any other SQL errors
            }
        }

        public User? LoginUser(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var user = new User
                    {
                        UserID = (int)reader["UserID"],
                        Username = reader["Username"].ToString(),
                        Email = reader["Email"].ToString(),
                        HashedPassword = reader["Hashed_password"].ToString(),
                        Salt = reader["Salt"].ToString(),
                        PaidSubscription = (bool)reader["PaidSubscription"],
                        IsAdmin = (bool)reader["IsAdmin"],
                        IsBanned = (bool)reader["IsBanned"]
                    };

                    // Verify password
                    if (BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
                    {
                        return user; // Return user if password is correct and account is not banned
                    }
                }

                return null; // Invalid username or password
            }
        }
    }
}
