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

        /// <summary>
        /// Get all loan requests
        /// </summary>
        /// <param name="request">RequestType: enumeration, can be: Borrow(0) the objets that you borrowed, Loan(1) the objects that you lent, All(2) all requests.
        /// ApiKey: your apiKey.</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ApiLoanRequest>> GetAllRequests([FromBody] LoanSearchRequest request)
        {
            int id = _userDao.GetUserId(request.ApiKey);
            if (id != -1)
            {
                var requests = _loanRequestControllerDAO.GetAllRequests(request.RequestType, id);
                return Ok(requests);
            }
            else
            {
                return BadRequest("INVALID_CREDENTIALS");
            }
        }

        /// <summary>
        /// Get the loan request corresponding to the given id.
        /// </summary>
        /// <param name="id">Id of the loan request.</param>
        /// <param name="apiKey">Your apiKey.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiLoanRequest> GetRequestById(int id, [FromBody] string apiKey)
        {
            int userId = _userDao.GetUserId(apiKey);
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

            return Ok(request);
        }

        /// <summary>
        /// Request a loan of the given object from the given user.
        /// </summary>
        /// <param name="request">IdUser: the id of the owner of the object.
        /// IdObject: the id of the object to borrow.
        /// ApiKey: your apiKey.
        /// RequestObjectType: enumeration, can be: Book (0), Movie(1) or Series(2).</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult RequestLoan([FromBody] LoanRequest request)
        {
            int idBorrower = _userDao.GetUserId(request.ApiKey);
            if (idBorrower == -1)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            if (idBorrower == request.IdUser)
            {
                return BadRequest("CANNOT_BORROW_TO_ONESELF");
            }

            switch (request.RequestObjectType)
            {
                case RequestObjectType.Book:
                    return RequestBookLoan(request, idBorrower);
                case RequestObjectType.Movie:
                    return RequestMovieLoan(request, idBorrower);
                case RequestObjectType.Series:
                    return RequestSeriesLoan(request, idBorrower);
                default:
                    return BadRequest("INVALID_OBJECT_TYPE");
            }
        }

        private ActionResult RequestBookLoan(LoanRequest request, int idBorrower)
        {
            var user = _userDao.GetUserById(request.IdUser);
            if (user == null)
            {
                return BadRequest("INVALID_USER_ID");
            }

            var collections = _bookCollectionDao.GetAllCollection(request.IdUser);

            if (collections.Any(c => c.Books.Any(b => b.Id == request.IdObject)))
            {
                bool isBorrowed = _loanRequestControllerDAO.IsBorrowed(request.IdObject, request.IdUser, RequestObjectType.Book);

                if (!isBorrowed)
                {
                    _loanRequestControllerDAO.RequestLoan(request.IdObject, request.IdUser, idBorrower, RequestObjectType.Book);
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
        
        private ActionResult RequestMovieLoan(LoanRequest request, int idBorrower)
        {
            var user = _userDao.GetUserById(request.IdUser);
            if (user == null)
            {
                return BadRequest("INVALID_USER_ID");
            }

            var collections = _movieCollectionDao.GetAllCollection(request.IdUser);

            if (collections.Any(c => c.Movies.Any(b => b.Id == request.IdObject)))
            {
                bool isBorrowed = _loanRequestControllerDAO.IsBorrowed(request.IdObject, request.IdUser, RequestObjectType.Movie);

                if (!isBorrowed)
                {
                    _loanRequestControllerDAO.RequestLoan(request.IdObject, request.IdUser, idBorrower, RequestObjectType.Movie);
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
        
        private ActionResult RequestSeriesLoan(LoanRequest request, int idBorrower)
        {
            var user = _userDao.GetUserById(request.IdUser);
            if (user == null)
            {
                return BadRequest("INVALID_USER_ID");
            }

            var collections = _seriesCollectionDao.GetAllCollection(request.IdUser);

            if (collections.Any(c => c.Series.Any(b => b.Id == request.IdObject)))
            {
                bool isBorrowed = _loanRequestControllerDAO.IsBorrowed(request.IdObject, request.IdUser, RequestObjectType.Series);

                if (!isBorrowed)
                {
                    _loanRequestControllerDAO.RequestLoan(request.IdObject, request.IdUser, idBorrower, RequestObjectType.Series);
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

        /// <summary>
        /// Update the current request it the loaner matches the given apiKey.
        /// </summary>
        /// <param name="id">The id of the request</param>
        /// <param name="request">RequestState: enumeration, can be: Accepted(0), Refused(1), Pending(2), Ongoing(3), Ended(4).
        /// ApiKey: your apiKey.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateLoanRequest(int id, [FromBody] ApiLoanRequestUpdate request)
        {
            var userId = _userDao.GetUserId(request.ApiKey);

            if (userId == -1)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            var loanRequest = _loanRequestControllerDAO.GetRequestById(id);
            if (loanRequest == null)
            {
                return NotFound();
            }

            if (loanRequest.IdOwner != userId)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            loanRequest.RequestState = request.RequestState;

            bool isOk = _loanRequestControllerDAO.UpdateLoanRequest(loanRequest);
            
            if (isOk)
            {
                return Ok();
            }
            else
            {
                return Problem("ERROR_ON_THE_SERVER");
            }
        }

        /// <summary>
        /// Search an object to borrow with the given title and given type.
        /// </summary>
        /// <param name="request">RequestObjectType: enumeration, can be: Book (0), Movie(1) or Series(2).
        /// Title: the title of the object.</param>
        /// <returns></returns>
        [HttpGet("searchObjectToBorrow")]
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
