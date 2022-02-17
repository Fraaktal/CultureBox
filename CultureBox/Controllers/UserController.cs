using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Http;
using NSwag.Annotations;

namespace CultureBox.Controllers
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

        [HttpGet("{id}")]
        public ActionResult<ApiUser> Get(int id)
        {
            var user = _userDao.GetUserById(id);
            if (user == null)
            {
                return NotFound(null);
            }

            return Ok(user);
        }

        [HttpGet("apikey")]
        public ActionResult<string> GetApiKey([FromBody] APIRequestUser u)
        {
            var apiKey = _userDao.GetApiKey(u.Username, u.Password);
            if (!string.IsNullOrEmpty(apiKey))
            {
                return Ok(apiKey);
            }

            return NotFound(null);
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

            return BadRequest(null);
        }

        [HttpDelete("{id}")]
        public ActionResult<bool> DeleteUser(int id, [FromBody] string password)
        {
            bool res = _userDao.DeleteUser(id, password);
            if (res)
            {
                return Ok(true);
            }

            return NotFound(false);
        }
    }

    public class APIRequestUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
