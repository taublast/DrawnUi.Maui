namespace DrawnUi.Draw;

/// <summary>
/// Compiled-bindings-friendly implementation for "Source.Parent.BindingContext.Path"
/// </summary>
[ContentProperty(nameof(Path))]
public class BindToParentContextExtension : IMarkupExtension<BindingBase>
{
    public string Path { get; set; }

    public BindableObject Source { get; set; }

    public BindingMode Mode { get; set; } = BindingMode.Default;

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Path))
        {
            throw new InvalidOperationException("Path must be set for BindToParentContextExtension.");
        }

        BindableObject bindable = null;
        object actualSource = null;

        if (Source == null)
        {
            var valueProvider = serviceProvider.GetService<IProvideValueTarget>();
            bindable = valueProvider?.TargetObject as BindableObject;
            if (bindable == null)
            {
                throw new InvalidOperationException("Cannot resolve target bindable object.");
            }
        }
        else
        {
            bindable = Source;
        }

        object ctx;
        if (bindable is SkiaControl control)
        {
            actualSource = control.Parent?.BindingContext;
        }
        else
        if (bindable is Element element)
        {
            actualSource = element.Parent?.BindingContext;
        }

        // Create a binding and set its source as bindable.Parent.BindingContext or the Source property
        var binding = new Binding
        {
            Path = "BindingContext." + Path,
            Source = actualSource,
            Mode = Mode
        };

        return binding;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}