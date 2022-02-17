using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CultureBox.Model
{
    public class ApiBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Description { get; set; }
        public string Theme { get; set; }
        public string ISBN { get; set; }
    }
}
