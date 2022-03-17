using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface ICollectionDAO
    {
        List<ApiCollection> GetAllCollection(int id);
        ApiCollection GetCollectionById(int userId, int id);
        ApiCollection CreateCollection(string name, int userId);
        bool DeleteCollection(int userId, int id);
        ApiCollection AddBookToCollection(int userId, int id, ApiBook book);
        ApiCollection RemoveBookFromCollection(ApiCollection collection, int reqBookId, out bool b);
    }

    public class CollectionDAO: ICollectionDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public CollectionDAO(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public List<ApiCollection> GetAllCollection(int id)
        {
            List<ApiCollection> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiCollection>("apicollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.Find(c => c.IdUser == id).ToList();
            });

            return res;
        }

        public ApiCollection GetCollectionById(int userId, int id)
        {
            ApiCollection res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiCollection>("apicollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.FindOne(c => c.IdUser == userId && c.Id == id);
            });

            return res;
        }

        public ApiCollection CreateCollection(string name, int userId)
        {
            ApiCollection res = new ApiCollection();
            res.Name = name;
            res.IdUser = userId;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiCollection>("apicollection");
                var collection = col.FindOne(x => x.Name == name);
                if(collection == null)
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
                var col = db.GetCollection<ApiCollection>("apicollection");

                if (col.FindOne(x => x.Id == id && x.IdUser == id) != null)
                {
                    isOk = col.Delete(id);
                }
            });

            return isOk;
        }

        public ApiCollection AddBookToCollection(int userId, int id, ApiBook book)
        {
            ApiCollection res = GetCollectionById(userId,id);

            if (res != null)
            {
                res.Books.Add(book);
                _dbExecutor.Execute(db =>
                {
                    var col = db.GetCollection<ApiCollection>("apicollection");
                    col.Update(res);
                });
            }

            return res;
        }

        public ApiCollection RemoveBookFromCollection(ApiCollection collection, int bookId, out bool res)
        {
            int removed = collection.Books.RemoveAll(b => b.Id == bookId);
            res = removed > 0;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiCollection>("apicollection");
                col.Update(collection);
            });
            
            return collection;
        }
    }
}
