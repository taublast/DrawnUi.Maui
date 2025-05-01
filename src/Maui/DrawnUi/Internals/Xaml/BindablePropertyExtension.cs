namespace DrawnUi.Draw;

public class BindablePropertyExtension : IMarkupExtension<BindableProperty>
{
    public string Name { get; set; }
    public Type Type { get; set; }

    public BindableProperty ProvideValue(IServiceProvider serviceProvider)
    {
        try
        {
            if (Type != null)
            {
                var property = Type.GetField(Name + "Property");
                var value = property.GetValue(null);
                return value as BindableProperty;
            }
        }
        catch (Exception e)
        {
        }
        return null;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return (this as IMarkupExtension<BindableProperty>).ProvideValue(serviceProvider);
    }
}