using System.Collections;
using System.Collections.Generic;

namespace Minikit
{
    public class TagContainer : IEnumerable<Tag>
    {
        private readonly List<Tag> _tags = new();

        public TagContainer()
        {

        }

        public TagContainer(Tag tag)
        {
            _tags.Add(tag);
        }

        public TagContainer(List<Tag> tags)
        {
            this._tags.AddRange(tags);
        }


        public List<Tag> GetTags()
        {
            return _tags;
        }

        public bool HasTag(Tag tag)
        {
            return _tags.Contains(tag);
        }

        public bool HasAnyTags(TagContainer tagContainer)
        {
            foreach (var tag in tagContainer.GetTags())
            {
                if (HasTag(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAllTags(TagContainer tagContainer)
        {
            foreach (var tag in tagContainer.GetTags())
            {
                if (!HasTag(tag))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsEmpty()
        {
            return _tags.Count == 0;
        }

        public int Count()
        {
            return _tags.Count;
        }

        public void AddTag(Tag tag)
        {
            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
            }
        }

        public void AddTags(TagContainer tagContainer)
        {
            foreach (var tag in tagContainer)
            {
                AddTag(tag);
            }
        }

        public void RemoveTag(Tag tag)
        {
            if (_tags.Contains(tag))
            {
                _tags.Remove(tag);
            }
        }

        public void RemoveTags(TagContainer tagContainer)
        {
            foreach (var tag in tagContainer)
            {
                RemoveTag(tag);
            }
        }

        public IEnumerator<Tag> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
