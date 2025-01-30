namespace FridayFilmClub.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public bool PaidSubscription { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
    }
}
