using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface IBookCollectionDAO
    {
        List<ApiBookCollection> GetAllCollection(int id);
        ApiBookCollection GetCollectionById(int userId, int id);
        ApiBookCollection CreateCollection(string name, int userId);
        bool DeleteCollection(int userId, int id);
        ApiBookCollection AddBookToCollection(int userId, int id, ApiBook book);
        ApiBookCollection RemoveBookFromCollection(ApiBookCollection collection, int reqBookId, out bool b);
        List<ApiObjectToBorrow> SearchBook(string title);
    }

    public class BookCollectionDao: IBookCollectionDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public BookCollectionDao(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public List<ApiBookCollection> GetAllCollection(int id)
        {
            List<ApiBookCollection> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBookCollection>("apibookcollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.Find(c => c.IdUser == id).ToList();
            });

            return res;
        }

        public ApiBookCollection GetCollectionById(int userId, int id)
        {
            ApiBookCollection res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBookCollection>("apibookcollection");
                col.EnsureIndex(x => x.IdUser);
                res = col.FindOne(c => c.IdUser == userId && c.Id == id);
            });

            return res;
        }

        public ApiBookCollection CreateCollection(string name, int userId)
        {
            ApiBookCollection res = new ApiBookCollection();
            res.Name = name;
            res.IdUser = userId;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBookCollection>("apibookcollection");
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
                var col = db.GetCollection<ApiBookCollection>("apibookcollection");

                if (col.FindOne(x => x.Id == id && x.IdUser == id) != null)
                {
                    isOk = col.Delete(id);
                }
            });

            return isOk;
        }

        public ApiBookCollection AddBookToCollection(int userId, int id, ApiBook book)
        {
            ApiBookCollection res = GetCollectionById(userId,id);

            if (res != null)
            {
                res.Books.Add(book);
                _dbExecutor.Execute(db =>
                {
                    var col = db.GetCollection<ApiBookCollection>("apibookcollection");
                    col.Update(res);
                });
            }

            return res;
        }

        public ApiBookCollection RemoveBookFromCollection(ApiBookCollection collection, int bookId, out bool res)
        {
            int removed = collection.Books.RemoveAll(b => b.Id == bookId);
            res = removed > 0;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBookCollection>("apibookcollection");
                col.Update(collection);
            });
            
            return collection;
        }

        public List<ApiObjectToBorrow> SearchBook(string title)
        {
            List<ApiObjectToBorrow> res = new List<ApiObjectToBorrow>();

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBookCollection>("apibookcollection");
                col.EnsureIndex(x => x.Name);
                
                var queryResult = col.FindAll();

                foreach (var c in queryResult)
                {
                    var book = c.Books.FirstOrDefault(b => b.Title?.ToLower()?.Contains(title.ToLower()) ?? false);

                    if (book != null)
                    {
                        res.Add(new ApiObjectToBorrow(book.Id, c.IdUser));
                    }
                }
            });

            return res;
        }
    }
}
