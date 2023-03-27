using LiteDB_Bench;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteDB_Bench
{
    public class SQLite_Test : ITest
    {
        private string _filename;
        private SqliteConnection _db;
        private int _count;

        public int Count { get { return _count; } }
        public int FileLength { get { return (int)new FileInfo(_filename).Length; } }

        public SQLite_Test(int count, string? password = null)
        {
            _count = count;
            _filename = "sqlite-" + Guid.NewGuid().ToString("n") + ".db";
            var cs = "Data Source=" + _filename;
            if (password != null) cs += "; Password=" + password;
            cs += ";Pooling=false";

            _db = new SqliteConnection(cs);

            SQLitePCL.Batteries.Init();
        }

        public void Prepare()
        {
            _db.Open();

            var table = new SqliteCommand("CREATE TABLE col (id INTEGER NOT NULL PRIMARY KEY, name TEXT, lorem TEXT)", _db);
            table.ExecuteNonQuery();

            var table2 = new SqliteCommand("CREATE TABLE col_bulk (id INTEGER NOT NULL PRIMARY KEY, name TEXT, lorem TEXT)", _db);
            table2.ExecuteNonQuery();
        }

        public void Insert()
        {
            var cmd = new SqliteCommand("INSERT INTO col (id, name, lorem) VALUES (@id, @name, @lorem)", _db);

            cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));
            cmd.Parameters.Add(new SqliteParameter("name", DbType.String));
            cmd.Parameters.Add(new SqliteParameter("lorem", DbType.String));

            foreach (var doc in Helper.GetDocs(_count))
            {
                cmd.Parameters["id"].Value = doc["_id"].AsInt32;
                cmd.Parameters["name"].Value = doc["name"].AsString;
                cmd.Parameters["lorem"].Value = doc["lorem"].AsString;

                cmd.ExecuteNonQuery();
            }
        }

        public void Bulk()
        {
            using (var trans = _db.BeginTransaction())
            {
                var cmd = new SqliteCommand("INSERT INTO col_bulk (id, name, lorem) VALUES (@id, @name, @lorem)", _db, trans);

                cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));
                cmd.Parameters.Add(new SqliteParameter("name", DbType.String));
                cmd.Parameters.Add(new SqliteParameter("lorem", DbType.String));

                foreach (var doc in Helper.GetDocs(_count))
                {
                    cmd.Parameters["id"].Value = doc["_id"].AsInt32;
                    cmd.Parameters["name"].Value = doc["name"].AsString;
                    cmd.Parameters["lorem"].Value = doc["lorem"].AsString;

                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
            }
        }

        public void Update()
        {
            var cmd = new SqliteCommand("UPDATE col SET name = @name, lorem = @lorem WHERE id = @id", _db);

            cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));
            cmd.Parameters.Add(new SqliteParameter("name", DbType.String));
            cmd.Parameters.Add(new SqliteParameter("lorem", DbType.String));

            foreach (var doc in Helper.GetDocs(_count))
            {
                cmd.Parameters["id"].Value = doc["_id"].AsInt32;
                cmd.Parameters["name"].Value = doc["name"].AsString;
                cmd.Parameters["lorem"].Value = doc["lorem"].AsString;

                cmd.ExecuteNonQuery();
            }
        }

        public void Query()
        {
            var cmd = new SqliteCommand("SELECT * FROM col WHERE id = @id", _db);

            cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));

            for (var i = 1; i <= _count; i++)
            {
                cmd.Parameters["id"].Value = i;

                var r = cmd.ExecuteReader();

                r.Read();

                var name = r.GetString(1);
                var lorem = r.GetString(2);

                r.Close();
            }
        }

        public void Delete()
        {
            var cmd = new SqliteCommand("DELETE FROM col", _db);

            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _db.Close();
            _db.Dispose();

            File.Delete(_filename);
        }
    }
}
