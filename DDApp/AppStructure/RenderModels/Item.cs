using System.Collections.Generic;

namespace DDApp.AppStructure.RenderModels
{
    public class Item
    {
        //Always displayed text 
        public string Title { get; set; }
        //Additional text under the title
        public string Description { get; set; }
        public string DatePublished { get; set; }
        //Reference to other page
        public string Link { get; set; }
        public string Icon { get; set; }
        public string Image { set; get; }

        public DescriptionList DescriptionList { get; set; }

        public ItemStyle? Style { get; set; }
    }
}
