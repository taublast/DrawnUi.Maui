using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using SkiaSharp;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using Xunit;

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
            layout.Measure(100, 100, 1);

            Assert.True(LayoutStructureCorrespondsToItemsSource(itemsSource, layout));
        }

        [Fact]
        public async Task LayoutAndChildDisposedAndNotLeaked()
        {
            var layout = CreateSampleLayoutWIthChildren();
            var image = layout.FindView<SkiaImage>("Image");

            var layoutRef = new WeakReference(layout);
            var childRef = new WeakReference(image);

            var destination = new SKRect(0, 0, 100, 100);
            layout.Measure(destination.Width, destination.Height, 1);
            var picture = RenderWithOperationsContext(destination, (ctx) =>
            {
                layout.Render(ctx, destination, 1);
            });

            layout.Dispose();

            Assert.True(layout.IsDisposed);
            Assert.True(image.IsDisposing);

            await Task.Delay(1500);

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


        /*
        [Fact]
        public void ChildNotDrawnWhenOutOfBounds()
        {
            var layout = CreateSampleLayoutWIthChildren();
            var image = layout.FindView<SkiaImage>("Image");
            var label = layout.FindView<SkiaLabel>("Label");

            image.MarginTop = 110;

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

    }
}
