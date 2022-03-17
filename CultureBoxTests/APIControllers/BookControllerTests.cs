using System.Collections.Generic;
using System.IO;
using CultureBox.APIControllers;
using CultureBox.Control;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CultureBoxTests.APIControllers
{
    [TestClass]
    public class BookControllerTests
    {
        public BookController BookController { get; set; }
        public DbExecutor DbExecutor { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            BookController = new BookController(new ApiBookController(new BookDAO(DbExecutor)));
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }

        [TestMethod]
        public void SearchBook()
        {
            var res = BookController.SearchBook(new ApiRequestBook(){Title = "Harry Potter"});

            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiBook>)(objectResult).Value;

            Assert.IsNotNull(result.Count);
            Assert.AreNotEqual(0, result.Count);
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var res = BookController.SearchBook(new ApiRequestBook() { Title = "Harry Potter" });

            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiBook>)(objectResult).Value;


            var res2 = BookController.GetAll();
            var objectResult2 = (OkObjectResult)res2.Result;
            var result2 = (List<ApiBook>)(objectResult2).Value;

            Assert.IsNotNull(result2);
            Assert.AreEqual(result2.Count, result.Count);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(objectResult2.StatusCode, 200);
        }

        [TestMethod]
        public void GetBookByIdTest()
        {
            var res = BookController.SearchBook(new ApiRequestBook() { Title = "Harry Potter" });

            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiBook>)(objectResult).Value;


            var res2 = BookController.GetBookById(1);
            var objectResult2 = (OkObjectResult)res2.Result;
            var result2 = (ApiBook)(objectResult2).Value;

            Assert.IsNotNull(result2);
            Assert.IsTrue(result.Contains(result2));
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(objectResult2.StatusCode, 200);
        }
        
        [TestMethod]
        public void SearchBookNoTitle()
        {
            var res = BookController.SearchBook(new ApiRequestBook() {  });
            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(400, objectResult.StatusCode);
        }
        
        
    }
}
