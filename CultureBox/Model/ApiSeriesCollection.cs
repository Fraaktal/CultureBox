using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CultureBox.Model
{
    public class ApiSeriesCollection
    {
        public ApiSeriesCollection()
        {
            Series = new List<ApiSeries>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int IdUser { get; set; }
        public List<ApiSeries> Series { get; set; }
    }
}
