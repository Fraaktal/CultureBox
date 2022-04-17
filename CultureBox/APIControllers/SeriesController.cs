using System.Collections.Generic;
using CultureBox.Control;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeriesController : ControllerBase
    {
        //livres en commun
        private readonly IApiMovieSerieController _apiMovieSerieController;

        public SeriesController(IApiMovieSerieController apiMovieSerieController)
        {
            _apiMovieSerieController = apiMovieSerieController;
        }

        /// <summary>
        /// Get all the series in the database
        /// </summary>
        /// <param name="resultCount">Maximum number of results.</param>
        /// <param name="offset">Offset from the first result.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApiSeries>> GetAll(int resultCount = 20, int offset = 0)
        {
            var Seriess = _apiMovieSerieController.GetAllSeries(resultCount, offset);
            return Ok(Seriess);
        }

        /// <summary>
        /// Get the series corresponding to the given id.
        /// </summary>
        /// <param name="id">Id of the series.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiSeries> GetSeriesById(int id)
        {
            var res = _apiMovieSerieController.GetSeriesById(id);
            if (res != null)
            {
                return Ok(res);
            }

            return NotFound("Series_NOT_FOUND");
        }

        /// <summary>
        /// Search for series using the title.
        /// </summary>
        /// <param name="title">The title of the series researched.</param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiSeries>> SearchSeries(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("INVALID_PARAMETERS");
            }

            var res = _apiMovieSerieController.SearchSeries(title);
            return Ok(res);
        }
    }
}