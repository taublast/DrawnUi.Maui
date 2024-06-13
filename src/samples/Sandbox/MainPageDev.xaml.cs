
using AppoMobi.Specials;
using DrawnUi.Maui.Infrastructure;
using Sandbox;
using Sandbox.Resources.Strings;
using Sandbox.Views;
using Sandbox.Views.Xaml2Pdf;
using System.Diagnostics;
using System.Text;


namespace MauiNet8;

public class BuggedLayout : SkiaLayout
{
    protected override void Draw(SkiaDrawingContext context, SKRect destination, float scale)
    {
        base.Draw(context, destination, scale);

        //if (UsingCacheType == SkiaCacheType.ImageDoubleBuffered && RenderObject != null)
        //{
        //    Debug.WriteLine($"[D] {Superview.CanvasView.CanvasSize.Width} -> {RenderObject.Bounds.Width} at {DrawingRect.Width}");

        //    if (DrawingRect.Width != RenderObject.Bounds.Width)
        //    {
        //        InvalidateMeasure();
        //    }
        //}

    }
}

public partial class MainPageDev : BasePage
{
    public MainPageDev()
    {
        try
        {
            InitializeComponent();

            Test();
        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }

    void Test()
    {
        //string shaderCode = SkSl.LoadFromResources($"{MauiProgram.ShadersFolder}/transfade.sksl");
        //var effect = SkSl.Compile(shaderCode);
    }
}