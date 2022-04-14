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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApiSeries>> GetAll(int resCount = 20, int offset = 0)
        {
            var Seriess = _apiMovieSerieController.GetAllSeries(resCount, offset);
            return Ok(Seriess);
        }

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

        [HttpGet("Search")]
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