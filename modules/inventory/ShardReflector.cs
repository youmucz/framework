using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace framework.modules.inventory
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ShardHiddenAttribute : Attribute
    {
    }
}

namespace framework.modules.inventory
{
    public static class ShardReflector
    {
        private static readonly Dictionary<string, Type> NativelyDefinedShardTypesByName = new();

        static ShardReflector()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Shard))
                        && !type.IsAbstract // Ignore abstract classes since we don't want to register them
                        && type.GetCustomAttribute<ShardHiddenAttribute>() == null) // Ignore shards that requested to be hidden
                    {
                        NativelyDefinedShardTypesByName.Add(type.FullName!, type);

                        //Debug.Log($"Registered {typeof(Shard).Name}: {type.Name} with key {type.FullName}");
                    }
                }
            }
        }


        public static Type GetRegisteredShardType(string key)
        {
            if (NativelyDefinedShardTypesByName.TryGetValue(key, out var type))
            {
                return type;
            }

            GD.PrintErr($"Failed to get registered {nameof(Shard)} type from key {key}");
            return null;
        }

        public static List<string> GetNativelyDefinedShardTypes()
        {
            return NativelyDefinedShardTypesByName.Keys.ToList();
        }
    }
}
