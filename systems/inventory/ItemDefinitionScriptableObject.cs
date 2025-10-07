using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using framework.systems.tag;
using Godot;

namespace framework.systems.inventory
{
    [Tool, Serializable]
    public partial class ItemDefinitionScriptableObject : Resource
    {
        // WARNING!!! If these properties are renamed, serialized data within is lost. Most, if not all, data for an ItemDefinition is within shards
        // [SerializeField] doesn't allow children of the declared type, we use SerializeReference here

        /// <summary> Tags that only exist on the item definition </summary>
        private List<Tag> _tags = new();

        /// <summary> Static Shards contain data that never changes. Static shards cannot be added or removed at runtime, and don't network </summary>
        private List<Shard> _staticShards = new ();

        /// <summary> Dynamic Shards contain data that may change. Dynamic shards CAN be added and removed at runtime, and DO network </summary>
        private List<Shard> _dynamicShards = new ();

        public List<Tag> GetTags()
        {
            return _tags;
        }

        public Shard GetFirstStaticShard(TagQuery tagQuery = null)
        {
            return GetAllStaticShards(tagQuery, true).FirstOrDefault();
        }

        public T GetFirstStaticShard<T>(TagQuery tagQuery = null) where T : Shard
        {
            return GetAllStaticShards<T>(tagQuery, true).FirstOrDefault();
        }

        public List<Shard> GetAllStaticShards(TagQuery tagQuery = null, bool returnFirst = false)
        {
            return GetAllStaticShards<Shard>(tagQuery);
        }

        public List<T> GetAllStaticShards<T>(TagQuery tagQuery = null, bool returnFirst = false) where T : Shard
        {
            List<T> foundShards = new();
            foreach (Shard shard in _staticShards)
            {
                if (shard.GetType().IsAssignableFrom(typeof(T)))
                {
                    if (tagQuery?.Test(shard.Tags) ?? true) // If no query is supplied, pass every shard
                    {
                        foundShards.Add(shard as T);

                        if (returnFirst)
                        {
                            return foundShards;
                        }
                    }
                }
            }
            return foundShards;
        }

        public Shard GetFirstDynamicShard(TagQuery tagQuery = null)
        {
            return GetAllDynamicShards(tagQuery, true).FirstOrDefault();
        }

        public T GetFirstDynamicShard<T>(TagQuery tagQuery = null) where T : Shard
        {
            return GetAllDynamicShards<T>(tagQuery, true).FirstOrDefault();
        }

        public List<Shard> GetAllDynamicShards(TagQuery tagQuery = null, bool returnFirst = false)
        {
            return GetAllDynamicShards<Shard>(tagQuery);
        }

        public List<T> GetAllDynamicShards<T>(TagQuery tagQuery = null, bool returnFirst = false) where T : Shard
        {
            List<T> foundShards = new();
            foreach (Shard shard in _dynamicShards)
            {
                if (shard.GetType().IsAssignableFrom(typeof(T)))
                {
                    if (tagQuery?.Test(shard.Tags) ?? true) // If no query is supplied, pass every shard
                    {
                        foundShards.Add(shard as T);

                        if (returnFirst)
                        {
                            return foundShards;
                        }
                    }
                }
            }
            return foundShards;
        }

#if TOOLS
        public void AddStaticShard(Shard _shard)
        {
            // staticShards.Add(_shard);
        }

        public void AddDynamicShard(Shard _shard)
        {
            // dynamicShards.Add(_shard);
        }
#endif
    }
}
