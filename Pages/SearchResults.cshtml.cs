using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using FridayFilmClub.Models; // Reference the centralized Movie model

public class SearchResultsModel : PageModel
{
    private readonly HttpClient _httpClient;

    public SearchResultsModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public List<Movie> Movies { get; set; }

    public async Task<IActionResult> OnGetAsync(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return Page();
        }

        var apiKey = "b3f20ef4";
        var url = $"http://www.omdbapi.com/?s={query}&apikey={apiKey}";

        var response = await _httpClient.GetStringAsync(url);
        var movieData = JsonConvert.DeserializeObject<MovieSearchResponse>(response);

        if (movieData?.Search != null)
        {
            Movies = movieData.Search;
        }

        return Page();
    }
}

public class MovieSearchResponse
{
    public List<Movie> Search { get; set; }
}
