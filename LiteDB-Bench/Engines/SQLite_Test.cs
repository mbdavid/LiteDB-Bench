class SQLite_Test : ITest
{
    private string _filename;
    private SqliteConnection _db;
    public int FileLength { get { return (int)new FileInfo(_filename).Length; } }

    public SQLite_Test()
    {
        _filename = "sqlite-" + Guid.NewGuid().ToString("n") + ".db";
        var cs = "Data Source=" + _filename;

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

    public bool Insert(ProgressTask progress)
    {
        foreach (var doc in Helper.GetDocs(COUNT))
        {
            var cmd = new SqliteCommand("INSERT INTO col (id, name, lorem) VALUES (@id, @name, @lorem)", _db);

            cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));
            cmd.Parameters.Add(new SqliteParameter("name", DbType.String));
            cmd.Parameters.Add(new SqliteParameter("lorem", DbType.String));

            cmd.Parameters["id"].Value = doc["_id"].AsInt32;
            cmd.Parameters["name"].Value = doc["name"].AsString;
            cmd.Parameters["lorem"].Value = doc["lorem"].AsString;

            cmd.ExecuteNonQuery();

            progress.Increment(1);

            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) return false;
        }

        return true;
    }

    public bool Bulk(ProgressTask progress)
    {
        using var trans = _db.BeginTransaction();

        foreach (var doc in Helper.GetDocs(COUNT))
        {
            var cmd = new SqliteCommand("INSERT INTO col_bulk (id, name, lorem) VALUES (@id, @name, @lorem)", _db, trans);

            cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));
            cmd.Parameters.Add(new SqliteParameter("name", DbType.String));
            cmd.Parameters.Add(new SqliteParameter("lorem", DbType.String));

            cmd.Parameters["id"].Value = doc["_id"].AsInt32;
            cmd.Parameters["name"].Value = doc["name"].AsString;
            cmd.Parameters["lorem"].Value = doc["lorem"].AsString;

            cmd.ExecuteNonQuery();

            progress.Increment(1);

            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) return false;
        }

        trans.Commit();

        return true;
    }

    public bool Update(ProgressTask progress)
    {
        foreach (var doc in Helper.GetDocs(COUNT))
        {
            var cmd = new SqliteCommand("UPDATE col SET name = @name, lorem = @lorem WHERE id = @id", _db);

            cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));
            cmd.Parameters.Add(new SqliteParameter("name", DbType.String));
            cmd.Parameters.Add(new SqliteParameter("lorem", DbType.String));

            cmd.Parameters["id"].Value = doc["_id"].AsInt32;
            cmd.Parameters["name"].Value = doc["name"].AsString;
            cmd.Parameters["lorem"].Value = doc["lorem"].AsString;

            cmd.ExecuteNonQuery();

            progress.Increment(1);
        }

        return true;
    }

    public bool Query(ProgressTask progress)
    {
        for (var i = 1; i <= COUNT; i++)
        {
            var cmd = new SqliteCommand("SELECT * FROM col WHERE id = @id", _db);

            cmd.Parameters.Add(new SqliteParameter("id", DbType.Int32));

            cmd.Parameters["id"].Value = i;

            var r = cmd.ExecuteReader();

            r.Read();

            var name = r.GetString(1);
            var lorem = r.GetString(2);

            r.Close();

            progress.Increment(1);
        }

        return true;
    }

    public bool Delete(ProgressTask progress)
    {
        var cmd = new SqliteCommand("DELETE FROM col", _db);

        cmd.ExecuteNonQuery();

        progress.Increment(COUNT);

        return true;
    }

    public void Dispose()
    {
        _db.Close();
        _db.Dispose();

        try
        {
            File.Delete(_filename);
        }
        catch
        {
        }
    }
}
