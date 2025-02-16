using System;
using System.Collections;
using System.Collections.Generic;

namespace J113D.UndoRedo.Collections
{
    public class TrackList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly IList<T> _list;

        /// <summary>
        /// Changetracker to use. If none is provided, <see cref="GlobalChangeTracker.ActiveChangeTracker"/> one will be used instead.
        /// </summary>
        public ChangeTracker? Tracker { get; set; }

        private ChangeTracker UsedTracker => Tracker ?? GlobalChangeTracker.ActiveChangeTracker;


        /// <inheritdoc/>
        public T this[int index]
        {
            get => _list[index];
            set
            {
                T previousItem = _list[index];

                UsedTracker.TrackCallbackChange(
                    () => _list[index] = value,
                    () => _list[index] = previousItem,
                    "List[]");
            }
        }

        /// <inheritdoc/>
        public int Count => _list.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => _list.IsReadOnly;


        public TrackList(IList<T> list, ChangeTracker? tracker)
        {
            _list = list;
            Tracker = tracker;
        }

        public TrackList(IList<T> list) : this(list, null) { }

        public TrackList(ChangeTracker? tracker) : this([], tracker) { }

        public TrackList() : this([], null) { }


        /// <inheritdoc/>
        public void Add(T item)
        {
            UsedTracker.TrackCallbackChange(
                () => _list.Add(item),
                () => _list.Remove(item),
                "List.Add");
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<T> range)
        {
            UsedTracker.BeginGroup("List.AddRange");
            foreach(T item in range)
            {
                Add(item);
            }

            UsedTracker.EndGroup();
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            UsedTracker.TrackCallbackChange(
                () => _list.Insert(index, item),
                () => _list.RemoveAt(index),
                "List.Insert");
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if(index < 0)
            {
                UsedTracker.BlankChange("List.Insert");
                return false;
            }

            UsedTracker.TrackCallbackChange(
                () => _list.RemoveAt(index),
                () => _list.Insert(index, item),
                "List.Insert");

            return true;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if(index >= _list.Count)
            {
                UsedTracker.BlankChange("List.RemoveAt");
                return;
            }

            T item = this[index];

            UsedTracker.TrackCallbackChange(
                () => _list.RemoveAt(index),
                () => _list.Insert(index, item),
                "List.RemoveAt");
        }

        /// <inheritdoc/>
        public void Clear()
        {
            T[] contents = [.. _list];

            UsedTracker.TrackCallbackChange(
                _list.Clear,
                () =>
                {
                    foreach(T item in contents)
                    {
                        _list.Add(item);
                    }
                },
                "List.Clear");
        }


        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <inheritdoc/>
        public T? Find(Predicate<T> match)
        {
            foreach(T item in this)
            {
                if(match(item))
                {
                    return item;
                }
            }

            return default;
        }

        /// <inheritdoc/>
        public List<T> FindAll(Predicate<T> match)
        {
            List<T> result = [];

            foreach(T item in this)
            {
                if(match(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            for(int i = startIndex; i < count; i++)
            {
                T item = this[i];
                if(match(item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, Count, match);
        }

        /// <inheritdoc/>
        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, Count, match);
        }

        /// <inheritdoc/>
        public T? FindLast(Predicate<T> match)
        {
            T? result = default;

            foreach(T item in this)
            {
                if(match(item))
                {
                    result = item;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            int result = -1;

            for(int i = startIndex; i < count; i++)
            {
                T item = this[i];
                if(match(item))
                {
                    result = i;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, Count, match);
        }

        /// <inheritdoc/>
        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(0, Count, match);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
