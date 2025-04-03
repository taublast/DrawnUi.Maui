using AppoMobi.Specials;
using DrawnUi.Draw;
using SkiaSharp;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;
using static UnitTests.RenderingTests;
using SkiaLayout = DrawnUi.Draw.SkiaLayout;

namespace UnitTests
{
    public class SkiaLayoutTests : DrawnTestsBase
    {
        [Fact]
        public void ItemsSourceNotSet()
        {
            var layout = new SkiaLayout
            {
                Type = LayoutType.Column
            };

            ObservableCollection<int> itemsSource = null;
            layout.ItemsSource = itemsSource;
            layout.ItemTemplate = new DataTemplate(() => new SkiaControl());

            layout.CommitInvalidations();
            layout.Measure(100, 100, 1);

            Assert.True(LayoutStructureCorrespondsToItemsSource(itemsSource, layout));
        }

        [Fact]
        public void ItemsSourceEmpty()
        {
            var layout = new SkiaLayout
            {
                Type = LayoutType.Column
            };

            var itemsSource = new ObservableCollection<int>();
            layout.ItemsSource = itemsSource;
            layout.ItemTemplate = new DataTemplate(() => new SkiaControl());

            layout.CommitInvalidations();
            layout.Measure(100, 100, 1);

            Assert.True(LayoutStructureCorrespondsToItemsSource(itemsSource, layout));
        }

        [Fact]
        public void ItemsSourceNotEmpty()
        {
            var layout = new SkiaLayout
            {
                Type = LayoutType.Column
            };

            var itemsSource = new ObservableCollection<int>() { 1, 2, 3, 4, 5 };
            layout.ItemsSource = itemsSource;
            layout.ItemTemplate = new DataTemplate(() => new SkiaControl());

            layout.CommitInvalidations();
            layout.Measure(100, 100, 1);

            Assert.True(LayoutStructureCorrespondsToItemsSource(itemsSource, layout));
        }

