using System;
using System.IO;
using LiteDB;

namespace CultureBox.DAO
{
    public interface IDbExecutor
    {
        void Execute(Action<LiteDatabase> method);
    }

    public class DbExecutor : IDbExecutor
    {
        private object SyncRoot { get; }

        public DbExecutor()
        {
            SyncRoot = new object();
            DbPath = Path.Combine(Directory.GetCurrentDirectory(), "culturebox.db");
        }

        public string DbPath { get; set; }

        public void Execute(Action<LiteDatabase> method)
        {
            lock (SyncRoot)
            {
                using (var db = new LiteDatabase(DbPath))
                {
                    method?.Invoke(db);
                }
            }
        }
    }
}
