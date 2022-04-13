﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultureBox.DAO;
using CultureBox.Model;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
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
        private readonly IMovieDao _movieDao;
        private readonly ISeriesDao _seriesDao;
        public const string IMDB = "https://imdb-api.com/fr";
        public const string SEARCH_MOVIE = "/API/SearchMovie/k_h13xs282/";
        public const string SEARCH_SERIES = "/API/SearchSeries/k_h13xs282/";
        

        public ApiMovieSerieController(IMovieDao movieDao, ISeriesDao seriesDao)
        {
            _movieDao = movieDao;
            _seriesDao = seriesDao;
        }

        public List<ApiMovie> SearchMovie(string title)
        {
            var client = new RestClient(IMDB);
            var request = new RestRequest(SEARCH_MOVIE+title, Method.Get);
            var t = (client.ExecuteAsync(request));
            t.Wait();
            if (t?.Result?.Content != null)
            {
                var res = JsonConvert.DeserializeObject<ImdbMovieResult>(t.Result.Content);
            }
            return null;
        }

        public List<ApiSeries> SearchSeries(string title)
        {
            var client = new RestClient(IMDB);
            var request = new RestRequest(SEARCH_SERIES + title, Method.Get);
            var t = (client.ExecuteAsync<List<object>>(request));
            t.Wait();
            var res = t.Result;

            return null;
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
        public string resultType { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }

    public class ImdbMovieResult
    {
        public string searchType { get; set; }
        public string expression { get; set; }
        public List<MovieResult> results { get; set; }
        public string errorMessage { get; set; }
    }
}