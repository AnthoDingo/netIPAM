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
    }
}
