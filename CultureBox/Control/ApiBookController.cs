using System.Collections.Generic;
using System.Linq;
using CultureBox.APIControllers;
using CultureBox.DAO;
using CultureBox.Model;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;

namespace CultureBox.Control
{
    public interface IApiBookController
    {
        List<ApiBook> Search(string title);
        ApiBook GetBookById(int id);
        List<ApiBook> GetAllBooks(int resCount, int offset);
    }

    public class ApiBookController : IApiBookController
    {
        private readonly IBookDAO _bookDao;

        public ApiBookController(IBookDAO bookDao)
        {
            _bookDao = bookDao;
        }

        public List<ApiBook> Search(string title)
        {
            var service = new BooksService(new BaseClientService.Initializer());

            var request = new VolumesResource.ListRequest(service, title)
            {
                MaxResults = 40
            };

            var searchResult = request.Execute();

            var books = ConvertToApiBook(searchResult.Items.ToList());
            foreach (var apiBook in books)
            {
                _bookDao.AddOrUpdateBook(apiBook);
            }

            return books;
        }

        public ApiBook GetBookById(int id)
        {
            return _bookDao.GetBookById(id);
        }

        public List<ApiBook> GetAllBooks(int resCount, int offset)
        {
            return _bookDao.GetAllBooks(resCount, offset);
        }

        private List<ApiBook> ConvertToApiBook(List<Volume> googleBooks)
        {
            return googleBooks.ConvertAll(x => new ApiBook
            {
                Title = x.VolumeInfo.Title ?? "",
                Authors = x.VolumeInfo.Authors?.ToList() ?? new List<string>(),
                Themes = x.VolumeInfo.Categories?.ToList() ?? new List<string>(),
                Description = x.VolumeInfo.Description ?? "",
                ISBN = GetIsbn(x.VolumeInfo.IndustryIdentifiers)
            });
        }

        private string GetIsbn(IList<Volume.VolumeInfoData.IndustryIdentifiersData> data)
        {
            if (data != null && data.Count > 0)
            {
                var isbn13 = data.FirstOrDefault(a => Equals(a.Type, "ISBN_13"));
                if (isbn13 != null)
                {
                    return isbn13.Identifier ?? "";
                }

                return data.FirstOrDefault()?.Identifier ?? "";
            }

            return "";
        }
    }
}
