using static J113D.UndoRedo.GlobalChangeTracker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace J113D.UndoRedo.Collections
{
    public class TrackCollection<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        private readonly ICollection<T> _collection;

        public int Count
            => _collection.Count;

        public bool IsReadOnly
            => _collection.IsReadOnly;

        public TrackCollection(ICollection<T> collection)
        {
            _collection = collection;
        }

        public void Add(T item)
        {
            TrackCallbackChange(
                () => _collection.Add(item),
                () => _collection.Remove(item),
                "Collection.Add");
        }

        public void Clear()
        {
            T[] contents = _collection.ToArray();

            TrackCallbackChange(
                _collection.Clear,
                () =>
                {
                    foreach(T item in contents)
                    {
                        _collection.Add(item);
                    }
                },
                "Collection.Clear");
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public bool Remove(T item)
        {
            if(!Contains(item))
            {
                return false;
            }

            TrackCallbackChange(
                () => _collection.Remove(item),
                () => _collection.Add(item),
                "Collection.Remove");

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}
