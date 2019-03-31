using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDApp.AppStructure.RenderModels
{

    public class TitleLite
    {
        public enum TitleType
        {
            h1,
            h2,
            h3,
            h4,
            h5,
            h6
        }
        public TitleType Type { get; set; }
        public string Text { get; set; }
        public string Class { get; set; }
    }
}
