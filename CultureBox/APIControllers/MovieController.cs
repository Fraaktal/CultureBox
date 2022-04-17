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
        private readonly IApiMovieSerieController _apiMovieSerieController;

        public MovieController(IApiMovieSerieController apiMovieSerieController)
        {
            _apiMovieSerieController = apiMovieSerieController;
        }

        /// <summary>
        /// Get all the movies in the database
        /// </summary>
        /// <param name="resultCount">Maximum number of results.</param>
        /// <param name="offset">Offset from the first result.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApiMovie>> GetAll(int resultCount = 20, int offset = 0)
        {
            var movies = _apiMovieSerieController.GetAllMovies(resultCount, offset);
            return Ok(movies);
        }

        /// <summary>
        /// Get the movie corresponding to the given id.
        /// </summary>
        /// <param name="id">Id of the movie.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Search for movies using the title.
        /// </summary>
        /// <param name="title">The title of the movie researched.</param>
        /// <returns></returns>
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
