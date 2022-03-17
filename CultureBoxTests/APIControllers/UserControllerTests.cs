using System.IO;
using CultureBox.APIControllers;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CultureBoxTests.APIControllers
{
    [TestClass]
    public class UserControllerTests
    {
        public UserController UserController { get; set; }
        public DbExecutor DbExecutor { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            UserController = new UserController(new UserDAO(DbExecutor), new CollectionDAO(DbExecutor));
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }

        [TestMethod]
        public void CreateUserTest()
        {
            var user = new APIRequestUser() {Username = "Test", Password = "pass"};
            var res = UserController.CreateUser(user);

            var objectResult = (OkObjectResult) res.Result;
            var result = (ApiUser)(objectResult).Value;

            Assert.AreEqual(result.Username, user.Username);
            Assert.AreNotEqual(result.Password, user.Password);
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [TestMethod]
        public void GetTest()
        {
            var user = new APIRequestUser() { Username = "Test", Password = "pass" };
            var res = UserController.CreateUser(user);

            var objectResult = (OkObjectResult)res.Result;
            var createdUser = (ApiUser)(objectResult).Value;

            var res2 = UserController.Get(createdUser.Id);

            var objectResult2 = (OkObjectResult)res2.Result;
            var gotUser = (ApiUser)(objectResult2).Value;

            Assert.AreEqual(gotUser.Username, user.Username);
            Assert.AreNotEqual(gotUser.Password, user.Password);

            Assert.AreEqual(createdUser.Username, gotUser.Username);
            Assert.AreEqual(createdUser.Password, gotUser.Password);

            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(objectResult2.StatusCode, 200);
        }

        [TestMethod]
        public void GetApiKeyTest()
        {
            var user = new APIRequestUser() { Username = "Test", Password = "pass" };
            var res = UserController.CreateUser(user);

            var objectResult = (OkObjectResult)res.Result;

            var res2 = UserController.GetApiKey(user);

            var objectResult2 = (OkObjectResult)res2.Result;
            var apiKey = (string)(objectResult2).Value;

            Assert.IsNotNull(apiKey);
            Assert.AreNotEqual(apiKey, "");

            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(objectResult2.StatusCode, 200);
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            var user = new APIRequestUser() { Username = "Test", Password = "pass" };
            var res = UserController.CreateUser(user);

            var objectResult = (OkObjectResult)res.Result;
            var createdUser = (ApiUser)(objectResult).Value;

            var res2 = UserController.DeleteUser(createdUser.Id, createdUser.ApiKey);

            var objectResult2 = (OkObjectResult)res2.Result;
            var isOk = (bool)(objectResult2).Value;

            var res3 = UserController.Get(createdUser.Id);

            var objectResult3 = (NotFoundObjectResult)res3.Result;

            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(objectResult2.StatusCode, 200);
            //Pas d'user
            Assert.AreEqual(objectResult3.StatusCode, 404);
        }
        
        
        [TestMethod]
        public void CreateFalseUserNoPass()
        {
            var user = new APIRequestUser() {Username = "Test"};
            var res = UserController.CreateUser(user);

            var objectResult = (OkObjectResult) res.Result;
            var result = (ApiUser)(objectResult).Value;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }
        [TestMethod]
        public void CreateFalseUserNoUser()
        {
            var user = new APIRequestUser() { };
            var res = UserController.CreateUser(user);

            var objectResult = (OkObjectResult) res.Result;
            var result = (ApiUser)(objectResult).Value;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }
        [TestMethod]
        public void CreateFalseUserNoUser()
        {
            var user = new APIRequestUser() { Password = "pass"  };
            var res = UserController.CreateUser(user);

            var objectResult = (OkObjectResult) res.Result;
            var result = (ApiUser)(objectResult).Value;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }
    }
}
