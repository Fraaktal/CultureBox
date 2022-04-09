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
        private readonly IBookDAO _bookDao;
        private readonly IUserDAO _userDao;
        private readonly ICollectionDAO _collectionDao;

        public LoanRequestController(ILoanRequestControllerDAO loanRequestControllerDAO, IBookDAO bookDao, 
            IUserDAO userDao, ICollectionDAO collectionDao)
        {
            _loanRequestControllerDAO = loanRequestControllerDAO;
            _bookDao = bookDao;
            _userDao = userDao;
            _collectionDao = collectionDao;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<LoanRequest>> GetAllRequests([FromBody] LoanSearchRequest br)
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
        public ActionResult<LoanRequest> GetRequestById(int id, [FromBody] string apiKey)
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
        public ActionResult RequestLoan([FromBody] LoanRequest br)
        {
            int idBorrower = _userDao.GetUserId(br.ApiKey);
            if (idBorrower == -1)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            var collections = _collectionDao.GetAllCollection(br.IdUser);

            if (collections == null)
            {
                return BadRequest("INVALID_USER_ID");
            } 

            if (collections.Any(c => c.Books.Any(b => b.Id == br.IdBook)))
            {
                bool isOk =_loanRequestControllerDAO.IsBorrowed(br.IdBook, br.IdUser);

                if (isOk)
                {
                    _loanRequestControllerDAO.RequestLoan(br.IdBook, br.IdUser, idBorrower);
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
    }

    public enum RequestType {Borrow, Loan, All}
    public class LoanSearchRequest
    {
        public RequestType RequestType { get; set; }
        public string ApiKey { get; set; }
    } 
    
    public class ApiLoanRequestUpdate
    {
        public RequestState RequestState { get; set; }
        public string ApiKey { get; set; }
    }

    public class LoanRequest
    {
        public int IdUser { get; set; }
        public int IdBook { get; set; }
        public string ApiKey { get; set; }
    }
}
