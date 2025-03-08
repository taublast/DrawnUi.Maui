using System.Linq;

namespace DrawnUi.Maui.Extensions;

public static class StaticResourcesExtensions
{

    public static T GetSetterValue<T>(
        this Style style,
        BindableProperty property)
    {
        var styleSetters = style.Setters;
        Setter setter = styleSetters.FirstOrDefault<Setter>((Func<Setter, bool>)(p => p.Property == property));
        return setter != null ? (T)setter.Value : default(T);
    }

    public static T Get<T>(this ResourceDictionary resources, string name, VisualElement view = null)
    {
        try
        {
            if (!resources.TryGetValue(name, out object retVal))
            {
                if (view != null)
                {
                    //try get from attached to view resources
                    foreach (var resourcesProvider in GetAllWithMyselfParents(view).ToArray())
                    {
                        if (resourcesProvider.Resources != null)
                        {
                            if (resourcesProvider.Resources.TryGetValue(name, out retVal))
                            {
                                return (T)retVal;
                            }
                        }
                    }
                }
                throw new Exception($"[StaticResources] {typeof(T).Name} not found \"{name}\"");
            }

            return (T)retVal;
        }
        catch (Exception e)
        {
            Trace.WriteLine(e);
            return default;
        }
    }



    public static List<VisualElement> GetAllWithMyselfParents(this VisualElement view)
    {
        var ret = new List<VisualElement>
        {
            view
        };

        var parent = view.Parent as VisualElement;
        var add = parent;
        while (add != null)
        {
            ret.Add(add);
            add = add.Parent as VisualElement;
        }

        return ret;
    }





}