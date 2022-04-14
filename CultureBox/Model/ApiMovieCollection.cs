using System.Collections.Generic;

namespace CultureBox.Model
{
    public class ApiMovieCollection
    {
        public ApiMovieCollection()
        {
            Movies = new List<ApiMovie>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdUser { get; set; }
        public List<ApiMovie> Movies { get; set; }
    }
}
