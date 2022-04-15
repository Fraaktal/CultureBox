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
        public MovieController MovieController { get; set; }
        public UserController UserController { get; set; }
        public BookController BookController { get; set; }
        public LoanRequestController LoanRequestController { get; set; }
        public SeriesController SeriesController { get; set; }
        public DbExecutor DbExecutor { get; set; }
        public MovieCollectionController CollectionMovieController { get; set; }
        public SeriesCollectionController CollectionSeriesController { get; set; }
        
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
            MovieController = new MovieController(
                new ApiMovieSerieController(
                    new MovieDao(DbExecutor), 
                    new SeriesDao(DbExecutor)
                )
            );
            SeriesController = new SeriesController(
                new ApiMovieSerieController(
                    new MovieDao(DbExecutor), 
                    new SeriesDao(DbExecutor)
                )
            );
            CollectionSeriesController = new SeriesCollectionController(new UserDAO(DbExecutor), new SeriesCollectionDAO(DbExecutor), new SeriesDao(DbExecutor));
            CollectionMovieController = new MovieCollectionController(new UserDAO(DbExecutor), new MovieCollectionDao(DbExecutor), new MovieDao(DbExecutor));
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
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
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
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
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
        public void TestSearchMovieToBorrow_noTitle() {
            // Bad request, no title
            var res1 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "", RequestObjectType = RequestObjectType.Movie });
            var objectResult1 = (ObjectResult)res1.Result;
            Assert.AreEqual(400, objectResult1.StatusCode);
        }
                
        [TestMethod]
        public void TestSearchMovieToBorrow_noBook() {
            // DB is truncated, no books
            var res2 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest(){Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie});
            var objectResult2 = (NotFoundResult)res2.Result;
            Assert.AreEqual(404, objectResult2.StatusCode);
        }
      
        [TestMethod]
        public void TestSearchMovieToBorrow() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionMovieController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult3 = (ObjectResult)collection.Result;
            var result3 = (ApiMovieCollection)(objectResult3).Value;
            
            var Movie = MovieController.SearchMovie("Harry Potter");
            var MovieRes = (ObjectResult)Movie.Result;
            var Movies = (List<ApiMovie>)(MovieRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = Movies[0].Id};
            var res4 = CollectionMovieController.AddMovieToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiMovieCollection)(objectResult4.Value);
            
            Assert.IsNotNull(result4);          
            Assert.AreEqual(200, objectResult4.StatusCode);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie });

            var objectResult5 = (ObjectResult)res5.Result;
            Assert.AreEqual(200, objectResult5.StatusCode); 
            var searched = (List<ApiObjectToBorrow>)(objectResult5.Value); 
            Assert.AreEqual(1, searched.Count); 
        }
        


        [TestMethod]
        public void TestSearchSeriesToBorrow_noTitle() {
            // Bad request, no title
            var res1 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "", RequestObjectType = RequestObjectType.Series });
            var objectResult1 = (ObjectResult)res1.Result;
            Assert.AreEqual(400, objectResult1.StatusCode);
        }
                
        [TestMethod]
        public void TestSearchSeriesToBorrow_noBook() {
            // DB is truncated, no books
            var res2 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest(){Title = "House", RequestObjectType = RequestObjectType.Series});
            var objectResult2 = (NotFoundResult)res2.Result;
            Assert.AreEqual(404, objectResult2.StatusCode);
        }
      
        [TestMethod]
        public void TestSearchSeriesToBorrow() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a Series to it
            var collection = CollectionSeriesController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult3 = (ObjectResult)collection.Result;
            var result3 = (ApiSeriesCollection)(objectResult3).Value;
            
            var Series = SeriesController.SearchSeries("House");
            var SeriesRes = (ObjectResult)Series.Result;
            var Seriess = (List<ApiSeries>)(SeriesRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = Seriess[0].Id};
            var res4 = CollectionSeriesController.AddSeriesToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiSeriesCollection)(objectResult4.Value);
            
            Assert.IsNotNull(result4);          
            Assert.AreEqual(200, objectResult4.StatusCode);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "House", RequestObjectType = RequestObjectType.Series });

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
        public void TestRequestLoan_BadCredentials_Movie() { 
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdObject = -1,
                ApiKey = "fezehf",
                RequestObjectType = RequestObjectType.Movie
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(400, objectResult3.StatusCode);
            
        }
        [TestMethod]
        public void TestRequestLoan_BadCredentials_Series() { 
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdObject = -1,
                ApiKey = "fezehf",
                RequestObjectType = RequestObjectType.Series
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(400, objectResult3.StatusCode);
            
        }
        
        
        
        [TestMethod]
        public void TestRequestLoan_SameUser() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
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
        public void TestRequestLoan_SameUser_Movie() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            int idUser = ((ApiUser)usr.Value).Id;    
            
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = idUser,
                IdObject = -1,
                ApiKey = apiKey,
                RequestObjectType = RequestObjectType.Movie
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(400, objectResult3.StatusCode); // Same User
            
        } 
        [TestMethod]
        public void TestRequestLoan_SameUser_Series() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            int idUser = ((ApiUser)usr.Value).Id;    
            
            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = idUser,
                IdObject = -1,
                ApiKey = apiKey,
                RequestObjectType = RequestObjectType.Series
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(400, objectResult3.StatusCode); // Same User
            
        }        
        


        
        [TestMethod]
        public void TestRequestLoan_BadUser() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey; 
            
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

            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(400, objectResult3.StatusCode); // Bad user
            
        }
        [TestMethod]
        public void TestRequestLoan_BadUser_Movie() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey; 
            
            var collection = CollectionMovieController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiMovieCollection)(objectResult1).Value;
            
            var book = MovieController.SearchMovie("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiMovie>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionMovieController.AddMovieToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiMovieCollection)(objectResult4.Value);

            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);

            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey,
                RequestObjectType = RequestObjectType.Movie
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(400, objectResult3.StatusCode); // Bad user
            
        }
        [TestMethod]
        public void TestRequestLoan_BadUser_Series() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey; 
            
            var collection = CollectionSeriesController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiSeriesCollection)(objectResult1).Value;
            
            var book = SeriesController.SearchSeries("House");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiSeries>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionSeriesController.AddSeriesToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiSeriesCollection)(objectResult4.Value);

            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "House", RequestObjectType = RequestObjectType.Series });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);

            // Just a bad API key
            var req3 = new LoanRequest()
            {
                IdUser = 0,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey,
                RequestObjectType = RequestObjectType.Series
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(400, objectResult3.StatusCode); // Bad user
            
        }




        
        [TestMethod]
        public void TestRequestLoan_BadBook() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            int idUser = ((ApiUser)usr.Value).Id; 
            
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
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
        public void TestRequestLoan_BadMovie() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            int idUser = ((ApiUser)usr.Value).Id; 
            
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result; 
            int idUser2 = ((ApiUser)usr2.Value).Id;    
        
            var req3 = new LoanRequest()
            {
                IdUser = idUser2,
                IdObject = -1,
                ApiKey = apiKey,
                RequestObjectType = RequestObjectType.Movie
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(404, objectResult3.StatusCode);
            
        }
        [TestMethod]
        public void TestRequestLoan_BadSeries() { 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            int idUser = ((ApiUser)usr.Value).Id; 
            
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result; 
            int idUser2 = ((ApiUser)usr2.Value).Id;    
        
            var req3 = new LoanRequest()
            {
                IdUser = idUser2,
                IdObject = -1,
                ApiKey = apiKey,
                RequestObjectType = RequestObjectType.Series
            };

            var res3 = LoanRequestController.RequestLoan(req3);
            var objectResult3 = (ObjectResult)res3;
            Assert.AreEqual(404, objectResult3.StatusCode);
            
        }
        


        [TestMethod]
        public void TestRequestLoan() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
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
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
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
        [TestMethod]
        public void TestRequestLoan_Movie() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionMovieController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiMovieCollection)(objectResult1).Value;
            
            var book = MovieController.SearchMovie("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiMovie>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionMovieController.AddMovieToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiMovieCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            Assert.AreEqual(1, result5.Count);
            
            Assert.IsFalse(result5[0].IdOwner < 1);
            Assert.IsFalse(result5[0].IdObject < 1);

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Movie
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
        [TestMethod]
        public void TestRequestLoan_Series() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionSeriesController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiSeriesCollection)(objectResult1).Value;
            
            var book = SeriesController.SearchSeries("House");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiSeries>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionSeriesController.AddSeriesToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiSeriesCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "House", RequestObjectType = RequestObjectType.Series });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            Assert.AreEqual(1, result5.Count);
            
            Assert.IsFalse(result5[0].IdOwner < 1);
            Assert.IsFalse(result5[0].IdObject < 1);

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Series
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



        [TestMethod]
        public void TestRequestLoan_borrowed() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
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
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            Assert.AreEqual(1, result5.Count);
            
            Assert.IsFalse(result5[0].IdOwner < 1);
            Assert.IsFalse(result5[0].IdObject < 1);

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Book
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
            
            // Can we get id ?
            var reqGet = LoanRequestController.GetRequestById(1, apiKey);
            var objetResReqGet = (ObjectResult)reqGet.Result;
            var resultReqGet = (ApiLoanRequest)(objetResReqGet.Value);
            Assert.AreEqual(200, objetResReqGet.StatusCode);
            Assert.AreEqual(1, resultReqGet.Id);
            Assert.IsNotNull(resultReqGet.IdOwner);
            Assert.IsNotNull(resultReqGet.IdRequester);

            var reqLoan2 = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Book
            };

            var resReqLoan2 = LoanRequestController.RequestLoan(reqLoan2);
            var objectResultReqLoan2 = (ObjectResult)resReqLoan2;
            Assert.AreEqual(400, objectResultReqLoan2.StatusCode); // Because already borrowed
        }
        [TestMethod]
        public void TestRequestLoan_borrowed_Movie() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionMovieController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiMovieCollection)(objectResult1).Value;
            
            var book = MovieController.SearchMovie("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiMovie>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionMovieController.AddMovieToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiMovieCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            Assert.AreEqual(1, result5.Count);
            
            Assert.IsFalse(result5[0].IdOwner < 1);
            Assert.IsFalse(result5[0].IdObject < 1);

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Movie
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
            
            // Can we get id ?
            var reqGet = LoanRequestController.GetRequestById(1, apiKey);
            var objetResReqGet = (ObjectResult)reqGet.Result;
            var resultReqGet = (ApiLoanRequest)(objetResReqGet.Value);
            Assert.AreEqual(200, objetResReqGet.StatusCode);
            Assert.AreEqual(1, resultReqGet.Id);
            Assert.IsNotNull(resultReqGet.IdOwner);
            Assert.IsNotNull(resultReqGet.IdRequester);

            var reqLoan2 = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Movie
            };

            var resReqLoan2 = LoanRequestController.RequestLoan(reqLoan2);
            var objectResultReqLoan2 = (ObjectResult)resReqLoan2;
            Assert.AreEqual(400, objectResultReqLoan2.StatusCode); // Because already borrowed
        }
        [TestMethod]
        public void TestRequestLoan_borrowed_Series() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionSeriesController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiSeriesCollection)(objectResult1).Value;
            
            var book = SeriesController.SearchSeries("House");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiSeries>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionSeriesController.AddSeriesToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiSeriesCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "House", RequestObjectType = RequestObjectType.Series });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            Assert.AreEqual(1, result5.Count);
            
            Assert.IsFalse(result5[0].IdOwner < 1);
            Assert.IsFalse(result5[0].IdObject < 1);

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Series
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

            
            // Can we get id ?
            var reqGet = LoanRequestController.GetRequestById(1, apiKey);
            var objetResReqGet = (ObjectResult)reqGet.Result;
            var resultReqGet = (ApiLoanRequest)(objetResReqGet.Value);
            Assert.AreEqual(200, objetResReqGet.StatusCode);
            Assert.AreEqual(1, resultReqGet.Id);
            Assert.IsNotNull(resultReqGet.IdOwner);
            Assert.IsNotNull(resultReqGet.IdRequester);


            var reqLoan2 = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Series
            };

            var resReqLoan2 = LoanRequestController.RequestLoan(reqLoan2);
            var objectResultReqLoan2 = (ObjectResult)resReqLoan2;
            Assert.AreEqual(400, objectResultReqLoan2.StatusCode); // Because already borrowed


        }

        [TestMethod]
        public void TestGetRequestById_InvalidCreds() {

            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
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
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Book
            };

            var resReqLoan = LoanRequestController.RequestLoan(reqLoan);
            var objectResultReqLoan = (StatusCodeResult)resReqLoan;

            var reqGet = LoanRequestController.GetRequestById(1, "eie");
            var objetResReqGet = (ObjectResult)reqGet.Result;
            Assert.AreEqual(400, objetResReqGet.StatusCode); // Bad Request, API KEY not exists

            var reqGet2 = LoanRequestController.GetRequestById(1, "");
            var objetResReqGet2 = (ObjectResult)reqGet2.Result;
            Assert.AreEqual(400, objetResReqGet2.StatusCode); // Bad Request, API KEY not exists

        }
        [TestMethod]
        public void TestGetRequestById_BadUser() {

            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionMovieController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiMovieCollection)(objectResult1).Value;
            
            var book = MovieController.SearchMovie("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiMovie>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionMovieController.AddMovieToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiMovieCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Movie
            };

            var resReqLoan = LoanRequestController.RequestLoan(reqLoan);
            var objectResultReqLoan = (StatusCodeResult)resReqLoan;

            // Create user 3
            var user3 = UserController.CreateUser(new RequestUser() {Username = "test3", Password = "test3"});
            var usr3 = (ObjectResult)user3.Result;
            string apiKey3 = ((ApiUser)usr3.Value).ApiKey;  

            var reqGet = LoanRequestController.GetRequestById(1, apiKey3);
            var objetResReqGet = (ObjectResult)reqGet.Result;
            Assert.AreEqual(400, objetResReqGet.StatusCode); // Bad Request, La demande ne lui appartient pas

        }
        [TestMethod]
        public void TestGetRequestById_NotFound() {

            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  

            var reqGet = LoanRequestController.GetRequestById(1, apiKey);
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(404, objetResReqGet.StatusCode); // Not found, no request

        }
        [TestMethod]
        public void TestUpdateReq_BadCred() {
            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Pending, ApiKey = ""} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(400, objetResReqGet.StatusCode); // No APIKEY

            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            
            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Pending, ApiKey = "apiKey"} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(400, objetResReqGet.StatusCode); // Bad APIKEY

        }
        [TestMethod]
        public void TestUpdateReq_NotFound() {
            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;  
            
            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Pending, ApiKey = apiKey} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(404, objetResReqGet.StatusCode); // No request 

        }
        [TestMethod]
        public void TestUpdateReq_BadUser() {

            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionMovieController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiMovieCollection)(objectResult1).Value;
            
            var book = MovieController.SearchMovie("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiMovie>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionMovieController.AddMovieToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiMovieCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Movie
            };

            var resReqLoan = LoanRequestController.RequestLoan(reqLoan);
            var objectResultReqLoan = (StatusCodeResult)resReqLoan;

            // Create user 3
            var user3 = UserController.CreateUser(new RequestUser() {Username = "test3", Password = "test3"});
            var usr3 = (ObjectResult)user3.Result;
            string apiKey3 = ((ApiUser)usr3.Value).ApiKey;  

            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Pending, ApiKey = apiKey3} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(400, objetResReqGet.StatusCode); // INVALID_CREDENTIALS (not the borrower and not the loaner)

        }
        [TestMethod]
        public void TestUpdateReq_BadUser() {

            // Create user 1 
            var user = UserController.CreateUser(new RequestUser() {Username = "test", Password = "test"});
            var usr = (ObjectResult)user.Result;
            string apiKey = ((ApiUser)usr.Value).ApiKey;            
            
            // Create a collection, add a book to it
            var collection = CollectionMovieController.CreateCollection(new ApiCollectionRequest() {ApiKey = apiKey, Name = "Collection"});
            var objectResult1 = (ObjectResult)collection.Result;
            var result3 = (ApiMovieCollection)(objectResult1).Value;
            
            var book = MovieController.SearchMovie("Harry Potter");
            var bookRes = (ObjectResult)book.Result;
            var books = (List<ApiMovie>)(bookRes.Value);
            
            var req = new ApiCollectionItemRequest(){ApiKey = apiKey, ObjectId = books[0].Id};
            var res4 = CollectionMovieController.AddMovieToCollection(result3.Id, req);
            var objectResult4 = (ObjectResult)res4.Result;
            var result4 = (ApiMovieCollection)(objectResult4.Value);
            
            var res5 = LoanRequestController.SearchObjectToBorrow(new SearchObjectToBorrowRequest() { Title = "Harry Potter", RequestObjectType = RequestObjectType.Movie });
            var objectResult5 = (ObjectResult)res5.Result;
            var result5 = (List<ApiObjectToBorrow>)(objectResult5.Value);
            
            // Create user 2
            var user2 = UserController.CreateUser(new RequestUser() {Username = "test2", Password = "test2"});
            var usr2 = (ObjectResult)user2.Result;
            string apiKey2 = ((ApiUser)usr2.Value).ApiKey;  

            var reqLoan = new LoanRequest()
            {
                IdUser = result5[0].IdOwner,
                IdObject = result5[0].IdObject,
                ApiKey = apiKey2,
                RequestObjectType = RequestObjectType.Movie
            };

            var resReqLoan = LoanRequestController.RequestLoan(reqLoan);
            var objectResultReqLoan = (StatusCodeResult)resReqLoan;

            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Accepted, ApiKey = apiKey3} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(200, objetResReqGet.StatusCode); // Ok

            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Refused, ApiKey = apiKey3} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(200, objetResReqGet.StatusCode); // Ok

            
            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Ongoing, ApiKey = apiKey3} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(200, objetResReqGet.StatusCode); // Ok

            var reqGet = LoanRequestController.UpdateLoanRequest(1, new ApiLoanRequestUpdate() {RequestState = RequestState.Ended, ApiKey = apiKey3} );
            var objetResReqGet = (StatusCodeResult)reqGet.Result;
            Assert.AreEqual(200, objetResReqGet.StatusCode); // Ok

        }
    }
}
