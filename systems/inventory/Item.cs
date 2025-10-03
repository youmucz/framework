using System.Linq;
using System.Collections.Generic;
using framework.systems.tag;

namespace framework.systems.inventory
{
    /// <summary> Represents a single object that can be held within a bag </summary>
    public class Item
    {
        public readonly List<Tag> Tags;

        protected readonly List<Shard> DynamicShards = new();
        protected ItemDefinitionScriptableObject ItemDefinition { get; private set; }


        public Item(ItemDefinitionScriptableObject itemDefinition, List<Shard> additionalDynamicShards = null)
        {
            ItemDefinition = itemDefinition;

            Tags = itemDefinition.GetTags();

            DynamicShards.AddRange(ItemDefinition.GetAllDynamicShards());
            if (additionalDynamicShards != null)
            {
                DynamicShards.AddRange(additionalDynamicShards);
            }
        }

        public Shard GetFirstShard(TagQuery tagQuery = null)
        {
            return GetAllShards(tagQuery, true).FirstOrDefault();
        }

        public T GetFirstShard<T>(TagQuery tagQuery = null) where T : Shard
        {
            return GetAllShards<T>(tagQuery, true).FirstOrDefault();
        }

        public List<Shard> GetAllShards(TagQuery tagQuery = null, bool returnFirst = false)
        {
            return GetAllShards<Shard>(tagQuery);
        }

        public List<T> GetAllShards<T>(TagQuery tagQuery = null, bool returnFirst = false) where T : Shard
        {
            List<T> foundShards = new();
            foreach (Shard dynamicShard in DynamicShards)
            {
                if (dynamicShard.GetType().IsAssignableFrom(typeof(T)))
                {
                    if (tagQuery?.Test(dynamicShard.Tags) ?? true) // If no query is supplied, pass every shard
                    {
                        foundShards.Add(dynamicShard as T);

                        if (returnFirst)
                        {
                            return foundShards;
                        }
                    }
                }
            }

            // Search for static shards on the item definition directly
            foundShards.AddRange(ItemDefinition.GetAllStaticShards<T>(tagQuery, returnFirst));

            return foundShards;
        }
    }
}
