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
        private readonly ILogger<UserController> _logger;
        private readonly IUserDAO _userDao;

        public UserController(ILogger<UserController> logger, IUserDAO userDAO)
        {
            _logger = logger;
            _userDao = userDAO;
        }

        [HttpGet("{id}")]
        public ApiUser Get(int id)
        {
            var user = _userDao.GetUserById(id);
            return user;
        }

        [HttpGet("login")]
        public string login([FromBody] APIRequestUser u)
        {
            var apiKey = _userDao.GetApiKey(u.Username, u.Password);
            return apiKey;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public void CreateUser([FromBody] APIRequestUser u)
        {
            _userDao.CreateUser(u.Username, u.Password);
        }

        [HttpDelete("{id}")]
        public void DeleteUser(int id, [FromBody] string password)
        {
            _userDao.DeleteUser(id, password);
        }
    }

    public class APIRequestUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
