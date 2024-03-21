 
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace DrawnUi.Maui.Controls;

 /// <summary>
 /// TODO
 /// </summary>
public partial class MauiEditorHandler : EditorHandler
{
    protected override void ConnectHandler(MauiTextView platformView)
    {
        base.ConnectHandler(platformView);

        platformView.BackgroundColor = UIColor.FromRGBA(0,0,0,0);
    }

}

