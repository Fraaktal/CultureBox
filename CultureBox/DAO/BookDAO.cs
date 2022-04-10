using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface IBookDAO
    {
        List<ApiBook> GetAllBooks(int resCount, int offset);
        ApiBook GetBookById(int id);
        void AddOrUpdateBook(ApiBook book);
    }

    public class BookDAO : IBookDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public BookDAO(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public List<ApiBook> GetAllBooks(int resCount, int offset)
        {
            List<ApiBook> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBook>("apibook");
                res = col.Find(x => true, offset, resCount).ToList();
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

        public void AddOrUpdateBook(ApiBook book)
        {
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiBook>("apibook");
                var existingBook = col.FindOne(b => b.ISBN == book.ISBN);

                if (existingBook == null)
                {
                    col.Insert(book);
                }
                else
                {
                    //est récupéré via API, on doit set l'id et on met les informations à jour.
                    book.Id = existingBook.Id;
                    col.Update(book);
                }
            });
        }
    }
}
