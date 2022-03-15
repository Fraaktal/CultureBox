using System.Collections.Generic;

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
