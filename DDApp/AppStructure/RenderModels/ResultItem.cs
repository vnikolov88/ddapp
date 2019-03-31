﻿using System.Collections.Generic;

namespace DDApp.AppStructure.RenderModels
{
    public class ContactInfoIcon
    {
        public string Icon { get; set; }
        public string Image { get; set; }
    }

    public class ResultItem
    {   
        /// <summary>
        /// Non-mandatory information
        /// </summary>
        public string ItemName { get; set; }
        public string DatePublished { get; set; }
        public string AdditionalInfo { get; set; }

        /// <summary>
        /// Mandatory Information
        /// </summary>
        public string Description { get; set; }
        public string Location { get; set; }
        public string Image { set; get; }

        /// <summary>
        /// Non-mandatory information
        /// </summary>
        public string ContactInfo { get; set; }
        public ContactInfoIcon ContactInfoIcon { get; set; }

        /// <summary>
        /// The Link is almost always necessary  
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Style is mandatory in order to change the appearance of the items
        /// </summary>
        public ResultItemStyle? Style { get; set; }
    }
}
