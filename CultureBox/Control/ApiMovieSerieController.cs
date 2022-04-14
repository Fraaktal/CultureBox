using System.Collections.Generic;
using CultureBox.DAO;
using CultureBox.Model;
using Newtonsoft.Json;
using RestSharp;

namespace CultureBox.Control
{
    public interface IApiMovieSerieController
    {
        List<ApiMovie> SearchMovie(string title);
        List<ApiSeries> SearchSeries(string title);
        ApiMovie GetMovieById(int id);
        ApiSeries GetSeriesById(int id);
        List<ApiMovie> GetAllMovies(int resCount, int offset);
        List<ApiSeries> GetAllSeries(int resCount, int offset);
    }

    public class ApiMovieSerieController : IApiMovieSerieController
    {
        private readonly IMovieDAO _movieDao;
        private readonly ISeriesDao _seriesDao;
        public const string IMDB = "https://imdb-api.com/fr";
        public const string SEARCH_MOVIE = "/API/SearchMovie/k_h13xs282/";
        public const string SEARCH_SERIES = "/API/SearchSeries/k_h13xs282/";
        

        public ApiMovieSerieController(IMovieDAO movieDao, ISeriesDao seriesDao)
        {
            _movieDao = movieDao;
            _seriesDao = seriesDao;
        }

        public List<ApiMovie> SearchMovie(string title)
        {
            List<ApiMovie> movies = new List<ApiMovie>();
            var client = new RestClient(IMDB);
            var request = new RestRequest(SEARCH_MOVIE+title, Method.Get);
            var t = (client.ExecuteAsync(request));
            t.Wait();
            if (t?.Result?.Content != null)
            {
                var res = JsonConvert.DeserializeObject<ImdbMovieSeriesResult>(t.Result.Content);
                if (res != null)
                {
                    foreach (var imdb in res.results)
                    {
                        var movie = new ApiMovie() { Title = imdb.title };
                        _movieDao.AddOrUpdateMovie(movie);
                        movies.Add(movie);
                    }
                }
            }
            return movies;
        }

        public List<ApiSeries> SearchSeries(string title)
        {
            List<ApiSeries> series = new List<ApiSeries>();
            var client = new RestClient(IMDB);
            var request = new RestRequest(SEARCH_SERIES + title, Method.Get);
            var t = (client.ExecuteAsync(request));
            t.Wait();
            if (t?.Result?.Content != null)
            {
                var res = JsonConvert.DeserializeObject<ImdbMovieSeriesResult>(t.Result.Content);
                if (res != null)
                {
                    foreach (var imdb in res.results)
                    {
                        var serie = new ApiSeries() { Title = imdb.title };
                        _seriesDao.AddOrUpdateSeries(serie);
                        series.Add(serie);
                    }
                }
                
            }
            return series;
        }

        public ApiMovie GetMovieById(int id)
        {
            return _movieDao.GetMovieById(id);
        }

        public ApiSeries GetSeriesById(int id)
        {
            return _seriesDao.GetSeriesById(id);
        }

        public List<ApiMovie> GetAllMovies(int resCount, int offset)
        {
            return _movieDao.GetAllMovies(resCount, offset);
        }

        public List<ApiSeries> GetAllSeries(int resCount, int offset)
        {
            return _seriesDao.GetAllSeries(resCount, offset);
        }
    }

    public class MovieResult
    {
        public string id { get; set; }
        public string title { get; set; }
    }

    public class ImdbMovieSeriesResult
    {
        public List<MovieResult> results { get; set; }
    }
}
