class Program
{
    public static int COUNT = 10;

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("usage: dotnet run <count>");
            return;
        }

        Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");

        COUNT = Convert.ToInt32(args[0]);

        AnsiConsole.Clear();

        AnsiConsole.Markup($"[bold teal]Embedded Database Performance Benchmark[/]\n");
        AnsiConsole.Markup($"[bold teal]=======================================[/]\n\n");

        // LiteDB
        var litedbTest = new LiteDB_Test();
        var litedbResult = new Results();

        RunTest($"LiteDB ({COUNT:0,0} documents)", litedbTest, litedbResult);

        // SQLite
        var sqliteTest = new SQLite_Test();
        var sqliteResult = new Results();

        RunTest($"SQLite ({COUNT:0,0} rows)", sqliteTest, sqliteResult);

        var table = new Table()
            .AddColumn("Database")
            .AddColumn("Insert", c => c.Alignment = Justify.Right)
            .AddColumn("Bulk", c => c.Alignment = Justify.Right)
            .AddColumn("Update", c => c.Alignment = Justify.Right)
            .AddColumn("Query", c => c.Alignment = Justify.Right)
            .AddColumn("Delete", c => c.Alignment = Justify.Right)
            .AddColumn("Size", c => c.Alignment = Justify.Right);

        AddRow(table, "LiteDB", litedbResult, sqliteResult);
        AddRow(table, "SQLite", sqliteResult, litedbResult);

        AnsiConsole.Write(table);
    }

    static void AddRow(Table table, string name, Results results, Results other)
    {
        table.AddRow(new Text(name),
            AddText(results.Insert, other.Insert),
            AddText(results.Bulk, other.Bulk),
            AddText(results.Update, other.Update),
            AddText(results.Query, other.Query),
            AddText(results.Delete, other.Delete),
            new Text(results.SizeFormatted()));
    }

    static Markup AddText(TimeSpan result, TimeSpan other)
    {
        var color = result > other ? "red" : "green";

        return new Markup($"[{color}]{result.TotalMilliseconds:0,0} ms[/]");
    }

    static void RunTest(string name, ITest test, Results results)
    {
        try
        {
            test.Prepare();

            AnsiConsole.Markup($"[bold darkorange]{name}[/]\n");
            AnsiConsole.Markup($"[bold darkorange]{"".PadLeft(name.Length, '=')}[/]\n");

            AnsiConsole.Progress()
                .AutoRefresh(true) // Turn off auto refresh
                .AutoClear(false)   // Do not remove the task list when done
                .HideCompleted(false)   // Hide tasks as they are completed
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new MyElapsedTimeColumn(),
                    new SpinnerColumn(),
                }).Start(ctx =>
                {
                    // Define tasks
                    var tInsert = ctx.AddTask("[silver] Insert[/]", false, COUNT);
                    var tBulk = ctx.AddTask("[silver] Bulk[/]", false, COUNT);
                    var tUpdate = ctx.AddTask("[silver] Update[/]", false, COUNT);
                    var tQuery = ctx.AddTask("[silver] Query[/]", false, COUNT);
                    var tDelete = ctx.AddTask("[silver] Delete[/]", false, COUNT);

                    RunTask("Insert", tInsert, test.Insert);
                    RunTask("Bulk", tBulk, test.Bulk);
                    RunTask("Update", tUpdate, test.Update);
                    RunTask("Query", tQuery, test.Query);
                    RunTask("Delete", tDelete, test.Delete);

                    results.Insert = tInsert.ElapsedTime!.Value;
                    results.Bulk = tBulk.ElapsedTime!.Value;
                    results.Update= tUpdate.ElapsedTime!.Value;
                    results.Query = tQuery.ElapsedTime!.Value;
                    results.Delete = tDelete.ElapsedTime!.Value;
                    results.Size = test.FileLength;

                });

        }
        finally
        {
            test.Dispose();
        }
    }

    static void RunTask(string name, ProgressTask progress, Action<ProgressTask> action)
    {
        progress.Description($"[lightslateblue] {name}[/]");
        progress.StartTask();

        action(progress);

        progress.Description($"[silver] {name}[/]");
        progress.StopTask();
    }
}
