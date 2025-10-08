using System.Linq;
using System.Collections.Generic;
using framework.core.interfaces;
using framework.modules.tag;
using Godot;

namespace framework.modules.inventory
{
    /// <summary> Represents a single object that can be held within a bag </summary>
    public partial class Item : Resource, IUnit
    {
        public List<Tag> Tags = new ();
        public string ItemId { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string Description { get; set; } = "";
        public Texture2D Icon { get; set; }
        public int MaxStackSize { get; set; } = 99;
        public int Value { get; set; } = 0;
    
        public virtual Item Clone()
        {
            return (Item)Duplicate();
        }

        // protected readonly List<Shard> DynamicShards = new();
        // protected ItemDefinition ItemDefinition { get; private set; }
        //
        // public Item(ItemDefinition itemDefinition, List<Shard> additionalDynamicShards = null)
        // {
        //     ItemDefinition = itemDefinition;
        //
        //     Tags = itemDefinition.GetTags();
        //
        //     DynamicShards.AddRange(ItemDefinition.GetAllDynamicShards());
        //     if (additionalDynamicShards != null)
        //     {
        //         DynamicShards.AddRange(additionalDynamicShards);
        //     }
        // }
        //
        // public Shard GetFirstShard(TagQuery tagQuery = null)
        // {
        //     return GetAllShards(tagQuery, true).FirstOrDefault();
        // }
        //
        // public T GetFirstShard<T>(TagQuery tagQuery = null) where T : Shard
        // {
        //     return GetAllShards<T>(tagQuery, true).FirstOrDefault();
        // }
        //
        // public List<Shard> GetAllShards(TagQuery tagQuery = null, bool returnFirst = false)
        // {
        //     return GetAllShards<Shard>(tagQuery);
        // }
        //
        // public List<T> GetAllShards<T>(TagQuery tagQuery = null, bool returnFirst = false) where T : Shard
        // {
        //     List<T> foundShards = new();
        //     foreach (Shard dynamicShard in DynamicShards)
        //     {
        //         if (dynamicShard.GetType().IsAssignableFrom(typeof(T)))
        //         {
        //             if (tagQuery?.Test(dynamicShard.Tags) ?? true) // If no query is supplied, pass every shard
        //             {
        //                 foundShards.Add(dynamicShard as T);
        //
        //                 if (returnFirst)
        //                 {
        //                     return foundShards;
        //                 }
        //             }
        //         }
        //     }
        //
        //     // Search for static shards on the item definition directly
        //     foundShards.AddRange(ItemDefinition.GetAllStaticShards<T>(tagQuery, returnFirst));
        //
        //     return foundShards;
        // }

        public void OnSpawn()
        {
            
        }

        public void OnDespawn()
        {
            
        }

        public void ResetState()
        {
            
        }
    }

    public enum ItemType
    {
        Equipment,  // 装备类物品
        Weapon, // 武器
        Armor, // 护甲/防具
        Tool, // 工具
        Consumable, // 消耗品
        Potion, // 药水
        Collectible, // 收集品
        Pickup, // 可拾取物品
        Loot, // 战利品/掉落物
        Treasure, // 宝物
        Artifact, // 神器/文物
        Relic, // 遗物
    }
}
