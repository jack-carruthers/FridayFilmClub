using Newtonsoft.Json;
using FridayFilmClub.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FridayFilmClub.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "b3f20ef4"; // Your OMDB API Key

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Movie>> GetFeaturedFilms(int count)
        {
            var featuredMovies = new List<Movie>();

            // Example IMDB IDs (replace with dynamic logic if needed)
            var movieIds = new[] { "tt0111161", "tt0068646", "tt0071562", "tt1013752", "tt0463985", "tt0829482", "tt2005151", "tt0137523", "tt0080684", "tt0816692", "tt0088763", "tt0110912"}; // Add more IDs
            foreach (var id in movieIds)
            {
                var url = $"http://www.omdbapi.com/?i={id}&apikey={ApiKey}";
                var response = await _httpClient.GetStringAsync(url);
                var movie = JsonConvert.DeserializeObject<Movie>(response);

                if (movie != null)
                {
                    featuredMovies.Add(movie);
                }

                if (featuredMovies.Count >= count)
                {
                    break;
                }
            }

            return featuredMovies;
        }
    }
}
