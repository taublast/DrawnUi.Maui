namespace Sandbox
{
    public class CDigitCell : BindableObject
    {
        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged();
                }
            }
        }

        private char _Mask;
        public char Mask
        {
            get { return _Mask; }
            set
            {
                if (_Mask != value)
                {
                    _Mask = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _Index;
        public int Index
        {
            get { return _Index; }
            set
            {
                if (_Index != value)
                {
                    _Index = value;
                    OnPropertyChanged();
                }
            }
        }




        private string _Display;
        public string Display
        {
            get { return _Display; }
            set
            {
                if (_Display != value)
                {
                    _Display = value;
                    OnPropertyChanged();
                }
            }
        }



    }
}