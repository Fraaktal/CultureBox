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
        private readonly IBookDAO _bookDAO;
        private readonly IApiBookController _apiBookController;

        public BookController(IBookDAO bookDAO, IApiBookController apiBookController)
        {
            _bookDAO = bookDAO;
            _apiBookController = apiBookController;
        }

        //pagination
        [HttpGet]
        public ActionResult<List<ApiBook>> Get()
        {
            var books = _bookDAO.GetAllBooks();
            if (books == null)
            {
                return Problem("BOOKS_NOT_FOUND");
            }

            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<string> GetBookById(int id)
        {
            var res = _bookDAO.GetBookById(id);
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
