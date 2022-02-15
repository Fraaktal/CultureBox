using System;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface IUserDAO
    {
        ApiUser GetUserById(int id);
        string GetApiKey(string username, string password);
        bool CreateUser(string username, string password);
        bool DeleteUser(int id, string password);
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

                apiKey = col.FindOne(x => x.Username == username && Equals(x.Password, password)).APIKey;
            });

            return apiKey;
        }

        public bool CreateUser(string username, string password)
        {
            bool isOk = false;
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");
                
                var user = new ApiUser()
                {
                    Username = username,
                    Password = password,
                    APIKey = new Guid().ToString()
                };
                
                isOk = col.Insert(user) >= 0;
            });

            return isOk;
        }

        public bool DeleteUser(int id, string password)
        {
            bool isOk = false;
            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiUser>("apiusers");

                if (col.FindOne(x => x.Id == id && Equals(x.Password, password)) != null)
                {
                    isOk = col.Delete(id);
                }
            });

            return isOk;
        }
    }
}

