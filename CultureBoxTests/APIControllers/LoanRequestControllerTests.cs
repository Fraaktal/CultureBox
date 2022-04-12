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
    public class LoanRequestControllerTests
    {
        public CollectionController CollectionController { get; set; }

        public UserController UserController { get; set; }
        public BookController BookController { get; set; }
        public LoanRequestController LoanRequestController { get; set; }
        public DbExecutor DbExecutor { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            CollectionController = new CollectionController(
                new UserDAO(DbExecutor),
                new CollectionDAO(DbExecutor), 
                new BookDAO(DbExecutor)
            );
            UserController = new UserController(new UserDAO(DbExecutor));
            BookController = new BookController(new ApiBookController(new BookDAO(DbExecutor)));
            LoanRequestController = new LoanRequestController(
                new LoanRequestControllerDAO(DbExecutor), 
                new BookDAO(DbExecutor), 
                new UserDAO(DbExecutor), 
                new CollectionDAO(DbExecutor)
            );
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }

        [TestMethod]
        public void TestGetAllRequests()
        {
            // Create user 1 
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            // Its collection is empty, normal
            var res = LoanRequestController.GetAllRequests(apiKey);

            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
            
            // No API Key ?
            var res = LoanRequestController.GetAllRequests("");
            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(400, objectResult.StatusCode);
            
            // Just a bad API key
            var res = LoanRequestController.GetAllRequests("dzkefu");
            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(400, objectResult.StatusCode);
            
        }
    }
}
