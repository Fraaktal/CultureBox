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
        }
        
        [TestMethod]
        public void CreateCollectionTest()
        {
        }
        
        [TestMethod]
        public void DeleteCollection()
        {
        }
        
        [TestMethod]
        public void AddBookToCollectionTest()
        {
        }
        
        [TestMethod]
        public void RemoveBookFromCollectionTest()
        {
        }
        
        [TestMethod]
        public void GetUserIdTest()
        {
        }
        
    }
}
