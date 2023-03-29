interface ITest : IDisposable
{
    int FileLength { get; }

    void Prepare();
    bool Insert(ProgressTask progress);
    bool Bulk(ProgressTask progress);
    bool Update(ProgressTask progress);
    bool Query(ProgressTask progress);
    bool Delete(ProgressTask progress);
}
