using DrawnUi.Maui.Views;
using Sandbox.Views;

namespace MauiNet8;

public partial class MainPageRepro : BasePage
{
    int count = 0;

    public MainPageRepro()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }



}

public class DebugCanvas : Canvas
{


    public DebugCanvas()
    {
        var shape = CreateSkiaLine(Colors.White, 10, 10, 100, 100, 5);
        Children.Add(shape.Item1);
    }

    protected override void Draw(SkiaDrawingContext context, SKRect destination, float scale)
    {
        base.Draw(context, destination, scale);


        if (Height > 0 && Width > 0)
        {
            var canvas = context.Canvas;
            var res = CreateImage((int)context.Width, (int)context.Height, (float)(context.Width / Width));
            var image = res.Item2;

            //drawn image with rotation to illustrate missing antialiasing
            using var paint = new SKPaint() //max quality
            {
                IsAntialias = true,
                FilterQuality = SKFilterQuality.High,
                IsDither = true
            };

            var count = canvas.Save();

            canvas.RotateDegrees((float)-10, (float)context.Width / 2, (float)context.Height / 2);

            canvas.DrawImage(image, 50, 50, paint); //DrawBitmap gives same edgy not-antialiased effect

            canvas.RestoreToCount(count);

            res.Item1.Dispose();
            res.Item2.Dispose();



        }
    }

    private static (SkiaShape, Point) CreateSkiaLine(Color color, double x1, double y1, double x2, double y2, int width = 5)
    {
        var distX = x1 - x2;
        var distY = y1 - y2;
        var lineDist = Math.Sqrt(distX * distX + distY * distY);

        var lineCenterX = x2 + (distX / 2);
        var lineCenterY = y2 + (distY / 2);
        var lineFinalX = lineCenterX - (lineDist / 2);

        var lineRotateAngle = Math.Acos(distX / lineDist) * (180 / Math.PI);

        var line = new SkiaShape()
        {
            Type = ShapeType.Rectangle,
            BackgroundColor = color,
            WidthRequest = lineDist,
            HeightRequest = width,
            Rotation = (distY < 0) ? 360 - lineRotateAngle : lineRotateAngle
        };

        return (line, new Point(lineFinalX, lineCenterY));
    }

    (SKSurface, SKImage) CreateImage(int width, int height, float renderingScale)
    {
        var surfaceInfo = new SKImageInfo(width / 3, height / 3);
        var surface = SKSurface.Create(surfaceInfo);
        surface.Canvas.Clear(SKColors.White);

        // Calculate the margin
        float margin = 12 * renderingScale;

        // Define the rectangle inside the white rectangle
        var redRect = new SKRect(
            margin,
            margin,
            surfaceInfo.Width - margin,
            surfaceInfo.Height - margin
        );

        using (var paint = new SKPaint())
        {
            paint.Color = SKColors.Red;
            surface.Canvas.DrawRect(redRect, paint);
        }

        surface.Flush();
        var image = surface.Snapshot();
        return (surface, image);
    }

}
