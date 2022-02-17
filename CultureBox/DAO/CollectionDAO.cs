using System.Collections.Generic;
using System.Linq;
using CultureBox.Model;

namespace CultureBox.DAO
{
    public interface ICollectionDAO
    {
        List<ApiCollection> GetAllCollection(int id);
    }

    public class CollectionDAO: ICollectionDAO
    {
        private readonly IDbExecutor _dbExecutor;

        public CollectionDAO(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public List<ApiCollection> GetAllCollection(int id)
        {
            List<ApiCollection> res = null;

            _dbExecutor.Execute(db =>
            {
                var col = db.GetCollection<ApiCollection>("apicollection");
                res = col.Find(c => c.IdUser == id).ToList();
            });

            return res;
        }
    }
}
