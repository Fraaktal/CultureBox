using System.Collections.Generic;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookCollectionController : ControllerBase
    {
        private readonly IUserDAO _userDao;
        private readonly IBookCollectionDAO _bookCollectionDao;
        private readonly IBookDAO _bookDao;

        public BookCollectionController(IUserDAO userDao, IBookCollectionDAO bookCollectionDao, IBookDAO bookDao)
        {
            _userDao = userDao;
            _bookCollectionDao = bookCollectionDao;
            _bookDao = bookDao;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiBookCollection>> GetAllCollection([FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _bookCollectionDao.GetAllCollection(userId);
                return Ok(res);
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiBookCollection> GetCollectionById(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _bookCollectionDao.GetCollectionById(userId, id);
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
        public ActionResult<ApiBookCollection> CreateCollection([FromBody] ApiCollectionRequest req)
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
                    var res = _bookCollectionDao.CreateCollection(req.Name, userId);

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
                bool res = _bookCollectionDao.DeleteCollection(userId, id);
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
        public ActionResult<ApiBookCollection> AddBookToCollection(int id, [FromBody] ApiCollectionItemRequest req)
        {
            int userId = GetUserId(req.ApiKey);
            if (userId != -1)
            {
                var book = _bookDao.GetBookById(req.ObjectId);

                if (book != null)
                {
                    var collection = _bookCollectionDao.AddBookToCollection(userId, id, book);
                    if (collection != null)
                    {
                        return Ok(collection);
                    }

                    return NotFound("COLLECTION_NOT_FOUND");
                }

                return NotFound("BOOK_NOT_FOUND");
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpDelete("/{id}/{bookId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiBookCollection> RemoveBookFromCollection(int id, int bookId, [FromBody] string apiKey)
        {
            int userId = GetUserId(apiKey);
            if (userId != -1)
            {
                var collection = _bookCollectionDao.GetCollectionById(userId, id);
                if (collection != null)
                {
                    collection = _bookCollectionDao.RemoveBookFromCollection(collection, bookId, out bool res);
                    if (res)
                    {
                        return Ok(collection);
                    }

                    return NotFound("BOOK_NOT_FOUND");
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
