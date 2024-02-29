using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OneLogin.Core
{
    public class JsonSerializerGlobalSettings
    {
        //public static JsonSerializerOptions Options { get; } = new()
        //{
        //    Converters =
        //    {
        //        new DateTimeJsonConverter()
        //    },
        //    PropertyNamingPolicy = null,
        //    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,//碰到html不会转义
        //    ReferenceHandler = ReferenceHandler.IgnoreCycles
        //};

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
