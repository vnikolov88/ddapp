using System.Collections.Generic;

namespace DDApp.AppStructure.RenderModels
{

    public enum ItemStyle
    {
        Hidden,
        LeftSideImg,
        RightSideImg,
        OneRowItem,
        OneRowItemLarge,
        OneRowItemIconColorInverted,
        TwoRowsItem,
        TwoRowsItemShortText,
        ThreeRowsItem
    }

    public class ItemList
    {
        public ItemStyle Style { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public string Subject { set; get; }
    }

}


