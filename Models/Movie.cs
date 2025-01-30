namespace FridayFilmClub.Models
{
    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Plot { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Writer { get; set; } // This was missing
        public string Rated { get; set; }
        public string Poster { get; set; }
        public string ImdbID { get; set; }
        public string Genre { get; set; }
        public double AverageRating { get; set; } // This was missing
        public int RatingCount { get; set; } // This was missing

        // Formats the average rating to one decimal place for display
        public string FormattedAverageRating => AverageRating.ToString("0.0"); // This was missing
    }
}
