using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface IBookDAO
    {
        List<ApiBook> GetAllBooks();
        ApiBook GetBookById(int id);
    }

    public class BookDAO : IBookDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public BookDAO(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public List<ApiBook> GetAllBooks()
        {
            List<ApiBook> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBook>("apibook");
                res = col.FindAll().ToList();
            });

            return res;
        }

        public ApiBook GetBookById(int id)
        {
            ApiBook res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBook>("apibook");
                res = col.FindOne(b => b.Id == id);
            });

            return res;
        }

        public void CreateBook(ApiBook book)
        {
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBook>("apibook");
                col.Insert(book);
            });
        }
    }
}
