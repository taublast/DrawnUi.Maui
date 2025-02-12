using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppoMobi.Models;
using AppoMobi.Touch;
using DrawnUi.Maui.Draw;
using TestCalendar.Views;

namespace TestCalendar.Drawn
{
	
	public partial class DrawnDay : SkiaShape
	{
		SkiaLabel txtText;

		public DrawnDay()
		{
			StrokeWidth = 1;
			BackgroundColor = TransparentColor;

			Content = new SkiaLabel()
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Fill,
				HorizontalTextAlignment = DrawTextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				Margin = new(0.25, 0, 0.25, 0.5),
				Padding = new(0, 4, 0, 4),
				TextColor = Colors.DimGray,
				FontSize = 15
			}.With((c) =>
			{
				txtText = c;
			});

			//InitGesturesForCell(SelectionBox);

			_front = txtText.TextColor;

			SetupCell();
		}

		protected override void OnBindingContextChanged()

		{
			SetupCell();
			base.OnBindingContextChanged();
		}

		Color _back { get; set; } = MonthView.ColorBackground;
		Color _front { get; set; }
		
		public override void OnDisposing()
		{
			base.OnDisposing();

			if ((BindingContext is AppoDay item))
			{
				try { item.PropertyChanged -= OnItemPropertyChanged; }
				catch (Exception e) { }
			}
		}

		

		public event EventHandler<TapEventArgs> ItemTapped;
		public async void CallItemTapped(object item, TapEventArgs e)
		{
			//this.Animate(AnimationType.Scale);
			//await Task.Delay(250);
			ItemTapped?.Invoke(item, e);
		}

		void OnTapped_Cell(object sender, TapEventArgs e)
		{
			CallItemTapped(this, e); //сагат
		}

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Selected")
			{
				StyleCell(BindingContext as AppoDay);
			}
		}

		public void SetupCell()
		{
			if (BindingContext is not AppoDay item)
				return;

			try { item.PropertyChanged -= OnItemPropertyChanged; }
			catch (Exception e) { }

			item.PropertyChanged += OnItemPropertyChanged;
			
			var ee = new PropertyChangedEventArgs("Selected");
			OnItemPropertyChanged(item, ee);


			StyleCell(item);
		}

		void StyleCell(AppoDay item)
		{
			if (item == null)
			{
				return;
			}

			if (item.Disabled)
			{
				_back = Colors.WhiteSmoke;
				this.BackgroundColor = _back;
				_front = Colors.DarkGray;
				txtText.FontAttributes = FontAttributes.None;
			}
			else
			{
				_front = Colors.DimGray;
				txtText.FontAttributes = FontAttributes.Bold;
			}

			txtText.TextColor = _front;
			txtText.Text = item.DayDesc;

			var dateAndTime = DateTime.Now;
			var today = dateAndTime.Date;

			if (item.Date == today)
			{
				StrokeColor = Colors.Red;
			}
			else
			{
				StrokeColor = Colors.Transparent;
			}

			SetupSelection(item.Selected);
		}

		void SetupSelection(bool value)
		{
			if (value)
			{
				this.BackgroundColor = Colors.Red;
				txtText.TextColor = MonthView.ColorBackground;
			}
			else
			{
				this.BackgroundColor = Colors.Transparent;
				txtText.TextColor = _front;
			}
		}
	}
}
