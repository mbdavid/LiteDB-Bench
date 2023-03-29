class Results
{
    public TimeSpan Insert = TimeSpan.MaxValue;
    public TimeSpan Bulk = TimeSpan.MaxValue;
    public TimeSpan Update = TimeSpan.MaxValue;
    public TimeSpan Query = TimeSpan.MaxValue;
    public TimeSpan Delete = TimeSpan.MaxValue;

    public long Size;

    /// <summary>
    /// Format a long file length to pretty file unit
    /// </summary>
    public string SizeFormatted()
    {
        var suf = new[] { "b", "kb", "mb", "gb", "tb" }; //Longs run out around EB
        if (this.Size == 0) return "0" + suf[0];
        var bytes = Math.Abs(this.Size);
        var place = Convert.ToInt64(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(this.Size) * num).ToString() + " " + suf[place];
    }
}
