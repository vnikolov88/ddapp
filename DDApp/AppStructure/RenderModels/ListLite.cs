using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDApp.AppStructure.RenderModels
{
    public class ListLite
    {
        public enum ListType
        {
            ul,
            ol            
        }

        public ListType Type { get; set; }
        public string[] Content { get; set; }        
        public string Class { get; set; }
    }
}
