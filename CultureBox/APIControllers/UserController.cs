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

        public UserController(IUserDAO userDAO)
        {
            _userDao = userDAO;
        }

        /// <summary>
        /// Get the user corresponding to the given id.
        /// </summary>
        /// <param name="id">Id of the user.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ApiUser> GetAllUser()
        {
            var users = _userDao.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Get the apiKey of the given user.
        /// </summary>
        /// <param name="request">Username: the name of the user.
        /// Password: the password of the user.</param>
        /// <returns></returns>
        [HttpGet("apikey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> GetApiKey([FromBody] RequestUser request)
        {
            var apiKey = _userDao.GetApiKey(request.Username, request.Password);
            if (!string.IsNullOrEmpty(apiKey))
            {
                return Ok(apiKey);
            }

            return BadRequest("INVALID_CREDENTIALS");
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="request">Username: the name of the user.
        /// Password: the password of the user.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiUser> CreateUser([FromBody] RequestUser request)
        {
            if (string.IsNullOrEmpty(request.Username) && string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("MISSING_USERNAME_AND_PASSWORD");
            }

            if (string.IsNullOrEmpty(request.Username))
            {
                return BadRequest("MISSING_USERNAME");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("MISSING_PASSWORD");
            }

            var user = _userDao.CreateUser(request.Username, request.Password);
            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest("USERNAME_ALREADY_TAKEN");
        }

        /// <summary>
        /// Delete the user corresponding to the given id if it corresponds to the apikey.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="apiKey">Your apikey.</param>
        /// <returns></returns>
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
}
