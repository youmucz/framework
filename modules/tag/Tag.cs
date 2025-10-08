using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace framework.modules.tag
{
    [Serializable]
    public class Tag : IEquatable<Tag>
    {
        private readonly string _key;
        public string Key => _key;

        private static Dictionary<string, Tag> _tags = new();
        [NonSerialized] public static readonly Tag Invalid = Get("");

        private Tag(string key)
        {
            _key = key;
            Initialize();
        }
        
        private void Initialize()
        {
            if (!string.IsNullOrEmpty(_key) && !_tags.ContainsKey(_key))
                _tags.Add(_key, this);
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
            return Equals(obj as Tag);
        }

        public bool Equals(Tag other)
        {
            if (other is null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return _key == other._key;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(_key))
            {
                return Invalid.GetHashCode();
            }

            return _key.GetHashCode();
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
