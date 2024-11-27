using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QGo.Functions
{
    public class ModifierKeysListConverter : JsonConverter<List<ModifierKeys>>
    {
        public override void WriteJson(JsonWriter writer, List<ModifierKeys> value, JsonSerializer serializer)
        {
            JArray array = new JArray(value.Select(v => v.ToString()));
            array.WriteTo(writer);
        }

        public override List<ModifierKeys> ReadJson(JsonReader reader, Type objectType, List<ModifierKeys> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return array.Select(token => (ModifierKeys)Enum.Parse(typeof(ModifierKeys), token.ToString())).ToList();
        }
    }
}
