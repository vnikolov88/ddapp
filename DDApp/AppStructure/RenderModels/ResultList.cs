using System.Collections.Generic;

namespace DDApp.AppStructure.RenderModels
{

    public enum ResultItemStyle
    {
        SingleRowResult,
        ThreeRowsResult,
        TwoRowsResultWithItemList
    }

    public class ResultList
    {
        /// <summary>
        /// Changes the appearance of the whole List of result items
        /// </summary>
        public ResultItemStyle Style { get; set; }

        /// <summary>
        /// Contains all the result items
        /// </summary>
        public IEnumerable<ResultItem> Items { get; set; } 
    }

}
