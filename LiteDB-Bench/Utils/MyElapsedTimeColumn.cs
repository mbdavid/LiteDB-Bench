using Spectre.Console.Rendering;
/// <summary>
/// A column showing the elapsed time of a task.
/// </summary>
public sealed class MyElapsedTimeColumn : ProgressColumn
{
    /// <inheritdoc/>
//    protected internal override bool NoWrap => true;

    protected override bool NoWrap => true;
    /// <summary>
    /// Gets or sets the style of the remaining time text.
    /// </summary>
    public Style Style { get; set; } = new Style(foreground: Color.Blue);

    /// <inheritdoc/>
    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        var elapsed = task.ElapsedTime;

        if (elapsed == null)
        {
            return new Markup("--".PadLeft(6));
        }

        return new Text($"{elapsed.Value.TotalMilliseconds,6:0,0} ms", Style ?? Style.Plain);
    }

    /// <inheritdoc/>
    public override int? GetColumnWidth(RenderOptions options)
    {
        return 10;
    }
}