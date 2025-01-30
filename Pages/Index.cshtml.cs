using FridayFilmClub.Models;
using FridayFilmClub.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FridayFilmClub.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MovieService _movieService;

        public IndexModel(MovieService movieService)
        {
            _movieService = movieService;
        }

        public List<Movie> FeaturedFilms { get; private set; }

        public async Task OnGetAsync()
        {
            FeaturedFilms = await _movieService.GetFeaturedFilms(12);
        }
    }
}
