using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace framework.utility
{
    public class Bucket<TKey, TValue>
    {
        public int Count => BucketEntriesByKey.Count;

        protected readonly Dictionary<TKey, TValue> BucketEntriesByKey = new();

        /// <summary> Attempts to add an entry to the bucket. Returns false if the bucket already has the given key </summary>
        public virtual bool Add(TKey key, TValue value)
        {
            if (BucketEntriesByKey.TryAdd(key, value))
            {
                return true;
            }

            return false;
        }

        /// <summary> Attempts to remove an entry from the bucket by key. Returns false if the bucket doesn't have the given key </summary>
        public virtual bool Remove(TKey key)
        {
            if (BucketEntriesByKey.ContainsKey(key))
            {
                return BucketEntriesByKey.Remove(key);
            }

            return false;
        }

        public virtual bool Has(TKey key)
        {
            return BucketEntriesByKey.ContainsKey(key);
        }
    }

    public class TruthBucket<TKey> : Bucket<TKey, bool>
    {
        public virtual bool AnyTruth()
        {
            return BucketEntriesByKey.Values.FirstOrDefault(b => b);
        }

        public virtual bool NoTruth()
        {
            return BucketEntriesByKey.Values.FirstOrDefault(b => b) == false;
        }
    }

    public class StackBucket<TKey, TValue> : Bucket<TKey, TValue>
    {
        public readonly Action<TValue> OnTopValueChanged = delegate { };

        private KeyValuePair<TKey, TValue> _topPair;


        public bool Peek(out KeyValuePair<TKey, TValue> outPair)
        {
            if (_topPair.Key != null)
            {
                outPair = _topPair;
                return true;
            }

            outPair = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            return false;
        }

        public bool Pop(out KeyValuePair<TKey, TValue> outPair)
        {
            if (BucketEntriesByKey.ContainsKey(_topPair.Key))
            {
                if (Remove(_topPair.Key))
                {
                    outPair = _topPair;
                    return true;
                }
            }

            outPair = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            return false;
        }

        public bool Push(TKey key, TValue value)
        {
            return Add(key, value);
        }

        public override bool Add(TKey key, TValue value)
        {
            if (base.Add(key, value))
            {
                _topPair = new KeyValuePair<TKey, TValue>(key, value);
                OnTopValueChanged.Invoke(_topPair.Value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Remove(TKey key)
        {
            if (base.Remove(key))
            {
                bool topPairChanged = false;

                // Nullify our cached top value if we just removed it
                if (_topPair.Key.Equals(key))
                {
                    _topPair = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
                    topPairChanged = true;
                }

                // If our cached top value is null, update it to whatever was most recently added to the bucket (if anything)
                if (_topPair.Key == null
                    && BucketEntriesByKey.Count > 0)
                {
                    _topPair = BucketEntriesByKey.ElementAt(BucketEntriesByKey.Count - 1);
                    topPairChanged = true;
                }

                if (topPairChanged)
                {
                    OnTopValueChanged.Invoke(_topPair.Value);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary> Does not allow entries with duplicate priorities due to the nature of SortedList </summary>
    /// <typeparam name="TPriority"> The type to use as the priority comparer - int is allowed </typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PriorityBucket<TKey, TValue, TPriority>
    {
        public readonly Action<TKey> OnHighestPriorityKeyChanged = delegate { };
        public readonly Action<TValue> OnHighestPriorityValueChanged = delegate { };

        public int Count => _sortedList.Count;

        private TKey _lastHighestPriorityKey;
        private TValue _lastHighestPriorityValue;
        private readonly SortedList<TPriority, KeyValuePair<TKey, TValue>> _sortedList = new();
        
        /// <summary> Attempts to add an entry to the bucket. Returns false if the bucket already has the given key </summary>
        public virtual bool Add(TKey key, TValue value, TPriority priority)
        {
            foreach (KeyValuePair<TPriority, KeyValuePair<TKey, TValue>> pair in _sortedList.ToArray())
            {
                if (pair.Value.Key.Equals(key)) // If we already have an entry for this key
                {
                    if (!pair.Key.Equals(priority))
                    {
                        _sortedList.Remove(pair.Key); // Remove the entry with the old priority
                        _sortedList.Add(priority, new KeyValuePair<TKey, TValue>(key, value));
                        OnSortedListChanged();

                        return true;
                    }
                    else
                    {
                        _sortedList[pair.Key] = new KeyValuePair<TKey, TValue>(key, value);
                        OnSortedListChanged();

                        return true;
                    }
                }
            }

            // If we already have this level of priority, don't add this entry
            if (_sortedList.ContainsKey(priority))
            {
                return false;
            }

            _sortedList.Add(priority, new KeyValuePair<TKey, TValue>(key, value));
            OnSortedListChanged();

            return true;
        }

        /// <summary> Attempts to remove an entry from the bucket by key. Returns false if the bucket doesn't have the given key </summary>
        public virtual bool Remove(TKey key)
        {
            foreach (KeyValuePair<TPriority, KeyValuePair<TKey, TValue>> pair in _sortedList.ToArray())
            {
                if (pair.Value.Key.Equals(key))
                {
                    _sortedList.Remove(pair.Key);
                    OnSortedListChanged();
                    return true;
                }
            }

            return false;
        }

        public virtual bool Has(TKey key)
        {
            foreach (var pair in _sortedList.Values)
            {
                if (pair.Key.Equals(key))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnSortedListChanged()
        {
            var cachedLastHighestPriorityKey = _lastHighestPriorityKey;
            _lastHighestPriorityKey = _sortedList.Count > 0 ? _sortedList.ElementAt(_sortedList.Count - 1).Value.Key : default(TKey);
            if (!_lastHighestPriorityKey!.Equals(cachedLastHighestPriorityKey))
            {
                OnHighestPriorityKeyChanged.Invoke(_lastHighestPriorityKey);
            }

            var cachedLastHighestPriorityValue = _lastHighestPriorityValue;
            _lastHighestPriorityValue = _sortedList.Count > 0 ? _sortedList.ElementAt(_sortedList.Count - 1).Value.Value : default(TValue);
            if (!_lastHighestPriorityValue!.Equals(cachedLastHighestPriorityValue))
            {
                OnHighestPriorityValueChanged.Invoke(_lastHighestPriorityValue);
            }
        }
    }
}
