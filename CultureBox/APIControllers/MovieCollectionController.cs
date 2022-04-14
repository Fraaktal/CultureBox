using System.Collections.Generic;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieCollectionController : ControllerBase
    {
        private readonly IUserDAO _userDao;
        private readonly IMovieCollectionDAO _movieCollectionDao;
        private readonly IMovieDAO _movieDao;

        public MovieCollectionController(IUserDAO userDao, IMovieCollectionDAO movieCollectionDao, IMovieDAO movieDao)
        {
            _userDao = userDao;
            _movieCollectionDao = movieCollectionDao;
            _movieDao = movieDao;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiMovieCollection>> GetAllCollection([FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _movieCollectionDao.GetAllCollection(userId);
                return Ok(res);
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiMovieCollection> GetCollectionById(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _movieCollectionDao.GetCollectionById(userId, id);
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
        public ActionResult<ApiMovieCollection> CreateCollection([FromBody] ApiCollectionRequest req)
        {
            if (req == null)
            {
                return BadRequest("EMPTY_PARAMETERS");
            }

            int userId = GetUserId(req.ApiKey);

            if (userId != -1)
            {
                if (!string.IsNullOrEmpty(req.Name))
                {
                    var res = _movieCollectionDao.CreateCollection(req.Name, userId);

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
                bool res = _movieCollectionDao.DeleteCollection(userId, id);
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
        public ActionResult<ApiMovieCollection> AddMovieToCollection(int id, [FromBody] ApiCollectionItemRequest req)
        {
            int userId = GetUserId(req.ApiKey);
            if (userId != -1)
            {
                var Movie = _movieDao.GetMovieById(req.ObjectId);

                if (Movie != null)
                {
                    var collection = _movieCollectionDao.AddMovieToCollection(userId, id, Movie);
                    if (collection != null)
                    {
                        return Ok(collection);
                    }

                    return NotFound("COLLECTION_NOT_FOUND");
                }

                return NotFound("Movie_NOT_FOUND");
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpDelete("/{id}/{MovieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiMovieCollection> RemoveMovieFromCollection(int id, int MovieId, [FromBody] string apiKey)
        {
            int userId = GetUserId(apiKey);
            if (userId != -1)
            {
                var collection = _movieCollectionDao.GetCollectionById(userId, id);
                if (collection != null)
                {
                    collection = _movieCollectionDao.RemoveMovieFromCollection(collection, MovieId, out bool res);
                    if (res)
                    {
                        return Ok(collection);
                    }

                    return NotFound("Movie_NOT_FOUND");
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
