using System;
using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;
using LiteDB;

namespace CultureBox.DAO
{
    public interface IUserDAO
    {
        ApiUser GetUserById(int id);
        string GetApiKey(string username, string password);
        ApiUser CreateUser(string username, string password);
        bool DeleteUser(int id, string apiKey);
        List<ApiUser> GetAllUsers();
        int GetUserId(string apiKey);
    }

    public class UserDAO : IUserDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public UserDAO(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public ApiUser GetUserById(int id)
        {
            ApiUser user = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");
                
                col.EnsureIndex(x => x.Id);
                
                user = col.FindOne(x => x.Id == id);

                if (user != null)
                {
                    user.Password = "*****";
                    user.ApiKey = "*****";
                }
            });

            return user;
        }

        public string GetApiKey(string username, string password)
        {
            string apiKey = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");

                col.EnsureIndex(x => x.Id);

                apiKey = col.FindOne(x => x.Username == username && x.Password == password)?.ApiKey;
            });

            return apiKey;
        }

        public ApiUser CreateUser(string username, string password)
        {
            ApiUser userCreated = null;
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");

                var res = col.FindOne(u => u.Username == username);
                if (res == null)
                {
                    var user = new ApiUser()
                    {
                        Username = username,
                        Password = password,
                        ApiKey = GenerateApiKey(col)
                    };

                    int id = col.Insert(user);
                    userCreated = col.FindById(id);
                    if (userCreated != null)
                    {
                        userCreated.Password = "*****";
                    }
                }
            });

            return userCreated;
        }

        private string GenerateApiKey(ILiteCollection<ApiUser> col)
        {
            string res = null;
            while (res == null || col.Exists(u => res == u.ApiKey))
            {
                res = RandomString(10) + "_" + RandomString(10);
            }

            return res;
        }

        public static string RandomString(int length)
        {
            Random r = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public bool DeleteUser(int id, string apiKey)
        {
            bool isOk = false;
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");

                if (col.FindOne(x => x.Id == id && x.ApiKey == apiKey) != null)
                {
                    isOk = col.Delete(id);
                }
            });

            return isOk;
        }

        public List<ApiUser> GetAllUsers()
        {
            List<ApiUser> users = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");

                users = col.FindAll().ToList();

                if (users != null)
                {
                    foreach (var apiUser in users)
                    {
                        apiUser.Password = "*****";
                        apiUser.ApiKey = "*****";
                    }
                }
            });

            return users;
        }

        public int GetUserId(string apiKey)
        {
            int id = -1;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");
                var usr = col.FindOne(x => x.ApiKey == apiKey);

                if (usr != null)
                {
                    id = usr.Id;
                }
            });

            return id;
        }
    }
}

