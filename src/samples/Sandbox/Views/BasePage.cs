using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Views
{
    public class BasePage : DrawnUiBasePage, IDisposable
    {
        public void Dispose()
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
            }
        }
    }
}
