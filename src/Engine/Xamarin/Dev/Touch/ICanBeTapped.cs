using System.Windows.Input;

namespace AppoMobi.Framework.Forms.UI.Touch
{
    public interface ICanBeTapped
    {
        public ICommand CommandTapped { get; set; }
    }
}