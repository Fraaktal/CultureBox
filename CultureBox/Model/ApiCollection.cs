using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CultureBox.Model
{
    public class ApiCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdUser { get; set; }
        public List<ApiBook> Books { get; set; }
    }
}
