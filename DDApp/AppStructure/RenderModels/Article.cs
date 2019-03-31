namespace DDApp.AppStructure.RenderModels
{
    public class Article
    {
        public enum ImageStyle{
            HasBorder,
            Profile
        }
        //Sets background-color, border, padding and so on
        //CSS: HasBorder, NoBorder ?
        public ImageStyle? Style { get; set; } = null;

        public string Title { set; get; }

        public string[] Sources { set; get; }
        //CSS class how image to be displayed (centered or on full screen width)
        //CSS Example: Centered and FullWidth
        public string Description { set; get; }

        public string DatePublished { get; set; }
    }
}
