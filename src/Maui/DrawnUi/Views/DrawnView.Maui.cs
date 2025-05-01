using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DrawnUi.Infrastructure.Enums;

namespace DrawnUi.Views
{

    [ContentProperty("Children")]
    public partial class DrawnView : ContentView, IDrawnBase, IAnimatorsManager, IVisualTreeElement
    {


    }
}
