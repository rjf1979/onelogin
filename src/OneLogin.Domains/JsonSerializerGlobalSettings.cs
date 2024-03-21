using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OneLogin.Core
{
    public class JsonSerializerGlobalSettings
    {
        public static JsonSerializerSettings Default { get; } = new()
        {
            Converters = new List<JsonConverter>
            {
                new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                }
            }
        };
    }
}
