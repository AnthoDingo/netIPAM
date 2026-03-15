using System.Text.Json;

namespace netIPAM.Models
{
    public enum SettingType
    {
        String,
        Integer,
        Boolean,
        Json
    }

    public class Setting
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
        public required SettingType Type { get; set; }

        public object GetTypedValue()
        {
            return Type switch
            {
                SettingType.Integer => int.Parse(Value),
                SettingType.Boolean => bool.Parse(Value),
                SettingType.Json => JsonDocument.Parse(Value),
                _ => Value
            };
        }
    }
}
