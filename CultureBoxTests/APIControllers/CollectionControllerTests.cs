using System.Collections.Generic;
using System.IO;
using CultureBox.APIControllers;
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

        public DbExecutor DbExecutor { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            CollectionController = new CollectionController(new UserDAO(DbExecutor),new CollectionDAO(DbExecutor), new BookDAO(DbExecutor));
            UserController = new UserController(new UserDAO(DbExecutor), new CollectionDAO(DbExecutor));
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
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = CollectionController.GetAllCollection(apiKey);

            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void GetAllCollectionTest_1()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (OkObjectResult)collection.Result;
            var result1 = (ApiCollection)(objectResult1).Value;

            var res = CollectionController.GetAllCollection(apiKey);
            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 1);
        }



        [TestMethod]
        public void GetCollectionByIdTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            
            var res = CollectionController.GetCollectionById(0,apikey);
            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 1);
        }
        
        [TestMethod]
        public void CreateCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            
            var req = new ApiCollectionRequest(){Apikey=apikey,Name= "Collection"};
            
            var res = Collection.Controller.CreateCollection(req);
            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 1);   
        }
        
        [TestMethod]
        public void DeleteCollection()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            
            CollectionController.DeleteCollection(0, apikey);
            var res = CollectionController.GetAllCollection(apikey);
            var objectResult = (OkOjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;
            
            Assert.IsNotNull(result);          
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
        }
        
        [TestMethod]
        public void AddBookToCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var res = BookController.SearchBook(new ApiRequestBook(){Title = "Harry Potter"});
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            
            var req = new ApiCollectionItemRequest(){Apikey = apikey, BookId = 0};
            
            var res = CollectionController.AddBookToCollection(0, req)
            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;
            
            Assert.IsNotNull(result);          
            Assert.AreEqual(objectResult.StatusCode, 200);
        }
        
        [TestMethod]
        public void RemoveBookFromCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var res = BookController.SearchBook(new ApiRequestBook(){Title = "Harry Potter"});
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            
            var req = new ApiCollectionItemRequest(){Apikey = apikey, BookId = 0};
            
            var res = CollectionController.AddBookToCollection(0, req)
            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;
            
            Assert.IsNull(result);          
            Assert.AreEqual(objectResult.StatusCode, 200);
        }
        
        [TestMethod]
        public void GetUserIdTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (OkObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var res = CollectionController.GetUserId(apikey);
            
            Assert.IsNotNull(res);
            Assert.AreEqual(res, apikey);
            Assert.AreEqual(res, -1);
        }
        
    }
}
