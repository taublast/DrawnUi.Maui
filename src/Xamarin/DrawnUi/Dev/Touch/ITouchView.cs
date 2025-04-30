using System.Windows.Input;

namespace AppoMobi.Framework.Forms.UI.Touch;

public interface ITouchView : ICanBeTapped, IDisposable
{

    public object CommandTappedParameter { get; set; }

    public ICommand CommandLongPressing { get; set; }

}
