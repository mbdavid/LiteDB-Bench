class LiteDB_Test : ITest
{
    private string _filename;
    private LiteDatabase _db;
    private ILiteCollection<BsonDocument> _col;
    private ILiteCollection<BsonDocument> _colBulk;
    public int FileLength { get { _db.Checkpoint(); return (int)new FileInfo(_filename).Length; } }

    public LiteDB_Test()
    {
        _filename = "litedb-" + Guid.NewGuid().ToString("n") + ".db";

        _db = new LiteDatabase(new ConnectionString { Filename = _filename });
        _col = _db.GetCollection("col");
        _colBulk = _db.GetCollection("col_bulk");
    }

    public void Prepare()
    {
    }

    public bool Insert(ProgressTask progress)
    {
        foreach (var doc in Helper.GetDocs(COUNT))
        {
            _col.Insert(doc);

            progress.Increment(1);

            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) return false;
        }

        return true;
    }

    public bool Bulk(ProgressTask progress)
    {
        _colBulk.Insert(Helper.GetDocs(COUNT));

        progress.Increment(COUNT);

        return true;
    }

    public bool Update(ProgressTask progress)
    {
        foreach (var doc in Helper.GetDocs(COUNT))
        {
            _col.Update(doc);

            progress.Increment(1);

            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) return false;
        }

        return true;
    }

    public bool Query(ProgressTask progress)
    {
        for (var i = 1; i <= COUNT; i++)
        {
            _col.FindById(i);

            progress.Increment(1);

            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) return false;
        }

        return true;
    }

    public bool Delete(ProgressTask progress)
    {
        _col.DeleteAll();

        progress.Increment(COUNT);

        return true;
    }

    public void Dispose()
    {
        _db.Dispose();

        File.Delete(_filename);
    }
}
