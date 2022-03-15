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
        public ActionResult<ApiUser> GetAllUser()
        {
            var users = _userDao.GetAllUsers();
            if (users == null)
            {
                return NotFound("USERS_NOT_FOUND");
            }

            return Ok(users);
        }

        [HttpGet("apikey")]
        public ActionResult<string> GetApiKey([FromBody] APIRequestUser u)
        {
            var apiKey = _userDao.GetApiKey(u.Username, u.Password);
            if (!string.IsNullOrEmpty(apiKey))
            {
                return Ok(apiKey);
            }

            return NotFound("INVALID_CREDENTIALS");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiUser> CreateUser([FromBody] APIRequestUser u)
        {
            var user = _userDao.CreateUser(u.Username, u.Password);
            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest("USERNAME_ALREADY_TAKEN");
        }

        [HttpDelete("{id}")]
        public ActionResult<bool> DeleteUser(int id, [FromBody] string apiKey)
        {
            bool res = _userDao.DeleteUser(id, apiKey);
            if (res)
            {
                return Ok(true);
            }

            return NotFound("USER_NOT_FOUND");
        }

        [HttpGet("{id}/Collection")]
        public ActionResult<bool> GetAllCollection(int id, [FromBody] string apiKey)
        {
            bool apiKeyOk = _userDao.CheckApiKey(id, apiKey);
            if (apiKeyOk)
            {
                var res = _collectionDao.GetAllCollection(id);
                if (res != null)
                {
                    return Ok(true);
                }

                return NotFound("COLLECTION_NOT_FOUND");

            }
            else
            {
                return BadRequest("INVALID_CREDENTIALS");
            }
        }
    }

    public class APIRequestUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
