using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDApp.AppStructure.RenderModels
{
    public class SocialBarItem
    {
        public string Id { get; set; }
        public string Class { get; set; }
        public string Href { get; set; }
        public string Text { get; set; }

        public string ImageSource { get; set; }
        public string Icon { get; set; }
    }
}
