using static J113D.UndoRedo.GlobalChangeTracker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace J113D.UndoRedo.Collections
{
    public class TrackList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly IList<T> _list;


        public T this[int index]
        {
            get => _list[index];
            set
            {
                T previousItem = _list[index];

                TrackCallbackChange(
                    () => _list[index] = value,
                    () => _list[index] = previousItem,
                    "List[]");
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;


        public TrackList(IList<T> list)
        {
            _list = list;
        }

        public TrackList() : this([]) { }


        public void Add(T item)
        {
            TrackCallbackChange(
                () => _list.Add(item),
                () => _list.Remove(item),
                "List.Add");
        }

        public void AddRange(IEnumerable<T> range)
        {
            BeginChangeGroup("List.AddRange");
            foreach(T item in range)
            {
                Add(item);
            }

            EndChangeGroup();
        }

        public void Insert(int index, T item)
        {
            TrackCallbackChange(
                () => _list.Insert(index, item),
                () => _list.RemoveAt(index),
                "List.Insert");
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if(index < 0)
            {
                BlankChange("List.Insert");
                return false;
            }

            TrackCallbackChange(
                () => _list.RemoveAt(index),
                () => _list.Insert(index, item),
                "List.Insert");

            return true;
        }

        public void RemoveAt(int index)
        {
            if(index >= _list.Count)
            {
                BlankChange("List.RemoveAt");
                return;
            }

            T item = this[index];

            TrackCallbackChange(
                () => _list.RemoveAt(index),
                () => _list.Insert(index, item),
                "List.RemoveAt");
        }

        public void Clear()
        {
            T[] contents = [.. _list];

            TrackCallbackChange(
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



        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

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

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, Count, match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, Count, match);
        }

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

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, Count, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(0, Count, match);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
