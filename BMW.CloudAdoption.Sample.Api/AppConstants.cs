using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BMW.CloudAdoption.Sample.Api;

public static class AppConstants
{
    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        Converters = { new StringEnumConverter() },
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
}