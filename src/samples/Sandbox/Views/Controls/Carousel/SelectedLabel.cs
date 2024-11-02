namespace Sandbox.Views.Controls.Carousel
{
    public class CustomIndicators : CarouselIndicators
    {
        public override void UpdateSource()
        {
            var values = new List<SelectedLabel>();
            for (int i = 0; i < TotalEnabled; i++)
            {
                var text = $"{i + 1}";
                values.Add(new()
                {
                    Text = text,
                    IsSelected = i == SelectedIndex
                });
            }

            Values = values;
            ItemsSource = Values;
        }


    }

    public class SelectedLabel : BindableObject
    {
        private bool _isSelected;
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
