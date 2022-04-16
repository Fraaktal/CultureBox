using System.Collections.Generic;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeriesCollectionController : ControllerBase
    {
        private readonly IUserDAO _userDao;
        private readonly ISeriesCollectionDAO _seriesCollectionDao;
        private readonly ISeriesDao _seriesDao;

        public SeriesCollectionController(IUserDAO userDao, ISeriesCollectionDAO seriesCollectionDao, ISeriesDao seriesDao)
        {
            _userDao = userDao;
            _seriesCollectionDao = seriesCollectionDao;
            _seriesDao = seriesDao;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiSeriesCollection>> GetAllCollection([FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _seriesCollectionDao.GetAllCollection(userId);
                return Ok(res);
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiSeriesCollection> GetCollectionById(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _seriesCollectionDao.GetCollectionById(userId, id);
                if (res != null)
                {
                    return Ok(res);
                }

                return NotFound("COLLECTION_NOT_FOUND");

            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiSeriesCollection> CreateCollection([FromBody] ApiCollectionRequest request)
        {
            if (request == null)
            {
                return BadRequest("EMPTY_PARAMETERS");
            }

            int userId = GetUserId(request.ApiKey);

            if (userId != -1)
            {
                if (!string.IsNullOrEmpty(request.Name))
                {
                    var res = _seriesCollectionDao.CreateCollection(request.Name, userId);

                    if (res != null)
                    {
                        return Ok(res);
                    }

                    return BadRequest("NAME_ALREADY_TAKEN");
                }

                return BadRequest("EMPTY_NAME");
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteCollection(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);

            if (userId != -1)
            {
                bool res = _seriesCollectionDao.DeleteCollection(userId, id);
                if (res)
                {
                    return Ok();
                }

                return NotFound("COLLECTION_NOT_FOUND");
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiSeriesCollection> AddSeriesToCollection(int id, [FromBody] ApiCollectionItemRequest request)
        {
            int userId = GetUserId(request.ApiKey);
            if (userId != -1)
            {
                var series = _seriesDao.GetSeriesById(request.ObjectId);

                if (series != null)
                {
                    var collection = _seriesCollectionDao.AddSeriesToCollection(userId, id, series);
                    if (collection != null)
                    {
                        return Ok(collection);
                    }

                    return NotFound("COLLECTION_NOT_FOUND");
                }

                return NotFound("SERIES_NOT_FOUND");
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpDelete("/{id}/{SeriesId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiSeriesCollection> RemoveSeriesFromCollection(int id, int SeriesId, [FromBody] string apiKey)
        {
            int userId = GetUserId(apiKey);
            if (userId != -1)
            {
                var collection = _seriesCollectionDao.GetCollectionById(userId, id);
                if (collection != null)
                {
                    collection = _seriesCollectionDao.RemoveSeriesFromCollection(collection, SeriesId, out bool res);
                    if (res)
                    {
                        return Ok(collection);
                    }

                    return NotFound("SERIES_NOT_FOUND");
                }

                return NotFound("COLLECTION_NOT_FOUND");
            }

            return BadRequest("INVALID_CREDENTIALS");
        }


        private int GetUserId(string apiKey)
        {
            int res = -1;

            if (!string.IsNullOrEmpty(apiKey))
            {
                res = _userDao.GetUserId(apiKey);
            }

            return res;
        }
    }
}
