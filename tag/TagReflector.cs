using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Minikit
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NativeTagAttribute : Attribute
    {
    }
}

namespace Minikit.Internal
{
    public static class TagReflector
    {
        private static List<string> _cachedNativelyDefinedTags = new();

        public static List<string> GetNativelyDefinedTags()
        {
            if (_cachedNativelyDefinedTags.Count > 0)
            {
                return _cachedNativelyDefinedTags;
            }

            List<string> nativelyDefinedTags = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsStatic())
                    {
                        foreach (var fieldInfo in type.GetFields())
                        {
                            if (fieldInfo.FieldType == typeof(string) && fieldInfo.GetCustomAttribute<NativeTagAttribute>() != null)
                            {
                                nativelyDefinedTags.Add((string)fieldInfo.GetValue(null));
                            }
                        }
                    }
                }
            }
            _cachedNativelyDefinedTags = nativelyDefinedTags;

            return _cachedNativelyDefinedTags;
        }
    }
}
