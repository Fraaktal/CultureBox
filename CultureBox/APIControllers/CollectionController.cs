using System.Collections.Generic;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollectionController : ControllerBase
    {
        private readonly IUserDAO _userDao;
        private readonly ICollectionDAO _collectionDao;
        private readonly IBookDAO _bookDao;

        public CollectionController(IUserDAO userDao, ICollectionDAO collectionDAO, IBookDAO bookDao)
        {
            _userDao = userDao;
            _collectionDao = collectionDAO;
            _bookDao = bookDao;
        }

        [HttpGet]
        public ActionResult<List<ApiCollection>> GetAllCollection([FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _collectionDao.GetAllCollection(userId);
                if (res != null)
                {
                    return Ok(res);
                }

                return NotFound("COLLECTION_NOT_FOUND");

            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpGet("{id}")]
        public ActionResult<ApiCollection> GetCollectionById(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (userId != -1)
            {
                var res = _collectionDao.GetCollectionById(userId, id);
                if (res != null)
                {
                    return Ok(res);
                }

                return NotFound("COLLECTION_NOT_FOUND");

            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpPost]
        public ActionResult<ApiCollection> CreateCollection([FromBody] ApiCollectionRequest req)
        {
            int userId = GetUserId(req.ApiKey);

            if (userId != -1)
            {
                if (!string.IsNullOrEmpty(req.Name))
                {
                    var res = _collectionDao.CreateCollection(req.Name, userId);

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
        public ActionResult DeleteCollection(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);

            if (userId != -1)
            {
                bool res = _collectionDao.DeleteCollection(userId, id);
                if (res)
                {
                    return Ok();
                }

                return NotFound("COLLECTION_NOT_FOUND");
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpPost("{id}")]
        public ActionResult<ApiCollection> AddBookToCollection(int id, [FromBody] ApiCollectionItemRequest req)
        {
            int userId = GetUserId(req.ApiKey);
            if (userId != -1)
            {
                var book = _bookDao.GetBookById(req.BookId);

                if (book != null)
                {
                    var collection = _collectionDao.AddBookToCollection(userId, id, book);
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

        [HttpDelete("/{id}")]
        public ActionResult<ApiCollection> RemoveBookFromCollection(int id, [FromBody] ApiCollectionItemRequest req)
        {
            int userId = GetUserId(req.ApiKey);
            if (userId != -1)
            {
                var collection = _collectionDao.GetCollectionById(userId, id);
                if (collection != null)
                {
                    collection = _collectionDao.RemoveBookFromCollection(collection, req.BookId, out bool res);
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

    public class ApiCollectionRequest
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
    }
    
    public class ApiCollectionItemRequest
    {
        public int BookId { get; set; }
        public string ApiKey { get; set; }
    }
}
