using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;

[ContentProperty(nameof(Value))]
//[ProvideCompiled("Microsoft.Maui.Controls.XamlC.SetterValueProvider")]
public class SkiaSetter : BindableObject
{
    readonly ConditionalWeakTable<BindableObject, object> _originalValues = new ConditionalWeakTable<BindableObject, object>();

    public string TargetName { get; set; }

    public BindableProperty Property { get; set; }

    public object Value { get; set; }

}
