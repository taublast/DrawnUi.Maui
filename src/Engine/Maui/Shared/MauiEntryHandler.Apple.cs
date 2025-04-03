 
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace DrawnUi.Controls;

 
public partial class MauiEntryHandler : EntryHandler
{
    protected override void ConnectHandler(MauiTextField platformView)
    {
        base.ConnectHandler(platformView);

        platformView.BorderStyle = UITextBorderStyle.None;
        
        platformView.BackgroundColor = UIColor.FromRGBA(0,0,0,0);
    }
    

}

