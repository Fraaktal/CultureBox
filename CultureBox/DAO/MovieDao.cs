using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface IMovieDao
    {
        List<ApiMovie> GetAllMovies(int resCount, int offset);
        ApiMovie GetMovieById(int id);
    }

    public class MovieDao : IMovieDao
    {
        public List<ApiMovie> GetAllMovies(int resCount, int offset)
        {
            //throw new NotImplementedException();
            return null;
        }

        public ApiMovie GetMovieById(int id)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
