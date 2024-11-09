using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Minikit
{
    [Serializable]
    public class Tag : IEquatable<Tag>
    {
        /// <summary> WARNING! This is not intended to be changed during runtime. It is marked public for Unity serialization purposes </summary>
        public readonly string Key;

        private static Dictionary<string, Tag> _tags = new();
        [NonSerialized] public static readonly Tag Invalid = Get("");

        private Tag(string key)
        {
            Key = key;
            Initialize();
        }
        
        private void Initialize()
        {
            if (!string.IsNullOrEmpty(Key)
                && !_tags.ContainsKey(Key))
            {
                _tags.Add(Key, this);
            }
        }

        public static Tag Get(string key)
        {
            if (_tags.TryGetValue(key, out var tag))
            {
                return tag;
            }
            else
            {
                return new Tag(key);
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Tag);
        }

        public bool Equals(Tag other)
        {
            if (other is null)
            {
                return false;
            }

            if (System.Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return Key == other.Key;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Key))
            {
                return Invalid.GetHashCode();
            }

            return Key.GetHashCode();
        }

        public static bool operator ==(Tag lhs, Tag rhs)
        {
            if (lhs is null && rhs is null)
            {
                return true;
            }

            if (lhs is null || rhs is null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Tag lhs, Tag rhs)
        {
            return !(lhs == rhs);
        }
    }
}
