using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface IMovieDAO
    {
        List<ApiMovie> GetAllMovies(int resCount, int offset);
        ApiMovie GetMovieById(int id);
        void AddOrUpdateMovie(ApiMovie movie);
    }

    public class MovieDao : IMovieDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public MovieDao(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }
        public List<ApiMovie> GetAllMovies(int resCount, int offset)
        {
            List<ApiMovie> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovie>("apimovie");
                res = col.Find(x => true, offset, resCount).ToList();
            });

            return res;
        }

        public ApiMovie GetMovieById(int id)
        {
            ApiMovie res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovie>("apimovie");
                res = col.FindOne(b => b.Id == id);
            });

            return res;
        }

        public void AddOrUpdateMovie(ApiMovie movie)
        {
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovie>("apimovie");
                var existingBook = col.FindOne(b => b.Title == movie.Title);

                if (existingBook == null)
                {
                    col.Insert(movie);
                }
                else
                {
                    //est récupéré via API, on doit set l'id et on met les informations à jour.
                    movie.Id = existingBook.Id;
                    col.Update(movie);
                }
            });
        }
    }
}
