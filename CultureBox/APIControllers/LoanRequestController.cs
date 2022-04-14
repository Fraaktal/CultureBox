using System.Collections.Generic;
using System.Linq;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoanRequestController : ControllerBase
    {
        private readonly ILoanRequestControllerDAO _loanRequestControllerDAO;
        private readonly IUserDAO _userDao;
        private readonly IBookCollectionDAO _bookCollectionDao;
        private readonly IMovieCollectionDAO _movieCollectionDao;
        private readonly ISeriesCollectionDAO _seriesCollectionDao;

        public LoanRequestController(ILoanRequestControllerDAO loanRequestControllerDAO, IUserDAO userDao, 
            IBookCollectionDAO bookCollectionDao, IMovieCollectionDAO movieCollectionDao, 
            ISeriesCollectionDAO seriesCollectionDao)
        {
            _loanRequestControllerDAO = loanRequestControllerDAO;
            _userDao = userDao;
            _bookCollectionDao = bookCollectionDao;
            _movieCollectionDao = movieCollectionDao;
            _seriesCollectionDao = seriesCollectionDao;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiLoanRequest>> GetAllRequests([FromBody] LoanSearchRequest br)
        {
            int id = _userDao.GetUserId(br.ApiKey);
            if (id != -1)
            {
                var requests = _loanRequestControllerDAO.GetAllRequests(br.RequestType, id);
                return Ok(requests);
            }
            else
            {
                return BadRequest("INVALID_CREDENTIALS");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiLoanRequest> GetRequestById(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
            if (id == -1)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            var request = _loanRequestControllerDAO.GetRequestById(id);

            if (request == null)
            {
                return NotFound();
            }

            if (request.IdOwner != userId && request.IdRequester != userId)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            return Ok(request);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult RequestLoan([FromBody] LoanRequest lr)
        {
            int idBorrower = _userDao.GetUserId(lr.ApiKey);
            if (idBorrower == -1)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            if (idBorrower == lr.IdUser)
            {
                return BadRequest("CANNOT_BORROW_TO_ONESELF");
            }

            switch (lr.RequestObjectType)
            {
                case RequestObjectType.Book:
                    return RequestBookLoan(lr, idBorrower);
                case RequestObjectType.Movie:
                    return RequestMovieLoan(lr, idBorrower);
                case RequestObjectType.Series:
                    return RequestSeriesLoan(lr, idBorrower);
                default:
                    return BadRequest("INVALID_OBJECT_TYPE");
            }
        }

        private ActionResult RequestBookLoan(LoanRequest lr, int idBorrower)
        {
            var collections = _bookCollectionDao.GetAllCollection(lr.IdUser);

            if (collections == null)
            {
                return BadRequest("INVALID_USER_ID");
            }

            if (collections.Any(c => c.Books.Any(b => b.Id == lr.IdObject)))
            {
                bool isBorrowed = _loanRequestControllerDAO.IsBorrowed(lr.IdObject, lr.IdUser, RequestObjectType.Book);

                if (!isBorrowed)
                {
                    _loanRequestControllerDAO.RequestLoan(lr.IdObject, lr.IdUser, idBorrower, RequestObjectType.Book);
                    return Ok();
                }
                else
                {
                    return BadRequest("BOOK_ALREADY_BORROWED");
                }
            }
            else
            {
                return NotFound("MISSING_BOOK_IN_USER_COLLECTIONS");
            }
        }
        
        private ActionResult RequestMovieLoan(LoanRequest lr, int idBorrower)
        {
            var collections = _bookCollectionDao.GetAllCollection(lr.IdUser);

            if (collections == null)
            {
                return BadRequest("INVALID_USER_ID");
            }

            if (collections.Any(c => c.Books.Any(b => b.Id == lr.IdObject)))
            {
                bool isBorrowed = _loanRequestControllerDAO.IsBorrowed(lr.IdObject, lr.IdUser, RequestObjectType.Movie);

                if (!isBorrowed)
                {
                    _loanRequestControllerDAO.RequestLoan(lr.IdObject, lr.IdUser, idBorrower, RequestObjectType.Movie);
                    return Ok();
                }
                else
                {
                    return BadRequest("MOVIE_ALREADY_BORROWED");
                }
            }
            else
            {
                return NotFound("MISSING_MOVIE_IN_USER_COLLECTIONS");
            }
        }
        
        private ActionResult RequestSeriesLoan(LoanRequest lr, int idBorrower)
        {
            var collections = _seriesCollectionDao.GetAllCollection(lr.IdUser);

            if (collections == null)
            {
                return BadRequest("INVALID_USER_ID");
            }

            if (collections.Any(c => c.Series.Any(b => b.Id == lr.IdObject)))
            {
                bool isBorrowed = _loanRequestControllerDAO.IsBorrowed(lr.IdObject, lr.IdUser, RequestObjectType.Series);

                if (!isBorrowed)
                {
                    _loanRequestControllerDAO.RequestLoan(lr.IdObject, lr.IdUser, idBorrower, RequestObjectType.Series);
                    return Ok();
                }
                else
                {
                    return BadRequest("SERIES_ALREADY_BORROWED");
                }
            }
            else
            {
                return NotFound("MISSING_SERIES_IN_USER_COLLECTIONS");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateLoanRequest(int id, [FromBody] ApiLoanRequestUpdate lr)
        {
            var userId = _userDao.GetUserId(lr.ApiKey);

            if (userId == -1)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            var request = _loanRequestControllerDAO.GetRequestById(id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.IdOwner != userId && request.IdRequester != userId)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            request.RequestState = lr.RequestState;

            bool isOk = _loanRequestControllerDAO.UpdateLoanRequest(request);
            
            if (isOk)
            {
                return Ok();
            }
            else
            {
                return Problem("ERROR_ON_THE_SERVER");
            }
        }
        
        [HttpGet("SearchBookToBorrow")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ApiObjectToBorrow>> SearchObjectToBorrow([FromBody] SearchObjectToBorrowRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                return BadRequest("INVALID_PARAMETERS");
            }

            List<ApiObjectToBorrow> apiObjectToBorrow = new List<ApiObjectToBorrow>();
            switch (request.RequestObjectType)
            {
                case RequestObjectType.Book:
                    apiObjectToBorrow = _bookCollectionDao.SearchBook(request.Title);
                    break;
                case RequestObjectType.Movie:
                    apiObjectToBorrow = _movieCollectionDao.SearchMovie(request.Title);
                    break;
                case RequestObjectType.Series:
                    apiObjectToBorrow = _seriesCollectionDao.SearchSeries(request.Title);
                    break;
            }

            if (apiObjectToBorrow.Count == 0)
            {
                return NotFound();
            }

            return Ok(apiObjectToBorrow);
        }
    }
}
