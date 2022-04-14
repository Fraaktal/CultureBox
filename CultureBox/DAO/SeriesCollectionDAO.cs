using System;
using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface ISeriesCollectionDAO
    {
        List<ApiSeriesCollection> GetAllCollection(int id);
        ApiSeriesCollection GetCollectionById(int userId, int id);
        ApiSeriesCollection CreateCollection(string name, int userId);
        bool DeleteCollection(int userId, int id);
        ApiSeriesCollection AddSeriesToCollection(int userId, int id, ApiSeries Series);
        ApiSeriesCollection RemoveSeriesFromCollection(ApiSeriesCollection collection, int reqSeriesId, out bool b);
        List<ApiObjectToBorrow> SearchSeries(string title);
    }

    public class SeriesCollectionDAO : ISeriesCollectionDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public SeriesCollectionDAO(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public List<ApiSeriesCollection> GetAllCollection(int id)
        {
            List<ApiSeriesCollection> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeriesCollection>("apiseriescollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.Find(c => c.IdUser == id).ToList();
            });

            return res;
        }

        public ApiSeriesCollection GetCollectionById(int userId, int id)
        {
            ApiSeriesCollection res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeriesCollection>("apiseriescollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.FindOne(c => c.IdUser == userId && c.Id == id);
            });

            return res;
        }

        public ApiSeriesCollection CreateCollection(string name, int userId)
        {
            ApiSeriesCollection res = new ApiSeriesCollection();
            res.Name = name;
            res.IdUser = userId;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeriesCollection>("apiseriescollection");
                var collection = col.FindOne(x => x.Name == name);
                if (collection == null)
                {
                    int id = col.Insert(res);
                    res = col.FindById(id);
                }
                else
                {
                    res = null;
                }
            });

            return res;
        }

        public bool DeleteCollection(int userId, int id)
        {
            bool isOk = false;
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeriesCollection>("apiseriescollection");

                if (col.FindOne(x => x.Id == id && x.IdUser == id) != null)
                {
                    isOk = col.Delete(id);
                }
            });

            return isOk;
        }

        public ApiSeriesCollection AddSeriesToCollection(int userId, int id, ApiSeries Series)
        {
            ApiSeriesCollection res = GetCollectionById(userId, id);

            if (res != null)
            {
                res.Series.Add(Series);
                _dbExecutor.Execute(db =>
                {
                    var col = db.GetCollection<ApiSeriesCollection>("apiseriescollection");
                    col.Update(res);
                });
            }

            return res;
        }

        public ApiSeriesCollection RemoveSeriesFromCollection(ApiSeriesCollection collection, int SeriesId, out bool res)
        {
            int removed = collection.Series.RemoveAll(b => b.Id == SeriesId);
            res = removed > 0;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeriesCollection>("apiseriescollection");
                col.Update(collection);
            });

            return collection;
        }

        public List<ApiObjectToBorrow> SearchSeries(string title)
        {
            List<ApiObjectToBorrow> res = new List<ApiObjectToBorrow>();

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiSeriesCollection>("apiseriescollection");
                col.EnsureIndex(x => x.Name);

                var queryResult = col.FindAll();

                foreach (var c in queryResult)
                {
                    var Series = c.Series.FirstOrDefault(b => b.Title?.Contains(title) ?? false);

                    if (Series != null)
                    {
                        res.Add(new ApiObjectToBorrow(Series.Id, c.IdUser));
                    }
                }
            });

            return res;
        }
    }
}
