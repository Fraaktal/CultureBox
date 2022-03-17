using System.Collections.Generic;
using System.IO;
using CultureBox.APIControllers;
using CultureBox.Control;
using CultureBox.Model;
using CultureBox.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting

namespace CultureBoxTests.APIControllers
{
    [TestClass]
    public class CollectionControllerTests
    {
        public CollectionController CollectionController { get; set; }
        public DbExecutor DbExecutor { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExceutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            CollectionController = new CollectionController(new ApiCollectionController(new CollectionDAO(DbExecutor)));
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }
        
        [TestMethod]
        public void GetAllCollectionTest()
        {
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
        pulic void RemoveBookFromCollectionTest()
        {
        }
        
        [TestMethod]
        public void GetUserIdTest()
        {
        }
        
    }
}
