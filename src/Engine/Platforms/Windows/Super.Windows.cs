using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DrawnUi.Maui.Draw
{
    public partial class Super
    {
        public static void Init()
        {
            if (Initialized)
                return;

            Initialized = true;

            if (Super.NavBarHeight < 0)

                Super.NavBarHeight = 50; //manual

            Super.StatusBarHeight = 0;

            //VisualDiagnostics.VisualTreeChanged += OnVisualTreeChanged;

        }

    }
}

