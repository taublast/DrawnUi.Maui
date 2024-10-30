using DrawnUi.Maui.Views;

namespace Sandbox;

public class MainPageBugCode : BasePageWithCanvas
{

    /*


    layout for stack :

    SkiaLayout:
        MeasureStack, cells measured in 2 passes -> LayoutCell -> cell.Arrange(..)


        SkiaControl:
            Arrange(..) ->
            PostArrange(..) ->
            AdaptCachedLayout(..) -> ArrangedDestination.Offset(destination.Left, destination.Top);

        In case of the bugged child:

        1. PostArrange from LayoutCell with destination = full canvas width
        2. PostArrange from Draw with destination = full canvas width
        3. PostArrange from DrawUsingRenderObject with destination = parent layout
        
     */


    private int _reloads;

    public override Canvas CreateCanvas()
    {
        _reloads++;

        return new Canvas()
        {
            Gestures = GesturesMode.Enabled,
            HardwareAcceleration = HardwareAccelerationMode.Enabled,

            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            WidthRequest = 300,
            HeightRequest = 300,
            BackgroundColor = Colors.Gray,

            Content = new SkiaLayout()
            {
                Tag = "Bugged",
                Padding = new Thickness(0),
                Type = LayoutType.Wrap,
                HeightRequest = 200,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0),
                Spacing = 8,
                BackgroundColor = Colors.Bisque,
                Children = new List<SkiaControl>()
                {

                    new SkiaLabel
                    {
                        ZIndex = 1,
                        Tag = "Label",
                        BackgroundColor = Colors.GhostWhite,
                        Text = $"Drawn {_reloads}",
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = 28
                    },

                    new SkiaShape
                    {
                        Tag = "Shape",
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.Center,
                        WidthRequest = 100,
                        HeightRequest = 100,
                        Type = ShapeType.Circle,
                        BackgroundColor = Colors.Orange
                    }

                }
            }
        };
    }



}