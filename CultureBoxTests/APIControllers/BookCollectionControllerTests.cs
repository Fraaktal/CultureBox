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
    public class BookCollectionControllerTests
    {
        public BookCollectionController CollectionController { get; set; }

        public UserController UserController { get; set; }
        public BookController BookController { get; set; }
        public DbExecutor DbExecutor { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            CollectionController = new BookCollectionController(new UserDAO(DbExecutor),new BookCollectionDao(DbExecutor), new BookDAO(DbExecutor));
            UserController = new UserController(new UserDAO(DbExecutor));
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
            var result = (List<ApiBookCollection>)(objectResult).Value;

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
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result1 = (ApiBookCollection)(objectResult1).Value;

            var res = CollectionController.GetAllCollection(apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiBookCollection>)(objectResult).Value;

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
            var col = (ApiBookCollection)(colres.Value);

            var res = CollectionController.GetCollectionById(col.Id, apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (ApiBookCollection)(objectResult.Value);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);
        }
 
        [TestMethod]
        public void GetCollectionByIdNotFound()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var res = CollectionController.GetCollectionById(3, apiKey);
            var objectResult = (ObjectResult)res.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 404);
        }
        
        [TestMethod]
        public void GetCollectionByIdBadReq()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            var apiKey2 = "dfgdfgdfg";
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);
            
            var res = CollectionController.GetCollectionById(col.Id, apiKey2);
            var objectResult = (ObjectResult)res.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 400);
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
            var result = (ApiBookCollection)(objectResult.Value);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.IsNotNull(result);   
        }
        
        [TestMethod]
        public void CreateCollectionBadReqNameTaken()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var req = new ApiCollectionRequest(){ApiKey= apiKey, Name= "Collection"};
            
            var res = CollectionController.CreateCollection(req);
            res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 400);
        } 
        
        [TestMethod]
        public void CreateCollectionBadReqEmpty()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var req = new ApiCollectionRequest(){ApiKey= apiKey, Name= "Collection"};
            
            var res = CollectionController.CreateCollection(null);
            var objectResult = (ObjectResult)res.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 400);
        }
        
        [TestMethod]
        public void CreateCollectionBadReqNameEmpty()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var req = new ApiCollectionRequest(){ApiKey= apiKey, Name= ""};
            
            var res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 400);
        }
        
        [TestMethod]
        public void CreateCollectionBadReqInvalid()
        {
            var apiKey = "dfsdfsd";
            
            var req = new ApiCollectionRequest(){ApiKey= apiKey, Name= "Collection"};
            
            var res = CollectionController.CreateCollection(req);
            var objectResult = (ObjectResult)res.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 400);
        }
        
        
        
        [TestMethod]
        public void DeleteCollection()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);

            CollectionController.DeleteCollection(col.Id, apiKey);
            var res = CollectionController.GetAllCollection(apiKey);
            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiBookCollection>)(objectResult).Value;
            
            Assert.IsNotNull(result);          
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
        }
        
        [TestMethod]
        public void DeleteCollectionNotFound()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var res = CollectionController.DeleteCollection(0, apiKey);
            var objectResult = (ObjectResult)res;
              
            Assert.AreEqual(404, objectResult.StatusCode);
        }
        
        [TestMethod]
        public void DeleteCollectionBadReq()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            var apiKey2 = "dvsdf";
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);

            
            var res = CollectionController.DeleteCollection(col.Id, apiKey2);
            var objectResult = (ObjectResult)res;
            
            Assert.AreEqual(objectResult.StatusCode, 400);
        }
        
        [TestMethod]
        public void AddBookToCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};

            var res2 = CollectionController.AddBookToCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;
            var result = (ApiBookCollection)(objectResult.Value);
            
            Assert.IsNotNull(result);          
            Assert.AreEqual(objectResult.StatusCode, 200);
        }
        
        [TestMethod]
        public void AddBooktoCollectionNotFound()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};

            var res2 = CollectionController.AddBookToCollection(1000, req);
            var objectResult = (ObjectResult)res2.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 404);
        }
        
        [TestMethod]
        public void AddBookToCollectionBookNotFound()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = 1};

            var res2 = CollectionController.AddBookToCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 404);
        }
        
        [TestMethod]
        public void AddBookToCollectionBadReq()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var apiKey2 = "dsfsdf";

            var res = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey2, ObjectId = 1};

            var res2 = CollectionController.AddBookToCollection(col.Id, req);
            var objectResult = (ObjectResult)res2.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 400);
        }
        
        [TestMethod]
        public void RemoveBookFromCollectionTest()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);

            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"}); 
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};

            var r = CollectionController.AddBookToCollection(col.Id, req);
            
            var res2 = CollectionController.RemoveBookFromCollection(col.Id, req.ObjectId, req.ApiKey);
            var objectResult = (ObjectResult)res2.Result;
            var result = (ApiBookCollection)(objectResult.Value);
            
            Assert.AreEqual(result.Books.Count, 0);          
            Assert.AreEqual(objectResult.StatusCode, 200);
        }
        
        [TestMethod]
        public void RemoveBooktoCollectionNotFound()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;

            var res = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};

            var res2 = CollectionController.RemoveBookFromCollection(15454, req.ObjectId, req.ApiKey);
            var objectResult = (ObjectResult)res2.Result;
            
            Assert.AreEqual(objectResult.StatusCode, 404);
        }
        
        [TestMethod]
        public void RemoveBookToCollectionBookNotFound()
        {
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);

            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = 1000000000};

            var res2 = CollectionController.RemoveBookFromCollection(col.Id, req.ObjectId, req.ApiKey);
            var objectResult = (ObjectResult)res2.Result;
            
            Assert.AreEqual(404, objectResult.StatusCode);
        }
        
        [TestMethod]
        public void RemoveBookToCollectionBadReq()
        {
            var user = UserController.CreateUser(new APIRequestUser() { Username = "test", Password = "test" });
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            var apiKey2 = "31561651";
                        
            var res = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)res.Result;
            var books = (List<ApiBook>)(bookRes.Value);
            
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var colres = (ObjectResult)collection.Result;
            var col = (ApiBookCollection)(colres.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey2, ObjectId = 1};

            var res2 = CollectionController.RemoveBookFromCollection(col.Id, req.ObjectId, req.ApiKey);
            var objectResult = (ObjectResult)res2.Result;
            
            Assert.AreEqual(400, objectResult.StatusCode);
        }
    }
}
