namespace DrawnUi.Maui.Draw
{
    public static class FluentExtensions
    {
       

        public static T With<T>(this T view, Action<T> action) where T : SkiaControl
        {
            action?.Invoke(view);
            return view;
        }

        public static T WithChildren<T>(this T view, params SkiaControl[] children) where T : SkiaLayout
        {
            foreach (SkiaControl child in children)
            {
                view.AddSubView(child);
            }
            return view;
        }

        public static T WithContent<T>(this T view, SkiaControl child) where T : IWithContent
        {
            view.Content = child;
            return view;
        }

        public static T WithParent<T>(this T view, IDrawnBase parent) where T : SkiaControl
        {
            parent.AddSubView(view);
            return view;
        }

        public static T CenterX<T>(this T view) where T : SkiaControl
        {
            view.HorizontalOptions = LayoutOptions.Center;
            return view;
        }

        public static T CenterY<T>(this T view) where T : SkiaControl
        {
            view.VerticalOptions = LayoutOptions.Center;
            return view;
        }


        #region GRID

        public static T WithRow<T>(this T view, int row) where T : SkiaControl
        {
            Grid.SetRow(view, row);
            return view;
        }

        public static T WithColumn<T>(this T view, int column) where T : SkiaControl
        {
            Grid.SetColumn(view, column);
            return view;
        }

        public static T WithRowSpan<T>(this T view, int rowSpan) where T : SkiaControl
        {
            Grid.SetRowSpan(view, rowSpan);
            return view;
        }

        public static T WithColumnSpan<T>(this T view, int columnSpan) where T : SkiaControl
        {
            Grid.SetColumnSpan(view, columnSpan);
            return view;
        }

        public static SkiaLayout WithColumnDefinitions(this SkiaLayout grid, string columnDefinitions)
        {
            var converter = new ColumnDefinitionCollectionTypeConverter();

            if (converter.CanConvertFrom(typeof(string)))
            {
                var columns = (ColumnDefinitionCollection)converter.ConvertFromInvariantString(columnDefinitions);
                grid.ColumnDefinitions = columns;
            }
            else
            {
                throw new InvalidOperationException("ColumnDefinitionCollectionTypeConverter cannot convert from string.");
            }
            return grid;
        }
        public static SkiaLayout WithRowDefinitions(this SkiaLayout grid, string definitions)
        {
            var converter = new ColumnDefinitionCollectionTypeConverter();

            if (converter.CanConvertFrom(typeof(string)))
            {
                var defs = (RowDefinitionCollection)converter.ConvertFromInvariantString(definitions);
                grid.RowDefinitions = defs;
            }
            else
            {
                throw new InvalidOperationException("RowDefinitionCollectionTypeConverter cannot convert from string.");
            }
            return grid;
        }

        #endregion
    }
}
