using System;
using System.Timers;
using Newtonsoft.Json;
using Toggl.Shared;

namespace Toggl.Networking.Serialization.Converters
{
    [Preserve(AllMembers = true)]
    public sealed class TimerViewConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(TimerView);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            => TimerView.FromString(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var timerView = (TimerView)value;
            writer.WriteValue(timerView.Value);
        }
    }
}
