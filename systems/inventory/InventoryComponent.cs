using System.Collections.Generic;
using framework.systems.tag;
using Godot;

namespace framework.systems.inventory
{
    /// <summary> Represents a collection of bags that can be held on a GameObject </summary>
    public partial class InventoryComponent : Node
    {
        private readonly List<Slot> _slots = new();

        public void AddSlot(Slot slot)
        {
            if (!_slots.Contains(slot))
            {
                _slots.Add(slot);
            }
        }

        public void AddSlots(List<Slot> slots)
        {
            foreach (Slot slot in slots)
            {
                AddSlot(slot);
            }
        }

        public void RemoveSlot(Slot slot)
        {
            if (_slots.Contains(slot))
            {
                _slots.Remove(slot);
            }
        }

        public void RemoveSlots(List<Slot> slots)
        {
            foreach (Slot slot in slots)
            {
                RemoveSlot(slot);
            }
        }

        public bool LootItem(Item item)
        {
            foreach (Slot slot in _slots)
            {
                if (slot.CanHoldItem(item))
                {
                    slot.SetItem(item);
                    OnItemAdded(item);

                    return true;
                }
            }

            return false;
        }

        public bool RemoveItem(Item item)
        {
            foreach (Slot slot in _slots)
            {
                if (slot.Item == item)
                {
                    slot.SetItem(null);
                    OnItemRemoved(item);

                    return true;
                }
            }

            return false;
        }

        public List<Slot> GetSlots(TagQuery slotTagQuery = null)
        {
            return GetSlots<Slot>(slotTagQuery);
        }

        public List<T> GetSlots<T>(TagQuery slotTagQuery = null) where T : Slot
        {
            List<T> foundSlots = new();
            foreach (Slot slot in IterateSlots())
            {
                if (slotTagQuery != null
                    && !slotTagQuery.Test(slot.SlotTags))
                {
                    continue;
                }

                if (slot.Item != null
                    && slot.Item is T)
                {
                    foundSlots.Add(slot as T);
                }
            }

            return foundSlots;
        }

        public List<Item> GetItems(TagQuery slotTagQuery = null, TagQuery itemTagQuery = null)
        {
            return GetItems<Item>(slotTagQuery, itemTagQuery);
        }

        public List<T> GetItems<T>(TagQuery slotTagQuery = null, TagQuery itemTagQuery = null) where T : Item
        {
            List<T> foundItems = new();
            foreach (var slot in IterateSlots())
            {
                if (slotTagQuery != null && !slotTagQuery.Test(slot.SlotTags))
                {
                    continue;
                }

                if (slot.Item != null && slot.Item is T)
                {
                    if (itemTagQuery != null
                        && !itemTagQuery.Test(slot.Item.Tags))
                    {
                        continue;
                    }

                    foundItems.Add(slot.Item as T);
                }
            }

            return foundItems;
        }

        public IEnumerable<Slot> IterateSlots()
        {
            return _slots;
        }

        protected virtual void OnItemAdded(Item item)
        {

        }

        protected virtual void OnItemRemoved(Item item)
        {

        }
    }
}
