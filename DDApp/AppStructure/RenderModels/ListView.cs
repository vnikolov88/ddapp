using System.Collections.Generic;

namespace DDApp.AppStructure.RenderModels
{
    public class ListView
    {
        public class Element
        {
            public string Title { get; set; }
        }
        public IEnumerable<Element> Elements { get; set; }
    }
}
