using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace CultureBox.DAO
{
    public interface IDbExecutor
    {
        void Execute(Action<LiteDatabase> method);
    }

    public class DbExecutor : IDbExecutor
    {
        private object SyncRoot { get; set; }

        public DbExecutor()
        {
            SyncRoot = new object();
        }

        public void Execute(Action<LiteDatabase> method)
        {
            lock (SyncRoot)
            {
                using (var db = new LiteDatabase(Program.DbPath))
                {
                    method?.Invoke(db);
                }
            }
        }
    }
}