        [Fact]
        public async Task NoLeaksDisposedLayoutWithChildren()
        {
            var layout = CreateSampleLayoutWIthChildren();
            var image = layout.FindView<SkiaImage>("Image");

            var layoutRef = new WeakReference(layout);
            var childRef = new WeakReference(image);

            var destination = new SKRect(0, 0, 100, 100);
            layout.CommitInvalidations();
            layout.Measure(destination.Width, destination.Height, 1);
            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx);
            });

            layout.Dispose();

            Assert.True(layout.IsDisposed);
            Assert.True(image.IsDisposing);

            await Task.Delay(10000);

            Assert.True(image.IsDisposed, "child failed to dispose in due time");

            image = null;
            layout = null;

            // First GC
            await Task.Yield();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.False(layoutRef.IsAlive, "layout should not be alive!");

            // Second GC
            //await Task.Yield();
            await Task.Delay(1500);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.False(childRef.IsAlive, "child should not be alive!");
        }

        [Fact]
        public void AbsoluteTypeRespectZIndex()
        {
            var layout = CreateAbsoluteLayoutSampleWIthChildren();

            var destination = new SKRect(0, 0, 100, float.PositiveInfinity);
            layout.CommitInvalidations();
            layout.Measure(destination.Width, destination.Height, 1);

            //prepare DrawingRect
            layout.Arrange(new SKRect(0, 0, layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height),
                layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height, 1);

            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx.WithDestination(layout.DrawingRect));
            });

            var cache = layout.RenderObject;
            var pixels = cache.Image.PeekPixels();
            var color = pixels.GetPixelColor(0, 0);

            Assert.Equal(color, SKColors.Red);
        }

        [Fact]
        public void ColumnTypeRespectZIndex()
        {
            var layout = new SkiaLayout
            {
                Type = LayoutType.Column,
                BackgroundColor = Colors.Black,
                Spacing = 0,
                UseCache = SkiaCacheType.Image,
                Children = new List<SkiaControl>()
                {
                    new SkiaShape()
                    {
                        ZIndex = 0,
                        Tag = "Green",
                        BackgroundColor = Colors.Green,
                        HeightRequest=100,
                        LockRatio=1,
                    },
                    new SkiaShape()
                    {
                        AddMarginTop=-100,
                        ZIndex = 1,
                        Tag = "Red",
                        BackgroundColor = Colors.Red,
                        HeightRequest=100,
                        LockRatio=1,
                    },
                    new SkiaShape()
                    {
                        //AddMarginTop=-200,
                        Tag = "Blue",
                        BackgroundColor = Colors.Blue,
                        HeightRequest=100,
                        LockRatio=-1,
                    },
                }
            };

            var destination = new SKRect(0, 0, 100, float.PositiveInfinity);
            layout.Measure(destination.Width, destination.Height, 1);

            //prepare DrawingRect
            layout.Arrange(new SKRect(0, 0, layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height),
                layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height, 1);

            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx.WithDestination(layout.DrawingRect));
            });

            var cache = layout.RenderObject;
            var pixels = cache.Image.PeekPixels();
            var color = pixels.GetPixelColor(0, 0);
            Assert.Equal(color, SKColors.Red);
        }

        [Fact]
        public void AbsoluteTypePaddingOk()
        {
            var layout = new SkiaLayout
            {
                Padding = new Thickness(16),
                Type = LayoutType.Absolute,
                WidthRequest = 100,
                Spacing = 0,
                UseCache = SkiaCacheType.Image,
                Children = new List<SkiaControl>()
                {
                    new SkiaShape()
                    {
                        ZIndex = 0,
                        Tag = "Green",
                        BackgroundColor = Colors.Green,
                        HeightRequest=100,
                        LockRatio=1,
                    },
                    new SkiaShape()
                    {
                        AddMarginTop=-100,
                        ZIndex = 1,
                        Tag = "Red",
                        BackgroundColor = Colors.Red,
                        HeightRequest=100,
                        LockRatio=1,
                    },
                    new SkiaShape()
                    {
                        //AddMarginTop=-200,
                        Tag = "Blue",
                        BackgroundColor = Colors.Blue,
                        HeightRequest=100,
                        LockRatio=-1,
                    },
                }
            };

            var destination = new SKRect(0, 0, 150, float.PositiveInfinity);
            layout.Measure(destination.Width, destination.Height, 1);

            //prepare DrawingRect
            layout.Arrange(new SKRect(0, 0, layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height),
                layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height, 1);

            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx.WithDestination(layout.DrawingRect));
            });

            var image = layout.RenderObject.Image;

            Assert.Equal(layout.DrawingRect.Width, 100);
        }

        [Fact]
        public void ColumnTypePaddingOk()
        {
            var layout = new SkiaLayout
            {
                Padding = new Thickness(16),
                Type = LayoutType.Column,
                WidthRequest = 100,
                Spacing = 0,
                UseCache = SkiaCacheType.Image,
                Children = new List<SkiaControl>()
                {
                    new SkiaShape()
                    {
                        ZIndex = 0,
                        Tag = "Green",
                        BackgroundColor = Colors.Green,
                        HeightRequest=100,
                        LockRatio=1,
                    },
                    new SkiaShape()
                    {
                        AddMarginTop=-100,
                        ZIndex = 1,
                        Tag = "Red",
                        BackgroundColor = Colors.Red,
                        HeightRequest=100,
                        LockRatio=1,
                    },
                    new SkiaShape()
                    {
                        //AddMarginTop=-200,
                        Tag = "Blue",
                        BackgroundColor = Colors.Blue,
                        HeightRequest=100,
                        LockRatio=-1,
                    },
                }
            };

            var destination = new SKRect(0, 0, 150, float.PositiveInfinity);
            layout.Measure(destination.Width, destination.Height, 1);

            //prepare DrawingRect
            layout.Arrange(new SKRect(0, 0, layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height),
                layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height, 1);

            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx.WithDestination(layout.DrawingRect));
            });

            var image = layout.RenderObject.Image;

            Assert.Equal(layout.DrawingRect.Width, 100);
        }

        [Fact]
        public void ColumnTypeMarginOk()
        {
            var layout = new SkiaLayout
            {
                BackgroundColor = Colors.Black,
                Type = LayoutType.Column,
                Margin = new Thickness(0),
                VerticalOptions = LayoutOptions.Fill,
                Spacing = 0,
                UseCache = SkiaCacheType.Image,
                Children = new List<SkiaControl>()
                {
                    new SkiaLabel()
                    {
                        BackgroundColor = Colors.Red,
                        WidthRequest = 100,
                        Tag="Label",
                        Text="Tests",
                    },
                }
            };

            var label = layout.FindViewByTag("Label");

            var destination = new SKRect(0, 0, 150, 150);
            layout.Measure(destination.Width, destination.Height, 1);

            //prepare DrawingRect
            layout.Arrange(new SKRect(0, 0, layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height),
                layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height, 1);

            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx.WithDestination(layout.DrawingRect));
            });

            var image = layout.RenderObject.Image;

            Assert.Equal(layout.DrawingRect.Width, 100);

            Assert.True(label.DrawingRect.Height > 0);

        }

        /*
        [Fact]
        public void ChildNotDrawnWhenOutOfBounds()
        {
            var layout = CreateSampleLayoutWIthChildren();
            var image = layout.FindView<SkiaImage>("Image");
            var label = layout.FindView<SkiaLabel>("Label");

            image.AddMarginTop = 110;

            var destination = new SKRect(0, 0, 100, 100);
            layout.Measure(destination.Width, destination.Height, 1);
            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx, destination, 1);
            });

            Assert.True(label.WasDrawn);
            Assert.False(image.WasDrawn);
        }
        */

        /// <summary>
        /// Check the generated structure to correspond to itemssource
        /// </summary>
        /// <param name="itemsSource"></param>
        /// <param name="layout"></param>
        /// <returns></returns>
        static bool LayoutStructureCorrespondsToItemsSource(IList itemsSource, SkiaLayout layout)
        {
            if (!layout.UsesRenderingTree)
            {
                throw new Exception("Incompatible layout, not using rendering tree");
            }

            if (itemsSource == null || itemsSource.Count == 0)
            {
                return layout.LatestStackStructure == null || layout.LatestStackStructure.GetChildren().Count() == 0;
            }

            if (layout.LatestStackStructure.GetChildren().Count() != itemsSource.Count())
                return false;


            var index = 0;
            foreach (var cell in layout.LatestStackStructure.GetChildren())
            {
                if (cell.ControlIndex != index)
                    return false;

                var item = itemsSource[cell.ControlIndex];

                index++;
            }

            return itemsSource.Count == index;
        }

        static SkiaLayout CreateSampleLayoutWIthChildren()
        {
            return new SkiaLayout
            {
                Children = new List<SkiaControl>()
                {
                    new SkiaLabel()
                    {
                        Tag="Label",
                        Text="Tests"
                    },
                    new SkiaShape()
                    {
                        Tag="Shape",
                        ZIndex = 1,
                        WidthRequest = 20,
                        LockRatio = 1
                    },
                    new SkiaImage()
                    {
                        Tag="Image",
                        WidthRequest = 50,
                        LockRatio = 1
                    },
                    new SkiaLabelFps()
                }
            };
        }

        static SkiaLayout CreateAbsoluteLayoutSampleWIthChildren()
        {
            return new SkiaLayout
            {
                BackgroundColor = Colors.Black,
                UseCache = SkiaCacheType.Image,
                Children = new List<SkiaControl>()
                {
                    new SkiaLabel()
                    {
                        Tag="Label",
                        Text="Tests",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        ZIndex = 2,
                    },
                    new SkiaShape()
                    {
                        Tag="Shape",
                        ZIndex = 1,
                        BackgroundColor = Colors.Red,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    },
                    new SkiaShape()
                    {
                        BackgroundColor = Colors.Yellow,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    },
                }
            };
        }

    }
}
