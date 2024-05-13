using System.Text.Json;

namespace UnitTests;

public class TestsBase
{


    public TestsBase()
    {
    }


    protected string Json(object value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

}