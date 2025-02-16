using J113D.UndoRedo.Collections;
using System;
using System.Linq;

namespace J113D.UndoRedo.Test
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        public void List_Set()
        {
            ChangeTracker tracker = new();
            TrackList<int> list = new([1, 2, 3], tracker)
            {
                [1] = 5 // calls list[1] = 5;
            };

            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(5, list[1]);

            tracker.Undo();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(2, list[1]);

            tracker.Redo();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(5, list[1]);
        }

        [TestMethod]
        public void List_Add()
        {
            ChangeTracker tracker = new();
            TrackList<int> list = new(tracker)
            {
                1 // calls list.Add(1);
            };

            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0]);

            tracker.Undo();
            Assert.AreEqual(0, list.Count);

            tracker.Redo();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0]);
        }

        [TestMethod]
        public void List_AddRange()
        {
            ChangeTracker tracker = new();
            TrackList<int> list = new(tracker);

            list.AddRange([1, 2, 3]);
            Assert.IsTrue(tracker.CanUndo);
            Assert.IsTrue(list.SequenceEqual([1, 2, 3]));

            tracker.Undo();
            Assert.AreEqual(0, list.Count);

            tracker.Redo();
            Assert.IsTrue(list.SequenceEqual([1, 2, 3]));
        }

        [TestMethod]
        public void List_Insert()
        {
            ChangeTracker tracker = new();
            TrackList<int> list = new([1, 2, 3], tracker);

            list.Insert(1, 5);
            Assert.IsTrue(tracker.CanUndo);
            Assert.IsTrue(list.SequenceEqual([1, 5, 2, 3]));

            tracker.Undo();
            Assert.IsTrue(list.SequenceEqual([1, 2, 3]));

            tracker.Redo();
            Assert.IsTrue(list.SequenceEqual([1, 5, 2, 3]));
        }

        [TestMethod]
        public void List_Remove()
        {
            ChangeTracker tracker = new();
            TrackList<int> list = new([1, 2, 3], tracker);

            list.Remove(1);
            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, list[0]);

            tracker.Undo();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);

            tracker.Redo();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(2, list[0]);
        }

        [TestMethod]
        public void List_RemoveAt()
        {
            ChangeTracker tracker = new();
            TrackList<int> list = new([1, 2, 3], tracker);

            list.RemoveAt(1);
            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(3, list[1]);

            tracker.Undo();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);

            tracker.Redo();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(3, list[1]);
        }

        [TestMethod]
        public void List_Clear()
        {
            ChangeTracker tracker = new();
            TrackList<int> list = new([1, 2, 3], tracker);

            list.Clear();
            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(0, list.Count);

            tracker.Undo();
            Assert.IsTrue(list.SequenceEqual([1, 2, 3]));

            tracker.Redo();
            Assert.AreEqual(0, list.Count);
        }
    }
}
