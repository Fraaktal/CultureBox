using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface IMovieCollectionDAO
    {
        List<ApiMovieCollection> GetAllCollection(int id);
        ApiMovieCollection GetCollectionById(int userId, int id);
        ApiMovieCollection CreateCollection(string name, int userId);
        bool DeleteCollection(int userId, int id);
        ApiMovieCollection AddMovieToCollection(int userId, int id, ApiMovie Movie);
        ApiMovieCollection RemoveMovieFromCollection(ApiMovieCollection collection, int reqMovieId, out bool b);
        List<ApiObjectToBorrow> SearchMovie(string title);
    }

    public class MovieCollectionDao : IMovieCollectionDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public MovieCollectionDao(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public List<ApiMovieCollection> GetAllCollection(int id)
        {
            List<ApiMovieCollection> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovieCollection>("apimoviecollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.Find(c => c.IdUser == id).ToList();
            });

            return res;
        }

        public ApiMovieCollection GetCollectionById(int userId, int id)
        {
            ApiMovieCollection res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovieCollection>("apimoviecollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.FindOne(c => c.IdUser == userId && c.Id == id);
            });

            return res;
        }

        public ApiMovieCollection CreateCollection(string name, int userId)
        {
            ApiMovieCollection res = new ApiMovieCollection();
            res.Name = name;
            res.IdUser = userId;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovieCollection>("apimoviecollection");
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
                var col = db.GetCollection<ApiMovieCollection>("apimoviecollection");

                if (col.FindOne(x => x.Id == id && x.IdUser == id) != null)
                {
                    isOk = col.Delete(id);
                }
            });

            return isOk;
        }

        public ApiMovieCollection AddMovieToCollection(int userId, int id, ApiMovie Movie)
        {
            ApiMovieCollection res = GetCollectionById(userId, id);

            if (res != null)
            {
                res.Movies.Add(Movie);
                _dbExecutor.Execute(db =>
                {
                    var col = db.GetCollection<ApiMovieCollection>("apimoviecollection");
                    col.Update(res);
                });
            }

            return res;
        }

        public ApiMovieCollection RemoveMovieFromCollection(ApiMovieCollection collection, int MovieId, out bool res)
        {
            int removed = collection.Movies.RemoveAll(b => b.Id == MovieId);
            res = removed > 0;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovieCollection>("apimoviecollection");
                col.Update(collection);
            });

            return collection;
        }

        public List<ApiObjectToBorrow> SearchMovie(string title)
        {
            List<ApiObjectToBorrow> res = new List<ApiObjectToBorrow>();

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiMovieCollection>("apimoviecollection");
                col.EnsureIndex(x => x.Name);

                var queryResult = col.FindAll();

                foreach (var c in queryResult)
                {
                    var Movie = c.Movies.FirstOrDefault(b => b.Title?.Contains(title) ?? false);

                    if (Movie != null)
                    {
                        res.Add(new ApiObjectToBorrow(Movie.Id, c.IdUser));
                    }
                }
            });

            return res;
        }
    }
}
