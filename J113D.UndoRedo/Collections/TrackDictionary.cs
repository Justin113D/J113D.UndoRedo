using static J113D.UndoRedo.GlobalChangeTracker;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace J113D.UndoRedo.Collections
{
    public class TrackDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                TValue previousItem = _dictionary[key];

                TrackCallbackChange(
                    () => _dictionary[key] = value,
                    () => _dictionary[key] = previousItem,
                    "Dictionary[]");
            }
        }

        public ICollection<TKey> Keys
            => _dictionary.Keys;

        public ICollection<TValue> Values
            => _dictionary.Values;

        public int Count
            => _dictionary.Count;

        public bool IsReadOnly
            => _dictionary.IsReadOnly;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
            => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
            => Values;

        public TrackDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public TrackDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public void Add(TKey key, TValue value)
        {
            TrackCallbackChange(
                () => _dictionary.Add(key, value),
                () => _dictionary.Remove(key),
                "Dictionary.Add");
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            TrackCallbackChange(
                () => _dictionary.Add(item),
                () => _dictionary.Remove(item),
                "Dictionary.Add");
        }

        public void Clear()
        {
            KeyValuePair<TKey, TValue>[] contents = _dictionary.ToArray();

            TrackCallbackChange(
                _dictionary.Clear,
                () =>
                {
                    foreach(KeyValuePair<TKey, TValue> item in contents)
                    {
                        _dictionary.Add(item);
                    }
                },
                "Dictionary.Clear");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            if(!TryGetValue(key, out TValue? value))
            {
                return false;
            }

            TrackCallbackChange(
                () => _dictionary.Remove(key),
                () => _dictionary.Add(key, value),
                "Dictionary.Remove");

            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if(!Contains(item))
            {
                return false;
            }

            TrackCallbackChange(
                () => _dictionary.Remove(item),
                () => _dictionary.Add(item),
                "Dictionary.Remove");

            return true;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
