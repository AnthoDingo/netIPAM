namespace netIPAM.Components.Switch
{
    internal class SwitchOption
    {
        /// <summary>
        /// Set UI when Switch State is On
        /// </summary>
        public string Onlabel { get; set; } = "ON";

        /// <summary>
        /// Set UI when Switch State is Off
        /// </summary>
        public string Offlabel { get; set; } = "OFF";

        /// <summary>
        /// Set CSS class names when Switch State is On
        /// </summary>
        public string Onstyle { get; set; } = "default";

        /// <summary>
        /// Set CSS class names when Switch State is Off
        /// </summary>
        public string Offstyle { get; set; } = "default";

        /// <summary>
        /// (Optional) Set Switch size in CSS string representation
        /// </summary>
        public string Size { get; set; } = "mini";

        /// <summary>
        /// (Optional) Set Switch CSS style
        /// </summary>
        public string Style { get; set; } = null;

        /// <summary>
        /// (Optional) Set Switch width
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// (Optional) Set Switch height
        /// </summary>
        public int? Height { get; set; }
    }
}
