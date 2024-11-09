using Godot;
using System.Collections;
using System.Collections.Generic;

namespace Minikit
{
    [System.Serializable]
    public enum TagQueryCondition
    {
        Any, // At least 1 tag in the query exists in the list of tags being tested against
        All, // All the tags in the query exist in the list of tags being tested against
        None // None of the tags in the query exist in the list of tags being tested against
    }

    [System.Serializable]
    public class TagQuery
    {
        [Export] private TagQueryCondition _condition = TagQueryCondition.Any;
        [Export] private List<Tag> _tagList;
        
        public TagQuery(TagQueryCondition condition, List<Tag> tagList)
        {
            _condition = condition;
            _tagList = tagList;
        }

        public bool Test(Tag tag)
        {
            return Test(new List<Tag>() { tag });
        }

        public bool Test(List<Tag> tagList)
        {
            if (_tagList == null || _tagList.Count == 0)
            {
                return true;
            }

            switch (_condition)
            {
                case TagQueryCondition.Any:
                    if (tagList == null)
                    {
                        return false;
                    }
                    foreach (var tag in _tagList)
                    {
                        if (tagList.Contains(tag))
                        {
                            return true;
                        }
                    }
                    return false;
                case TagQueryCondition.All:
                    if (tagList == null)
                    {
                        return false;
                    }
                    foreach (var tag in this._tagList)
                    {
                        if (!tagList.Contains(tag))
                        {
                            return false;
                        }
                    }
                    return true;
                case TagQueryCondition.None:
                    if (tagList == null)
                    {
                        return true;
                    }
                    foreach (var tag in this._tagList)
                    {
                        if (tagList.Contains(tag))
                        {
                            return false;
                        }
                    }
                    return true;
            }

            return false;
        }
    }
}
