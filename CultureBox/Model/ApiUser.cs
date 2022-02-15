using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CultureBox.Model
{
    public class ApiUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string APIKey { get; set; }
    }
}
