using System.Collections.Generic;

namespace DDApp.AppStructure.RenderModels
{
    public class DetailedListView
    {
        public class Element
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public IEnumerable<Element> Elements { get; set; }
    }
}
