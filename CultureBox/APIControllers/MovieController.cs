using System.Collections.Generic;
using CultureBox.Control;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        //livres en commun
        private readonly IApiMovieSerieController _apiMovieSerieController;

        public MovieController(IApiMovieSerieController apiMovieSerieController)
        {
            _apiMovieSerieController = apiMovieSerieController;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApiMovie>> GetAll(int resultCount = 20, int offset = 0)
        {
            var movies = _apiMovieSerieController.GetAllMovies(resultCount, offset);
            return Ok(movies);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiMovie> GetMovieById(int id)
        {
            var res = _apiMovieSerieController.GetMovieById(id);
            if (res != null)
            {
                return Ok(res);
            }

            return NotFound("MOVIE_NOT_FOUND");
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiMovie>> SearchMovie(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("INVALID_PARAMETERS");
            }

            var res = _apiMovieSerieController.SearchMovie(title);
            return Ok(res);
        }
    }
}
