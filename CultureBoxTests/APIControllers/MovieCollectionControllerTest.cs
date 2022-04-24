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
    public class MovieCollectionControllerTest
    {
        public MovieCollectionController CollectionController { get; set; }

        public UserController UserController { get; set; }
        public MovieController MovieController { get; set; }
        public DbExecutor DbExecutor { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            CollectionController = new MovieCollectionController(new UserDAO(DbExecutor), new MovieCollectionDao(DbExecutor), new MovieDao(DbExecutor));
            UserController = new UserController(new UserDAO(DbExecutor));
            MovieController = new MovieController(new ApiMovieSerieController(new MovieDao(DbExecutor),new SeriesDao(DbExecutor)));
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }

        [TestMethod]
        public void GetAllCollectionTest()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = CollectionController.GetAllCollection(apiKey);

            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiMovieCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void GetAllCollectionBadReq()
        {
            var res = CollectionController.GetAllCollection("dfsdfsd");

            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void GetAllCollectionTest_1()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var objectResult1 = (ObjectResult)collection.Result;
            var result1 = (ApiMovieCollection)(objectResult1).Value;

            var res = CollectionController.GetAllCollection(apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiMovieCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 1);
        }



        [TestMethod]
        public void GetCollectionByIdTest()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var res = CollectionController.GetCollectionById(col.Id, apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (ApiMovieCollection)(objectResult.Value);

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetCollectionByIdNotFound()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = CollectionController.GetCollectionById(3, apiKey);
            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(objectResult.StatusCode, 404);
        }

        [TestMethod]
        public void GetCollectionByIdBadReq()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            var apiKey2 = "dfgdfgdfg";

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var res = CollectionController.GetCollectionById(col.Id, apiKey2);
            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void CreateCollectionTest()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var req = new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" };

            var res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;
            var result = (ApiMovieCollection)(objectResult.Value);

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateCollectionBadReqNameTaken()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var req = new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" };

            var res = CollectionController.CreateCollection(req);
            res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void CreateCollectionBadReqEmpty()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var req = new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" };

            var res = CollectionController.CreateCollection(null);
            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void CreateCollectionBadReqNameEmpty()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var req = new ApiCollectionRequest() { ApiKey = apiKey, Name = "" };

            var res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void CreateCollectionBadReqInvalid()
        {
            var apiKey = "dfsdfsd";

            var req = new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" };

            var res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }



        [TestMethod]
        public void DeleteCollection()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            CollectionController.DeleteCollection(col.Id, apiKey);
            var res = CollectionController.GetAllCollection(apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiMovieCollection>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void DeleteCollectionNotFound()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = CollectionController.DeleteCollection(0, apiKey);
            var objectResult = (ObjectResult)res;

            Assert.AreEqual(404, objectResult.StatusCode);
        }

        [TestMethod]
        public void DeleteCollectionBadReq()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            var apiKey2 = "dvsdf";

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);


            var res = CollectionController.DeleteCollection(col.Id, apiKey2);
            var objectResult = (ObjectResult)res;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void AddMovieToCollectionTest()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = MovieController.SearchMovie("Harry Potter");
            var MovieRes = (ObjectResult)res.Result;
            var Movies = (List<ApiMovie>)(MovieRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey, ObjectId = Movies[0].Id };

            var res2 = CollectionController.AddMovieToCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;
            var result = (ApiMovieCollection)(objectResult.Value);

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [TestMethod]
        public void AddMovietoCollectionNotFound()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = MovieController.SearchMovie("Harry Potter");
            var MovieRes = (ObjectResult)res.Result;
            var Movies = (List<ApiMovie>)(MovieRes.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey, ObjectId = Movies[0].Id };

            var res2 = CollectionController.AddMovieToCollection(1000, req);
            var objectResult = (ObjectResult)res2.Result;

            Assert.AreEqual(objectResult.StatusCode, 404);
        }

        [TestMethod]
        public void AddMovieToCollectionMovieNotFound()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey, ObjectId = 1 };

            var res2 = CollectionController.AddMovieToCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;

            Assert.AreEqual(objectResult.StatusCode, 404);
        }

        [TestMethod]
        public void AddMovieToCollectionBadReq()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var apiKey2 = "dsfsdf";

            var res = MovieController.SearchMovie("Harry Potter");
            var MovieRes = (ObjectResult)res.Result;
            var Movies = (List<ApiMovie>)(MovieRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey2, ObjectId = 1 };

            var res2 = CollectionController.AddMovieToCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;

            Assert.AreEqual(objectResult.StatusCode, 400);
        }

        [TestMethod]
        public void RemoveMovieFromCollectionTest()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = MovieController.SearchMovie("Harry Potter");
            var MovieRes = (ObjectResult)res.Result;
            var Movies = (List<ApiMovie>)(MovieRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey, ObjectId = Movies[0].Id };

            var r = CollectionController.AddMovieToCollection(col.Id, req);

            var res2 = CollectionController.RemoveMovieFromCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;
            var result = (ApiMovieCollection)(objectResult.Value);

            Assert.AreEqual(result.Movies.Count, 0);
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [TestMethod]
        public void RemoveMovietoCollectionNotFound()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = MovieController.SearchMovie("Harry Potter");
            var MovieRes = (ObjectResult)res.Result;
            var Movies = (List<ApiMovie>)(MovieRes.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey, ObjectId = Movies[0].Id };

            var res2 = CollectionController.RemoveMovieFromCollection(15454, req);
            var objectResult = (ObjectResult)res2.Result;

            Assert.AreEqual(objectResult.StatusCode, 404);
        }

        [TestMethod]
        public void RemoveMovieToCollectionMovieNotFound()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey, ObjectId = 1000000000 };

            var res2 = CollectionController.RemoveMovieFromCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;

            Assert.AreEqual(404, objectResult.StatusCode);
        }

        [TestMethod]
        public void RemoveMovieToCollectionBadReq()
        {
            var user = UserController.CreateUser(new RequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            var apiKey2 = "31561651";

            var res = MovieController.SearchMovie("Harry Potter");
            var MovieRes = (ObjectResult)res.Result;
            var Movies = (List<ApiMovie>)(MovieRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() { ApiKey = apiKey, Name = "Collection" });
            var colres = (ObjectResult)collection.Result;
            var col = (ApiMovieCollection)(colres.Value);

            var req = new ApiCollectionItemRequest() { ApiKey = apiKey2, ObjectId = 1 };

            var res2 = CollectionController.RemoveMovieFromCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;

            Assert.AreEqual(400, objectResult.StatusCode);
        }
    }
}
