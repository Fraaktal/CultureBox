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

        /// <summary>
        /// Get all of the collection corresponding to the user linked to the apiKey.
        /// </summary>
        /// <param name="apiKey">Your apikey.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the collection corresponding to the given id if it's linked to the user corresponding to the apikey.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        /// <param name="apiKey">Your apikey.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Create a new collection with the given name using your apiKey.
        /// </summary>
        /// <param name="request">Name: The name of your collection.
        /// ApiKey: your apikey.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete the collection corresponding to the given id if it's linked to the user corresponding to the apikey.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        /// <param name="apiKey">Your apikey.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add a series the collection corresponding to the given id if it's linked to the user corresponding to the apikey.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        /// <param name="request">ObjectId: id of the series to add to the collection.
        /// ApiKey: your apikey.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete the series corresponding to the given series id from the collection corresponding to the given id if it's linked to the user corresponding to the apikey.
        /// </summary>
        /// <param name="id">Id of the collection.</param>
        /// <param name="request">ObjectId: id of the series to add to the collection.
        /// ApiKey: your apikey.</param>
        /// <returns></returns>
        [HttpDelete("{id}/DeleteSeries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiSeriesCollection> RemoveSeriesFromCollection(int id, [FromBody] ApiCollectionItemRequest request)
        {
            int userId = GetUserId(request.ApiKey);
            if (userId != -1)
            {
                var collection = _seriesCollectionDao.GetCollectionById(userId, id);
                if (collection != null)
                {
                    collection = _seriesCollectionDao.RemoveSeriesFromCollection(collection, request.ObjectId, out bool res);
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
