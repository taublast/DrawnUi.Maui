namespace DrawnUi.Maui.Controls;


public partial class MauiEntry : Entry, IEditor
{
    public MauiEntry()
    {
        base.Completed += MauiEntry_Completed;
    }

    private void MauiEntry_Completed(object sender, EventArgs e)
    {
        OnCompleted?.Invoke(this, null);
    }

    public static readonly BindableProperty MaxLinesProperty = BindableProperty.Create(nameof(MaxLines),
        typeof(int), typeof(MauiEntry), 1);
    /// <summary>
    /// WIth 1 will behave like an ordinary Entry, with -1 (auto) or explicitly set you get an Editor
    /// </summary>
    public int MaxLines
    {
        get { return (int)GetValue(MaxLinesProperty); }
        set { SetValue(MaxLinesProperty, value); }
    }

    /// <summary>
    /// Occurs when the user finalizes the text in an entry with the return key.
    /// </summary>
    public event EventHandler OnCompleted;

    public new void Completed()
    {
        OnCompleted?.Invoke(this, null);
    }
}