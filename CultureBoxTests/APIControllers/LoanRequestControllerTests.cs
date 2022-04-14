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
        public BookCollectionController CollectionController { get; set; }

        public UserController UserController { get; set; }
        public BookController BookController { get; set; }
        public LoanRequestController LoanRequestController { get; set; }
        public DbExecutor DbExecutor { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb2.db");
            CollectionController = new BookCollectionController(
                new UserDAO(DbExecutor),
                new BookCollectionDao(DbExecutor), 
                new BookDAO(DbExecutor)
            );
            UserController = new UserController(new UserDAO(DbExecutor));
            BookController = new BookController(new ApiBookController(new BookDAO(DbExecutor)));
            LoanRequestController = new LoanRequestController(
                new LoanRequestControllerDAO(DbExecutor), 
                new UserDAO(DbExecutor), 
                new BookCollectionDao(DbExecutor), 
                new MovieCollectionDao(DbExecutor), 
                new SeriesCollectionDAO(DbExecutor)
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
            var res1 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "", RequestObjectType = RequestObjectType.Book });
            var objectResult1 = (ObjectResult)res1.Result;
            Assert.AreEqual(400, objectResult1.StatusCode);
        }
                
        [TestMethod]
        public void TestSearchBookToBorrow_noBook() {
            // DB is truncated, no books
            var res2 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest(){Title = "Harry Potter", RequestObjectType = RequestObjectType.Book});
            var objectResult2 = (NotFoundResult)res2.Result;
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
            var result3 = (ApiBookCollection)(objectResult3).Value;
            
            var book = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiBook>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionController.AddBookToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiBookCollection)(objectResult4.Value);
            
            Assert.IsNotNull(result4);          
            Assert.AreEqual(200, objectResult4.StatusCode);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Book });

            var objectResult5 = (ObjectResult)res5.Result;
            Assert.AreEqual(200, objectResult5.StatusCode); 
            var searched = (List<ApiObjectToBorrow>)(objectResult5.Value); 
            Assert.AreEqual(1, searched.Count); 
        }
        
        [TestMethod]
        public void TestRequestLoan_BadCredentials() { 
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdObject = -1,
                ApiKey = "fezehf"
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
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
                IdObject = -1,
                ApiKey = apiKey
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
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
                IdObject = -1,
                ApiKey = apiKey
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
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
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiBookCollection)(objectResult1).Value;
            
            var book = BookController.SearchBook("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiBook>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionController.AddBookToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiBookCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Book });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new APIRequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            Assert.AreEqual(1, result5.Count);
            
            Assert.IsFalse(result5[0].IdOwner < 1);
            Assert.IsFalse(result5[0].IdObject < 1);

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2
            };

            var resReqLoan = LoanRequestController.RequestLoan(reqLoan);
            var objectResultReqLoan = (StatusCodeResult)resReqLoan;
            Assert.AreEqual(200, objectResultReqLoan.StatusCode);
            
            var reqGetBorrow = new LoanSearchRequest()
            {
                ApiKey = apiKey,
                RequestType = RequestType.Loan
            };
            var resSearch1 = LoanRequestController.GetAllRequests(reqGetBorrow);
            var objectResultSearch1 = (ObjectResult)resSearch1.Result;
            var resultSearch1 = (List<ApiLoanRequest>)(objectResultSearch1.Value);
            Assert.AreEqual(200, objectResultSearch1.StatusCode);
            Assert.AreEqual(1, resultSearch1.Count);
            
            var reqGetBorrow2 = new LoanSearchRequest()
            {
                ApiKey = apiKey2,
                RequestType = RequestType.Borrow
            };
            var resSearch2 = LoanRequestController.GetAllRequests(reqGetBorrow);
            var objectResultSearch2 = (ObjectResult)resSearch2.Result;
            var resultSearch2 = (List<ApiLoanRequest>)(objectResultSearch2.Value);
            Assert.AreEqual(200, objectResultSearch2.StatusCode);
            Assert.AreEqual(1, resultSearch2.Count);


            
            
        }
    }
}
