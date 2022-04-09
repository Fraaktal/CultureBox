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
            Assert.AreEqual(200, objectResult.StatusCode);
        }
        
        [TestMethod]
        public void CreateUserTest_2UsersSamePseudo()
        {
            // We test to create 1 user (OK)
            var user = new APIRequestUser() {Username = "Test", Password = "pass"};
            var res1 = UserController.CreateUser(user);
            var objectResult = (ObjectResult) res1.Result;
            var result = (ApiUser)(objectResult).Value;
            Assert.AreEqual(result.Username, user.Username);
            Assert.AreNotEqual(result.Password, user.Password); // Pas de MDP retourné en clair !!
            Assert.AreEqual(200, objectResult.StatusCode);
            
            // Now, we test to add another user with the same username
            var user2 = new APIRequestUser() {Username = "Test", Password = "pass2"};
            var res2 = UserController.CreateUser(user2);
            var objectResult2 = (ObjectResult2) res2.Result;
            Assert.AreEqual(400, objectResult2.StatusCode);
            
            // We get all users, and verify that there is only 1 user
            var list = UserController.GetAllUsers();            
            var objectResultList = (objectResultList) list.Result;
            var result = (List<ApiUser>)(objectResultList).Value;
            Assert.AreEqual(1, list.Count);
            
            
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

            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual(200, objectResult2.StatusCode);
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

            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual(200, objectResult2.StatusCode);
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

            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual(200, objectResult2.StatusCode);
            //Pas d'user
            Assert.AreEqual(404, objectResult3.StatusCode);
        }
        
        
        [TestMethod]
        public void CreateFalseUserNoPass()
        {
            var user = new APIRequestUser() {Username = "Test"};
            var res = UserController.CreateUser(user);

            var objectResult = (BadRequestObjectResult) res.Result;
            var result = (string)(objectResult).Value;

            Assert.AreEqual(400, objectResult.StatusCode);
        }
        [TestMethod]
        public void CreateFalseUserNoUser()
        {
            var user = new APIRequestUser() { };
            var res = UserController.CreateUser(user);

            var objectResult = (BadRequestObjectResult) res.Result;
            var result = (string)(objectResult).Value;

            Assert.AreEqual(400, objectResult.StatusCode);
        }
        [TestMethod]
        public void CreateFalseUserNoUserButPasswd()
        {
            var user = new APIRequestUser() { Password = "pass"  };
            var res = UserController.CreateUser(user);

            var objectResult = (BadRequestObjectResult) res.Result;
            var result = (string)(objectResult).Value;

            Assert.AreEqual(400, objectResult.StatusCode);
        }

        [TestMethod]
        public void GetApiKeyNoUser()
        {
            var user = new APIRequestUser() { };
            var res = UserController.GetApiKey(user);

            var objectResult = (ObjectResult)res.Result;
            var apiKey = (string)(objectResult).Value;
            Assert.AreEqual(400, objectResult.StatusCode);
        }
          
        [TestMethod]
        public void GetApiKeyBadUser()
        {  
            var user = new APIRequestUser() { Username = "Toto", Password = "tata" };
            var res = UserController.GetApiKey(user);

            var objectResult = (ObjectResult)res.Result;
            var apiKey = (string)(objectResult).Value;
            Assert.AreEqual(400, objectResult.StatusCode);
        } 
        
        [TestMethod]
        public void TestDeleteBadUser()
        {  
            var res = UserController.DeleteUser(999, "aaaaaaa");

            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(404, objectResult.StatusCode);
        }
        [TestMethod]
        public void TestDeleteUser()
        {
            var user = new APIRequestUser() { Username = "Test", Password = "pass" };
            UserController.CreateUser(user);
            var res = UserController.DeleteUser(1, "test");

            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(400, objectResult.StatusCode);
        }

        [TestMethod]
        public void TestDeleteUserBadRequest_NoApiKey()
        {
            var res = UserController.DeleteUser(1, "");

            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(400, objectResult.StatusCode);
        }
        
        [TestMethod]
        public void TestDeleteUser_BadAPIKey()
        {
            // User id = 1 because the database is truncated.
            var user = new APIRequestUser() { Username = "Test", Password = "pass" };
            UserController.CreateUser(user);
            var apiKey1 = UserController.GetApiKey(user);
            
            // User id = 2 because the database is truncated.
            var user2 = new APIRequestUser() { Username = "Test2", Password = "pass2" };
            UserController.CreateUser(user2);
            var apiKey2 = UserController.GetApiKey(user2);
            
            // We test to delete user1 with apikey from user2
            var res = UserController.DeleteUser(1, apiKey2);

            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(400, objectResult.StatusCode);
        }
    }
}
