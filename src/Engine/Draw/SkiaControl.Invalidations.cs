using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawnUi.Maui.Draw;

public partial class SkiaControl
{

    //public static void CallInvalidateMeasure(SkiaControl control)
    //{
    //    control?.InvalidateMeasure();
    //}

    //protected readonly ConcurrentDictionary<string, Action<SkiaControl>> InvalidationActions = new();

    //protected readonly ConcurrentDictionary<string, bool> InvalidationFlags = new();

    //protected void SetInvalidationAction(string key, Action<SkiaControl> action)
    //{
    //    InvalidationActions[key] = action;
    //}

    //protected void InvalidateAction(string actionName)
    //{
    //    InvalidationFlags[actionName] = true;
    //}

    //public void ExecuteInvalidationActions()
    //{
    //    foreach (var key in InvalidationFlags.Keys)
    //    {
    //        if (InvalidationFlags[key])
    //        {
    //            InvalidationActions[key]?.Invoke(this);
    //            InvalidationFlags[key] = false;
    //        }
    //    }
    //}

}



