using DrawnUi.Maui.Draw;
using Microsoft.Extensions.Logging;

namespace ShadersCarouselDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "FontText");
                    fonts.AddFont("OpenSans-Semibold.ttf", "FontTextBold");
                });


            builder.UseDrawnUi(new()
            {
                DesktopWindow = new()
                {
                    Width = 400,
                    Height = 700,
                    //IsFixedSize = true //user cannot resize window
                }
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }

    public class SimpleItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Banner { get; set; }
    }

    public class MockDataProvider
    {
        public void ResetIndex()
        {
            index = 0;
        }

        public void ResetIndexSmall()
        {
            indexSmall = 0;
        }

        int seed = 0;

        int seedSmall = 0;

        public List<SimpleItemViewModel> GetRandomItems(int count)
        {
            var items = new List<SimpleItemViewModel>();

            for (int i = 0; i < count; i++)
            {
                index++;

                items.Add(new SimpleItemViewModel
                {
                    Id = (int)index,
                    Title = GetRandomName(),
                    Description = GetRandomDescription(),
                    Banner = GetRandomImage(),
                });
            }

            return items;
        }

        public List<SimpleItemViewModel> GetRandomSmallItems(int count)
        {
            var items = new List<SimpleItemViewModel>();

            for (int i = 0; i < count; i++)
            {
                indexSmall++;

                items.Add(new SimpleItemViewModel
                {
                    Id = (int)indexSmall,
                    Title = GetRandomName(),
                    Description = GetRandomDescription(),
                    Banner = GetRandomSmallImage(),
                });
            }

            return items;
        }

        public string GetRandomDescription()
        {
            return descriptions[random.Next(descriptions.Length)];
        }

        public string GetRandomName()
        {
            return NameGenerator.GenerateRandomName();
        }

        public string GetRandomImage()
        {
            seed++;
            return $"https://picsum.photos/600?random={seed}";
            //return $"https://picsum.photos/seed/{seed}/600/600";
        }

        public string GetRandomSmallImage()
        {
            seedSmall++;
            return $"https://picsum.photos/seed/{seedSmall}/300/300";
        }

        public string GetRandomAvatar()
        {
            return $"https://picsum.photos/seed/{seed}/200/200";
        }

        private static Random random = new Random();

        long index;
        long indexSmall;


        private static string[] descriptions = new string[]
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            "Vestibulum id ligula porta felis euismod semper.",
            "Donec id elit non mi porta gravida at eget metus.",
            "Nullam quis risus eget urna mollis ornare vel eu leo.",
            "Donec ullamcorper nulla non metus auctor fringilla."
        };

        private static string[] images = new string[]
        {
            "https://4.img-dpreview.com/files/p/TS1200x900~sample_galleries/7566437716/8187357568.jpg",
            "https://2.img-dpreview.com/files/p/TS1200x900~sample_galleries/7566437716/3825995157.jpg",
            "https://1.img-dpreview.com/files/p/TS1200x900~sample_galleries/7566437716/9281906178.jpg",
            "https://2.img-dpreview.com/files/p/TS1200x900~sample_galleries/7566437716/1548752695.jpg",
            "https://4.img-dpreview.com/files/p/TS1200x900~sample_galleries/7566437716/2659915629.jpg"
        };

        // 

        /// <summary>
        /// use NameGenerator.GenerateRandomName()
        /// </summary>
        public class NameGenerator
        {


            private static string[] firstNames = new string[]
            {
                "John", "Jane", "Peter", "Paul", "James", "Jill", "Soth", "Sara", "Robert", "Rachel"
            };

            private static string[] lastNames = new string[]
            {
                "Smith", "Bronson", "Williams", "Brown", "Jones", "Mall", "Davis", "Garcia", "Rodriguez", "Wilson"
            };

            public static string GenerateRandomName()
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];

                return firstName + " " + lastName;
            }
        }



    }
}
