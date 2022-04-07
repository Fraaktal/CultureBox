using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CultureBox.APIControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserDAO _userDao;
        private readonly ICollectionDAO _collectionDao;

        public UserController(IUserDAO userDAO, ICollectionDAO collectionDAO)
        {
            _userDao = userDAO;
            _collectionDao = collectionDAO;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiUser> Get(int id)
        {
            var user = _userDao.GetUserById(id);
            if (user == null)
            {
                return NotFound("USER_NOT_FOUND");
            }

            return Ok(user);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiUser> GetAllUser()
        {
            var users = _userDao.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("apikey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> GetApiKey([FromBody] APIRequestUser u)
        {
            var apiKey = _userDao.GetApiKey(u.Username, u.Password);
            if (!string.IsNullOrEmpty(apiKey))
            {
                return Ok(apiKey);
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiUser> CreateUser([FromBody] APIRequestUser u)
        {
            if (string.IsNullOrEmpty(u.Username) && string.IsNullOrEmpty(u.Password))
            {
                return BadRequest("MISSING_USERNAME_AND_PASSWORD");
            }

            if (string.IsNullOrEmpty(u.Username))
            {
                return BadRequest("MISSING_USERNAME");
            }

            if (string.IsNullOrEmpty(u.Password))
            {
                return BadRequest("MISSING_PASSWORD");
            }

            var user = _userDao.CreateUser(u.Username, u.Password);
            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest("USERNAME_ALREADY_TAKEN");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> DeleteUser(int id, [FromBody] string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            var user = _userDao.GetUserById(id);

            if (user == null)
            {
                return NotFound("USER_NOT_FOUND");
            }

            var userId = _userDao.GetUserId(apiKey);

            if (userId == -1)
            {
                return BadRequest("INVALID_CREDENTIALS");
            }

            if (userId != id)
            {
                return BadRequest("CANNOT_DELETE_OTHER_USER");
            }

            bool res = _userDao.DeleteUser(id, apiKey);
            if (res)
            {
                return Ok(true);
            }

            return NotFound("USER_NOT_FOUND");
        }
    }

    public class APIRequestUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
