using Newtonsoft.Json;

namespace Monopoly
{
    public class PropertyGroupFactory
    {
        public static PropertyGroup[] GenerateGroups(string allData)
        {
            return JsonConvert.DeserializeObject<PropertyGroup[]>(allData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}
