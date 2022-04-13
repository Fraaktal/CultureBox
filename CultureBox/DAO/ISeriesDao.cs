using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface ISeriesDao
    {
        List<ApiSeries> GetAllSeries(int resCount, int offset);
        ApiSeries GetSeriesById(int id);
    }
    public class SeriesDao : ISeriesDao
    {
        public List<ApiSeries> GetAllSeries(int resCount, int offset)
        {
            throw new NotImplementedException();
        }

        public ApiSeries GetSeriesById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
