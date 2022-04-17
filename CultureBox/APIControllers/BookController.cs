using System.Collections.Generic;
using CultureBox.Control;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        //livres en commun
        private readonly IApiBookController _apiBookController;

        public BookController(IApiBookController apiBookController)
        {
            _apiBookController = apiBookController;
        }

        /// <summary>
        /// Get all the books in the database
        /// </summary>
        /// <param name="resultCount">Maximum number of results.</param>
        /// <param name="offset">Offset from the first result.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApiBook>> GetAll(int resultCount = 20, int offset = 0)
        {
            var books = _apiBookController.GetAllBooks(resultCount, offset);
            return Ok(books);
        }

        /// <summary>
        /// Get the book corresponding to the given id.
        /// </summary>
        /// <param name="id">Id of the book.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiBook> GetBookById(int id)
        {
            var res = _apiBookController.GetBookById(id);
            if (res != null)
            {
                return Ok(res);
            }

            return NotFound("BOOKS_NOT_FOUND");
        }

        /// <summary>
        /// Search for books using the title.
        /// </summary>
        /// <param name="title">The title of the book researched.</param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiBook>> SearchBook(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("INVALID_PARAMETERS");
            }
            
            var res = _apiBookController.Search(title);
            return Ok(res);
        }
    }
}
