interface ITest : IDisposable
{
    int FileLength { get; }

    void Prepare();
    void Insert(ProgressTask progress);
    void Bulk(ProgressTask progress);
    void Update(ProgressTask progress);
    void Query(ProgressTask progress);
    void Delete(ProgressTask progress);
}
