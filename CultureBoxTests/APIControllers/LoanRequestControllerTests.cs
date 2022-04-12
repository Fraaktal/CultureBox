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
            LoanRequestController = new LoanRequestController(
                new LoanRequestControllerDAO(DbExecutor), 
                new ApiBookController(new BookDAO(DbExecutor)), 
                new UserController(new UserDAO(DbExecutor), 
                new CollectionController(new UserDAO(DbExecutor),new CollectionDAO(DbExecutor), new BookDAO(DbExecutor))
            );
            BookController = new BookController(new ApiBookController(new BookDAO(DbExecutor)));
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }

        [TestMethod]
        public void TestGetAllRequests()
        {
            Assert.AreEqual(true, true);
        }
    }
}
