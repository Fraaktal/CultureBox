using System.Collections.Generic;

namespace CultureBox.Model
{
    public class ApiBookCollection
    {
        public ApiBookCollection()
        {
            Books = new List<ApiBook>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdUser { get; set; }
        public List<ApiBook> Books { get; set; }
    }
}
