using LiteDB;

namespace LiteDB_Bench
{
    public class LiteDB_Test : ITest
    {
        private string _filename;
        private LiteDatabase _db;
        private ILiteCollection<BsonDocument> _col;
        private ILiteCollection<BsonDocument> _colBulk;
        private int _count;

        public int Count => _count;
        public int FileLength { get { _db.Checkpoint(); return (int)new FileInfo(_filename).Length; } }

        public LiteDB_Test(int count, string? password = null)
        {
            _count = count;
            _filename = "litedb-" + Guid.NewGuid().ToString("n") + ".db";

            _db = new LiteDatabase(new ConnectionString { Filename = _filename, Password = password });
            _col = _db.GetCollection("col");
            _colBulk = _db.GetCollection("col_bulk");
        }

        public void Prepare()
        {
        }

        public void Insert()
        {
            foreach (var doc in Helper.GetDocs(_count))
            {
                _col.Insert(doc);
            }
        }

        public void Bulk()
        {
            _colBulk.Insert(Helper.GetDocs(_count));
        }

        public void Update()
        {
            foreach (var doc in Helper.GetDocs(_count))
            {
                _col.Update(doc);
            }
        }

        public void Query()
        {
            for (var i = 1; i <= _count; i++)
            {
                _col.FindById(i);
            }
        }

        public void Delete()
        {
            _col.DeleteAll();
        }

        public void Dispose()
        {
            _db.Dispose();
            File.Delete(_filename);
        }
    }
}
