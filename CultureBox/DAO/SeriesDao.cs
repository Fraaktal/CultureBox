using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface ISeriesDao
    {
        List<ApiSeries> GetAllSeries(int resCount, int offset);
        ApiSeries GetSeriesById(int id);
        void AddOrUpdateSeries(ApiSeries serie);
    }
    public class SeriesDao : ISeriesDao
    {
        private readonly IDbExecutor _dbExecutor;

        public SeriesDao(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }
        public List<ApiSeries> GetAllSeries(int resCount, int offset)
        {
            List<ApiSeries> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeries>("apiseries");
                res = col.Find(x => true, offset, resCount).ToList();
            });

            return res;
        }

        public ApiSeries GetSeriesById(int id)
        {
            ApiSeries res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeries>("apiseries");
                res = col.FindOne(b => b.Id == id);
            });

            return res;
        }

        public void AddOrUpdateSeries(ApiSeries movie)
        {
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeries>("apiseries");
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
