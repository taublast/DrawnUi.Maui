namespace DrawnUi.Maui.Draw;

public interface ISelectableOption : IHasTitleWithId, ICanBeSelected
{
    public bool IsReadOnly { get; }
}