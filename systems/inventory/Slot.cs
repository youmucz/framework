using System;
using System.Collections;
using System.Collections.Generic;
using framework.systems.tag;

namespace framework.systems.inventory
{
    /// <summary> Represents a single slot within an inventory, and can hold an item </summary>
    [System.Serializable]
    public class Slot
    {
        public static Slot Invalid = new Slot();

        /// <summary> Tags that define this slot (Slot.Backpack, Slot.Equipment.Helmet, etc) </summary>
        public List<Tag> SlotTags = new();

        /// <summary> The tag query that determines what items are allowed to be placed in this slot </summary>
        public TagQuery ItemTagQuery = new TagQuery(TagQueryCondition.All, null);

        /// <summary> The item currently in this slot </summary>
        public Item Item { get; private set; } = null;


        /// <summary> Whether this slot is allowed to hold a given item according to rules defined by the slot. Does not 
        /// consider whether an item held in the slot at the time or not </summary>
        public bool AllowedToHoldItem(Item item)
        {
            if (ItemTagQuery != null)
            {
                return ItemTagQuery.Test(item.Tags);
            }

            return true;
        }

        public bool CanHoldItem(Item item)
        {
            if (Item != null)
            {
                return false;
            }

            if (!AllowedToHoldItem(item))
            {
                return false;
            }

            return true;
        }

        public void SetItem(Item item)
        {
            Item = item;
        }
    }
}
