using System.Collections.Generic;
using System.IO;
using System.Linq;
using CultureBox.APIControllers;
using CultureBox.Control;
using CultureBox.DAO;
using CultureBox.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CultureBoxTests.APIControllers
{
    [TestClass]
    public class SeriesControllerTest
    {
        public SeriesController SeriesController { get; set; }
        public DbExecutor DbExecutor { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DbExecutor = new DbExecutor();
            DbExecutor.DbPath = Path.Combine(Directory.GetCurrentDirectory(), "testdb.db");
            SeriesController = new SeriesController(new ApiMovieSerieController(new MovieDao(DbExecutor), new SeriesDao(DbExecutor)));
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(DbExecutor.DbPath);
        }

        [TestMethod]
        public void SearchSeries()
        {
            var res = SeriesController.SearchSeries("House");

            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiSeries>)(objectResult).Value;

            Assert.IsNotNull(result.Count);
            Assert.AreNotEqual(0, result.Count);
            Assert.AreEqual(objectResult.StatusCode, 200);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var res = SeriesController.SearchSeries("House");

            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiSeries>)(objectResult).Value;


            var res2 = SeriesController.GetAll(10000);
            var objectResult2 = (OkObjectResult)res2.Result;
            var result2 = (List<ApiSeries>)(objectResult2).Value;

            Assert.IsNotNull(result2);
            Assert.AreEqual(result2.Count, result.Count);
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(objectResult2.StatusCode, 200);
        }

        [TestMethod]
        public void GetSeriesByIdTest()
        {
            var res = SeriesController.SearchSeries("House");

            var objectResult = (OkObjectResult)res.Result;
            var result = (List<ApiSeries>)(objectResult).Value;


            var res2 = SeriesController.GetSeriesById(1);
            var objectResult2 = (OkObjectResult)res2.Result;
            var result2 = (ApiSeries)(objectResult2).Value;

            Assert.IsNotNull(result2);
            Assert.IsTrue(result.Any(r => r.Title == result2.Title && r.Id == result2.Id));
            Assert.AreEqual(objectResult.StatusCode, 200);
            Assert.AreEqual(objectResult2.StatusCode, 200);
        }

        [TestMethod]
        public void GetSeriesByIdTest_NotFound()
        {
            var res = SeriesController.GetSeriesById(1);
            var objectResult = (ObjectResult)res.Result;

            Assert.AreEqual(404, objectResult.StatusCode);
        }

        [TestMethod]
        public void SearchSeriesNoTitle()
        {
            var res = SeriesController.SearchSeries(null);
            var objectResult = (ObjectResult)res.Result;
            Assert.AreEqual(400, objectResult.StatusCode);
        }
    }
}
