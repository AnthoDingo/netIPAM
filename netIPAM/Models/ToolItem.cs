namespace netIPAM.Models
{
    internal sealed class ToolItem
    {
        public required string Name { get; set; }
        public string? Icon { get; set; }
        public string? Tooltip { get; set; }
        public required string Link { get; set; }
        public bool ShowInMenuBar { get; set; } = false;
    }
}
