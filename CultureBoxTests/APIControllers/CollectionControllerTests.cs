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
    public class CollectionControllerTests
    {
        public CollectionController CollectionController { get; set; }

        public UserController UserController { get; set; }
        public BookController BookController { get; set; }
        public DbExecutor DbExecutor { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            CollectionController = new CollectionController(new UserDAO(DbExecutor),new CollectionDAO(DbExecutor), new BookDAO(DbExecutor));
            UserController = new UserController(new UserDAO(DbExecutor), new CollectionDAO(DbExecutor));
            BookController = new BookController(new ApiBookController(new BookDAO(DbExecutor)));
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }
        
        [TestMethod]
        public void GetAllCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = CollectionController.GetAllCollection(apiKey);

            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void GetAllCollectionTest_1()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result1 = (ApiCollection)(objectResult1).Value;

            var res = CollectionController.GetAllCollection(apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 1);
        }



        [TestMethod]
        public void GetCollectionByIdTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiCollection)(colres.Value);

            var res = CollectionController.GetCollectionById(col.Id, apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (ApiCollection)(objectResult.Value);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void CreateCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var req = new ApiCollectionRequest(){ApiKey= apiKey, Name= "Collection"};
            
            var res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;
            var result = (ApiCollection)(objectResult.Value);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);   
        }
        
        [TestMethod]
        public void DeleteCollection()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiCollection)(colres.Value);

            CollectionController.DeleteCollection(col.Id, apiKey);
            var res = CollectionController.GetAllCollection(apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;
            
            Assert.IsNotNull(result);          
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
        }
        
        [TestMethod]
        public void AddBookToCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var res = BookController.SearchBook(new ApiRequestBook(){Title = "Harry Potter"});
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiCollection)(colres.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, BookId = books[0].Id};

            var res2 = CollectionController.AddBookToCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;
            var result = (ApiCollection)(objectResult.Value);
            
            Assert.IsNotNull(result);          
            Assert.AreEqual(objectResult.StatusCode, 200);
        }
        
        [TestMethod]
        public void RemoveBookFromCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var res = BookController.SearchBook(new ApiRequestBook(){Title = "Harry Potter"});
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"}); 
            var colres = (ObjectResult)collection.Result;
            var col = (ApiCollection)(colres.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, BookId = books[0].Id};

            var r = CollectionController.AddBookToCollection(col.Id, req);
            
            var res2 = CollectionController.RemoveBookFromCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;
            var result = (ApiCollection)(objectResult.Value);
            
            Assert.AreEqual(result.Books.Count, 0);          
            Assert.AreEqual(objectResult.StatusCode, 200);
        }
    }
}
