using System.Collections.Generic;
using CultureBox.Control;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IApiBookController _apiBookController;

        public BookController(IApiBookController apiBookController)
        {
            _apiBookController = apiBookController;
        }

        //todo pagination
        [HttpGet]
        public ActionResult<List<ApiBook>> GetAll()
        {
            var books = _apiBookController.GetAllBooks();
            if (books == null)
            {
                return Problem("BOOKS_NOT_FOUND");
            }

            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiBook> GetBookById(int id)
        {
            var res = _apiBookController.GetBookById(id);
            if (res != null)
            {
                return Ok(res);
            }

            return NotFound("BOOKS_NOT_FOUND");
        }

        [HttpGet("search")]
        public ActionResult<List<ApiBook>> SearchBook([FromBody] ApiRequestBook b)
        {
            var res = _apiBookController.Search(b);
            if (res != null)
            {
                return Ok(res);
            }

            return BadRequest("INVALID_PARAMETERS");
        }
    }

    public class ApiRequestBook
    {
        public string Title { get; set; }
    }
}
