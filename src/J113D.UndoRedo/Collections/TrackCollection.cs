using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace J113D.UndoRedo.Collections
{
	/// <summary>
	/// A collection wrapper that automatically tracks changes
	/// </summary>
	/// <typeparam name="T">Collection element type</typeparam>
	public class TrackCollection<T> : ICollection<T>, IReadOnlyCollection<T>
	{
		private readonly ICollection<T> _collection;

		/// <summary>
		/// Changetracker to use. If none is provided, <see cref="GlobalChangeTracker.ActiveChangeTracker"/> one will be used instead.
		/// </summary>
		public ChangeTracker? Tracker { get; set; }

		private ChangeTracker UsedTracker => Tracker ?? GlobalChangeTracker.ActiveChangeTracker;


		/// <inheritdoc/>
		public int Count
			=> _collection.Count;

		/// <inheritdoc/>
		public bool IsReadOnly
			=> _collection.IsReadOnly;

		/// <summary>
		/// Creates a track collection that wraps around a collection
		/// </summary>
		/// <param name="collection">The collection to wrap around</param>
		/// <param name="tracker">The tracker to use</param>
		public TrackCollection(ICollection<T> collection, ChangeTracker? tracker)
		{
			_collection = collection;
			Tracker = tracker;
		}

		/// <summary>
		/// Creates a track collection that wraps around a collection
		/// </summary>
		/// <param name="collection">The collection to wrap around</param>
		public TrackCollection(ICollection<T> collection) : this(collection, null) { }

		/// <summary>
		/// Creates a track collection that wraps around a new collection
		/// </summary>
		/// <param name="tracker">The tracker to use</param>
		public TrackCollection( ChangeTracker? tracker) : this([], tracker) { }

		/// <summary>
		/// Creates a track collection that wraps around a new collection
		/// </summary>
		public TrackCollection() : this([], null) { }

		/// <inheritdoc/>
		public void Add(T item)
		{
			UsedTracker.TrackCallbackChange(
				() => _collection.Add(item),
				() => _collection.Remove(item),
				"Collection.Add");
		}

		/// <inheritdoc/>
		public void Clear()
		{
			T[] contents = _collection.ToArray();

			UsedTracker.TrackCallbackChange(
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

		/// <inheritdoc/>
		public bool Contains(T item)
		{
			return _collection.Contains(item);
		}

		/// <inheritdoc/>
		public void CopyTo(T[] array, int arrayIndex)
		{
			_collection.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator()
		{
			return _collection.GetEnumerator();
		}

		/// <inheritdoc/>
		public bool Remove(T item)
		{
			if(!Contains(item))
			{
				return false;
			}

			UsedTracker.TrackCallbackChange(
				() => _collection.Remove(item),
				() => _collection.Add(item),
				"Collection.Remove");

			return true;
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _collection.GetEnumerator();
		}
	}
}
