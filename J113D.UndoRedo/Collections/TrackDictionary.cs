using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace J113D.UndoRedo.Collections
{
    public class TrackDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// Changetracker to use. If none is provided, <see cref="GlobalChangeTracker.ActiveChangeTracker"/> one will be used instead.
        /// </summary>
        public ChangeTracker? Tracker { get; set; }

        private ChangeTracker UsedTracker => Tracker ?? GlobalChangeTracker.ActiveChangeTracker;


        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {

                if(_dictionary.TryGetValue(key, out TValue? oldValue))
                {
                    UsedTracker.TrackCallbackChange(
                        () => _dictionary[key] = value,
                        () => _dictionary[key] = oldValue,
                        "Dictionary[]");
                }
                else
                {
                    UsedTracker.TrackCallbackChange(
                        () => _dictionary[key] = value,
                        () => _dictionary.Remove(key),
                        "Dictionary[]");
                }

            }
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys
            => _dictionary.Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values
            => _dictionary.Values;

        /// <inheritdoc/>
        public int Count
            => _dictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly
            => _dictionary.IsReadOnly;

        /// <inheritdoc/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
            => Keys;

        /// <inheritdoc/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
            => Values;


        public TrackDictionary(IDictionary<TKey, TValue> dictionary, ChangeTracker? tracker)
        {
            _dictionary = dictionary;
            Tracker = tracker;
        }

        public TrackDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

        public TrackDictionary(ChangeTracker? tracker) : this(new Dictionary<TKey, TValue>(), tracker) { }

        public TrackDictionary() : this(new Dictionary<TKey, TValue>(), null) { }


        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            UsedTracker.TrackCallbackChange(
                () => _dictionary.Add(key, value),
                () => _dictionary.Remove(key),
                "Dictionary.Add");
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            UsedTracker.TrackCallbackChange(
                () => _dictionary.Add(item),
                () => _dictionary.Remove(item),
                "Dictionary.Add");
        }

        /// <inheritdoc/>
        public void Clear()
        {
            KeyValuePair<TKey, TValue>[] contents = _dictionary.ToArray();

            UsedTracker.TrackCallbackChange(
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

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            if(!TryGetValue(key, out TValue? value))
            {
                UsedTracker.BlankChange("Dictionary.Remove");
                return false;
            }

            UsedTracker.TrackCallbackChange(
                () => _dictionary.Remove(key),
                () => _dictionary.Add(key, value),
                "Dictionary.Remove");

            return true;
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if(!Contains(item))
            {
                UsedTracker.BlankChange("Dictionary.Remove");
                return false;
            }

            UsedTracker.TrackCallbackChange(
                () => _dictionary.Remove(item),
                () => _dictionary.Add(item),
                "Dictionary.Remove");

            return true;
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
