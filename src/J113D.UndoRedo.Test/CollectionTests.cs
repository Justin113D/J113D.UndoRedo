using J113D.UndoRedo.Collections;
using System.Linq;

namespace J113D.UndoRedo.Test
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void Collection_Add()
        {
            ChangeTracker tracker = new();
            TrackCollection<int> collection = new([], tracker)
            {
                1 // calls collection.Add(1);
            };

            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(1, collection.First());

            tracker.Undo();
            Assert.AreEqual(0, collection.Count);

            tracker.Redo();
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(1, collection.First());
        }

        [TestMethod]
        public void Collection_Remove()
        {
            ChangeTracker tracker = new();
            TrackCollection<int> collection = new([1, 2, 3], tracker);

            collection.Remove(1);
            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.Contains(1));

            tracker.Undo();
            Assert.AreEqual(3, collection.Count);
            Assert.IsTrue(collection.Contains(1));

            tracker.Redo();
            Assert.AreEqual(2, collection.Count);
            Assert.IsFalse(collection.Contains(1));
        }


        [TestMethod]
        public void Collection_Clear()
        {
            ChangeTracker tracker = new();
            TrackCollection<int> collection = new([1, 2, 3], tracker);

            collection.Clear();
            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(0, collection.Count);

            tracker.Undo();
            Assert.IsTrue(collection.SequenceEqual([1, 2, 3]));

            tracker.Redo();
            Assert.AreEqual(0, collection.Count);
        }
    }
}
