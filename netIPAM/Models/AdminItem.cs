namespace netIPAM.Models
{
    internal sealed class AdminItem
    {
        public required string Title { get; set; }
        public required string Icon { get; set; }
        public required List<AdminSubItem> Items { get; set; }
    }

    internal sealed class  AdminSubItem
    {
        public required string Title { get; set; }
        public required string Icon { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
    }
}
