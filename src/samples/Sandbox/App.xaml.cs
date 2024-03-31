using DrawnUi.Maui;
using Microsoft.Maui.Platform;
using Sandbox.Views;
using System.Globalization;
using System.Reflection;

namespace Sandbox
{
    public partial class App : Application
    {
        public App()
        {
            Super.SetLocale("en");

            InitializeComponent();

            MainPage = new AppShell();

            var mask = "MainPage";

            var xamlResources = this.GetType().Assembly.GetCustomAttributes<XamlResourceIdAttribute>();

            MainPages = xamlResources
                .Where(x => x.Type.Name.Contains(mask) && x.Type.Name != mask)
                .Select(s => new MainPageVariant()
                {
                    Name = s.Type.Name.Replace(mask, string.Empty),
                    Type = s.Type
                }).ToList();
        }

        public static List<MainPageVariant> MainPages { get; protected set; }

    }

    public record MainPageVariant()
    {
        public Type Type { get; set; }
        public string Name { get; set; }
    }

    public class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
