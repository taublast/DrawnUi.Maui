namespace DrawnUi.Internals;

public class SelectableAction : TitleWithStringId
{
    public SelectableAction()
    {
        Id = Guid.CreateVersion7().ToString();
    }
    public SelectableAction(string title, Action action)
    {
        Id = Guid.CreateVersion7().ToString();
        Title = title;
        Action = action;
    }

    public SelectableAction(string id, string title, Action action)
    {
        Id = id;
        Title = title;
        Action = action;
    }


    public Action Action { get; set; }
    public bool Selected { get; set; }

    public bool IsReadOnly { get; } = false;
}
