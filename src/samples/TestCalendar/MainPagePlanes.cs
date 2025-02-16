using AppoMobi.Xam;
using DrawnUi.Maui.Draw;
using TestCalendar.Drawn;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{
    public class MainPagePlanes : BaseCodePage
    {
        Canvas Canvas;

        SkiaLabel DebugLabel;

		public static DebugImage DebugLayerA;
		public static DebugImage DebugLayerB;
		public static DebugImage DebugLayerC;

		protected override void Dispose(bool isDisposing)
        {
	        if (isDisposing)
	        {
		        this.Content = null;
		        Canvas?.Dispose();
			}

			base.Dispose(isDisposing);
        }
		
        public override void Build()
        {
            Canvas?.Dispose();

            Canvas = new Canvas()
			{
				Gestures = GesturesMode.Lock,
				HardwareAcceleration = HardwareAccelerationMode.Enabled,
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Colors.Gray,

				Content = new SkiaLayout() // absolute wrapper
				{
					HorizontalOptions = LayoutOptions.Fill,
					VerticalOptions = LayoutOptions.Fill
				}.WithChildren(

					new SkiaLayout()
					{
						Type = LayoutType.Column,
						Spacing = 8,
						BackgroundColor = Colors.HotPink,
						HorizontalOptions = LayoutOptions.Fill,
						
						//VSTACK CONTENT
						Children = new List<SkiaControl>()
						{
							new SkiaLabel()
							{
								IsVisible = true,
								Tag="Show scroll offset",
								UseCache = SkiaCacheType.Operations,
								HeightRequest = 32,
								TextColor = Colors.Yellow,
								HorizontalOptions = LayoutOptions.Center,
								Text = $"Drawn Month Control",
							}.Adjust((c) =>
							{
								DebugLabel = c;
							}),
							new MonthsScroll()
							{
								IsVisible = false,
								BackgroundColor = Colors.DarkSlateGray,
								HorizontalOptions = LayoutOptions.Fill,
								HeightRequest = 260,
							}.Adjust((c) =>
							{
								c.Scrolled += (sender, point) =>
								{
									DebugLabel.Text = $"Scroll Y-offset: {c.InternalViewportOffset.Pixels.Y:0}";
								};
							}),
						 
							new SmartScroll()
							{       
								IsVisible = true,
								HorizontalOptions = LayoutOptions.Fill,
								VerticalOptions = LayoutOptions.Fill,
							}.WithContent(new SkiaLayout()
							{
								Tag="Recycled",
								Virtualisation = VirtualisationType.Managed,
								HorizontalOptions = LayoutOptions.Fill,
			 
								Type = LayoutType.Column,
								Spacing=4,
								ItemsSource = Enumerable.Range(1, 2000).ToList(),
								MeasureItemsStrategy = MeasuringStrategy.MeasureFirst,
								ItemTemplate = new DataTemplate(() =>
								{
									var label = new SkiaLabel()
									{
										Tag="CellLabel",
										Padding = new(8,4),
										HorizontalOptions = LayoutOptions.Fill,
										HorizontalTextAlignment = DrawTextAlignment.Center,
										BackgroundColor = Colors.DarkBlue,
										TextColor = Colors.White,
										FontSize = 32
									};
									label.SetBinding(SkiaLabel.TextProperty, new Binding("."));
									return label;
								})
							})
						 
						}
					},
					new SkiaLabel()
					{
						UseCache = SkiaCacheType.Operations,
						ZIndex = 100,
						Tag = "Reloads",
						TextColor = Colors.White,
						BackgroundColor = Colors.Black,
						Padding = new(6),
						HorizontalOptions = LayoutOptions.Start,
						VerticalOptions = LayoutOptions.End,
						Text = $"Reloads: {this.CountReloads}",
					},
					new SkiaLayout()
					{
						Type = LayoutType.Column
					}.WithChildren(new DebugImage()
						{
							Text = "A",
							StrokeWidth = 1,
							StrokeColor = Colors.DimGray,
							ZIndex = 100,
							WidthRequest = 90,
							HeightRequest = 90,
							VerticalOptions = LayoutOptions.Start,
							HorizontalOptions = LayoutOptions.Start
						}.Adjust((c) =>
						{
							MainPage.DebugLayerA = c;
						}),
						new DebugImage()
						{
							Text = "B",
							ZIndex = 100,
							StrokeWidth = 1,
							StrokeColor = Colors.DimGray,
							WidthRequest = 90,
							HeightRequest = 90,
							VerticalOptions = LayoutOptions.Start,
							HorizontalOptions = LayoutOptions.End
						}.Adjust((c) =>
						{
							MainPage.DebugLayerB = c;
						}),
						new DebugImage()
						{
							Text = "C",
							ZIndex = 100,
							StrokeWidth = 1,
							StrokeColor = Colors.DimGray,
							WidthRequest = 90,
							HeightRequest = 90,
							VerticalOptions = LayoutOptions.Start,
							HorizontalOptions = LayoutOptions.End
						}.Adjust((c) =>
						{
							MainPage.DebugLayerC = c;
						})
					)
				)
			};

            this.Content = Canvas;
        }

    }
}
