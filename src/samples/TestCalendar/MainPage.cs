using DrawnUi.Maui.Draw;
using TestCalendar.Drawn;
using Canvas = DrawnUi.Maui.Views.Canvas;

namespace Sandbox
{
    public class MainPage : BaseCodePage
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
						Spacing = 9,
						BackgroundColor = Colors.DarkGrey,
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
								VerticalTextAlignment = TextAlignment.Center,
								Text = $"Drawn Month Control",
							}.Adjust((c) =>
							{
								DebugLabel = c;
							}),
							new BlueCalendar()
							{
								MaximumWidthRequest = 300,
								HorizontalOptions = LayoutOptions.Center,
								VerticalOptions = LayoutOptions.Fill
							}.Adjust((c) =>
							{
								c.SelectionDatesChanged += (s, e) =>
								{
									System.Diagnostics.Debug.WriteLine($"Sel: {e}");
								};
							})
						}
					},
					new SkiaLabel()
					{
						UseCache = SkiaCacheType.Operations,
						ZIndex = 991,
						Tag = "Reloads",
						TextColor = Colors.White,
						BackgroundColor = Colors.Black,
						Padding = new(6),
						HorizontalOptions = LayoutOptions.Start,
						VerticalOptions = LayoutOptions.End,
						Text = $"Reloads: {this.CountReloads}",
					}
					
				)
			};

            this.Content = Canvas;
        }

    }
}
