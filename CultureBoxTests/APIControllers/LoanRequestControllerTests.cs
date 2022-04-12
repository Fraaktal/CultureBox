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
        public void TestGetAllRequests_noReqs()
        {
            // Create user 1 
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;
            
            // Its collection is empty, normal
            var req1 = new LoanSearchRequest()
            {
                RequestType = RequestType.All,
                ApiKey = apiKey
            };

            var res = LoanRequestController.GetAllRequests(req1);

            var objectResult = (ObjectResult)res.Result;
            var result = (List<ApiLoanRequest>)(objectResult).Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(result.Count, 0);
            
            // No API Key ?
            var req2 = new LoanSearchRequest()
            {
                RequestType = RequestType.Loan,
                ApiKey = ""
            };

            var res2 = LoanRequestController.GetAllRequests(req2);
            var objectResult2 = (ObjectResult)res2.Result;
            Assert.AreEqual(400, objectResult2.StatusCode);
            
            // Just a bad API key
            var req3 = new LoanSearchRequest()
            {
                RequestType = RequestType.Loan,
                ApiKey = "fezehf"
            };

            var res3 = LoanRequestController.GetAllRequests(req3);
            var objectResult3 = (ObjectResult)res3.Result;
            Assert.AreEqual(400, objectResult3.StatusCode);
            
        }
        
        [TestMethod]
        public void TestSearchBookToBorrow_noTitle() {
            // Bad request, no title
            var res1 = LoanRequestController.SearchBookToBorrow("");
            var objectResult1 = (ObjectResult)res1.Result;
            Assert.AreEqual(400, objectResult1.StatusCode);
        }
                
        [TestMethod]
        public void TestSearchBookToBorrow_noBook() {
            // DB is truncated, no books
            var res2 = LoanRequestController.SearchBookToBorrow("Harry Potter");
            var objectResult2 = (ObjectResult)res2.Result;
            Assert.AreEqual(404, objectResult2.StatusCode);
        }
      
        [TestMethod]
        public void TestSearchBookToBorrow() {
            // Create user 1 
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult3 = (ObjectResult)collection.Result;
            var result3 = (ApiCollection)(objectResult3).Value;
            
            var book = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiBook>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, BookId = books[0].Id};
            var res4 = CollectionController.AddBookToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiCollection)(objectResult4.Value);
            
            Assert.IsNotNull(result4);          
            Assert.AreEqual(200, objectResult4.StatusCode);
            
            var res5 = LoanRequestController.SearchBookToBorrow("Harry Potter");
            var objectResult5 = (ObjectResult)res5.Result;
            Assert.AreEqual(200, objectResult5.StatusCode); 
            var searched = (List<ApiBookToBorrow>)(objectResult5.Value); 
            Assert.AreEqual(1, searched.Count); 
        }
        
        [TestMethod]
        public void TestRequestLoan_BadCredentials() { 
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdBook = -1,
                ApiKey = "fezehf"
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3.Result;
            Assert.AreEqual(400, objectResult3.StatusCode);
            
        }
        
        [TestMethod]
        public void TestRequestLoan_SameUser() { 
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            int idUser = ((ApiUser)usr.Value).Id;    
            
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = idUser,
                IdBook = -1,
                ApiKey = apiKey
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3.Result;
            Assert.AreEqual(400, objectResult3.StatusCode); // Same User
            
        }
        
        [TestMethod]
        public void TestRequestLoan_BadBook() { 
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            int idUser = ((ApiUser)usr.Value).Id; 
            
            var user2 = UserController.CreateUser(new APIRequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result; 
            int idUser2 = ((ApiUser)usr2.Value).Id;    
        
            var req3 = new LoanRequest()
            {
                IdUser = idUser2,
                IdBook = -1,
                ApiKey = apiKey
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3.Result;
            Assert.AreEqual(404, objectResult3.StatusCode);
            
        }
        
        [TestMethod]
        public void TestRequestLoan() {
            // Create user 1 
            var user = UserController.CreateUser(new APIRequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult3 = (ObjectResult)collection.Result;
            var result3 = (ApiCollection)(objectResult3).Value;
            
            var book = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiBook>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, BookId = books[0].Id};
            var res4 = CollectionController.AddBookToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchBookToBorrow("Harry Potter");
            var objectResult5 = (ObjectResult)res5.Result;
            var searched = (List<ApiBookToBorrow>)(objectResult5.Value); 
            
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdBook = -1,
                ApiKey = "fezehf"
            };

            var res3 = LoanRequestController.GetAllRequests(req3);
            var objectResult3 = (ObjectResult)res3.Result;
            Assert.AreEqual(400, objectResult3.StatusCode);
            
            
        }
    }
}
