namespace Sandbox.Views
{
    public class BasePage : DrawnUiBasePage, IDisposable
    {
        public virtual void Dispose()
        {
            foreach (var child in InternalChildren)
            {
                if (child is IDisposable disposable)
                {
                    disposable?.Dispose();
                }
                else
                if (child is Grid grid)
                {
                    foreach (var gridChild in grid.Children)
                    {
                        if (gridChild is IDisposable disposableChild)
                        {
                            disposableChild?.Dispose();
                        }
                    }
                }
                else
                if (child is StackLayout stack)
                {
                    foreach (var stackChild in stack.Children)
                    {
                        if (stackChild is IDisposable disposableChild)
                        {
                            disposableChild?.Dispose();
                        }
                    }
                }
            }
        }
    }
}
