using J113D.UndoRedo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace J113D.UndoRedo.Test
{
    [TestClass]
    public class DictionaryTests
    {
        [TestMethod]
        public void Dictionary_SetExisting()
        {
            ChangeTracker tracker = new();
            TrackDictionary<int, int> dictionary = new(new Dictionary<int, int>() { [0] = 10 }, tracker)
            {
                [0] = 20
            };

            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(20, dictionary[0]);

            tracker.Undo();
            Assert.AreEqual(10, dictionary[0]);

            tracker.Redo();
            Assert.AreEqual(20, dictionary[0]);
        }

        [TestMethod]
        public void Dictionary_SetNew()
        {
            ChangeTracker tracker = new();
            TrackDictionary<int, int> dictionary = new(tracker)
            {
                [0] = 10
            };

            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(10, dictionary[0]);

            tracker.Undo();
            Assert.IsFalse(dictionary.ContainsKey(0));

            tracker.Redo();
            Assert.AreEqual(10, dictionary[0]);
        }

        [TestMethod]
        public void Dictionary_Add()
        {
            ChangeTracker tracker = new();
            TrackDictionary<int, int> dictionary = new(tracker)
            {
                { 0, 10 }
            };

            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(10, dictionary[0]);

            tracker.Undo();
            Assert.IsFalse(dictionary.ContainsKey(0));

            tracker.Redo();
            Assert.AreEqual(10, dictionary[0]);
        }

        [TestMethod]
        public void Dictionary_AddPair()
        {
            ChangeTracker tracker = new();
            TrackDictionary<int, int> dictionary = new(tracker)
            {
                new KeyValuePair<int, int>(0, 10)
            };

            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(10, dictionary[0]);

            tracker.Undo();
            Assert.IsFalse(dictionary.ContainsKey(0));

            tracker.Redo();
            Assert.AreEqual(10, dictionary[0]);
        }

        [TestMethod]
        public void Dictionary_Remove()
        {
            ChangeTracker tracker = new();
            TrackDictionary<int, int> dictionary = new(new Dictionary<int, int>() { [0] = 10 }, tracker);

            dictionary.Remove(0);
            Assert.IsTrue(tracker.CanUndo);
            Assert.IsFalse(dictionary.ContainsKey(0));

            tracker.Undo();
            Assert.AreEqual(10, dictionary[0]);

            tracker.Redo();
            Assert.IsFalse(dictionary.ContainsKey(0));
        }

        [TestMethod]
        public void Dictionary_RemovePair()
        {
            ChangeTracker tracker = new();
            TrackDictionary<int, int> dictionary = new(new Dictionary<int, int>() { [0] = 10 }, tracker);

            dictionary.Remove(new KeyValuePair<int, int>(0, 10));
            Assert.IsTrue(tracker.CanUndo);
            Assert.IsFalse(dictionary.ContainsKey(0));

            tracker.Undo();
            Assert.AreEqual(10, dictionary[0]);

            tracker.Redo();
            Assert.IsFalse(dictionary.ContainsKey(0));
        }

        [TestMethod]
        public void Dictionary_Clear()
        {
            ChangeTracker tracker = new();
            TrackDictionary<int, int> dictionary = new(new Dictionary<int, int>() { [0] = 10, [1] = 20, [2] = 30 }, tracker);

            dictionary.Clear();
            Assert.IsTrue(tracker.CanUndo);
            Assert.AreEqual(0, dictionary.Count);

            tracker.Undo();
            Assert.AreEqual(3, dictionary.Count);
            Assert.AreEqual(dictionary[0], 10);
            Assert.AreEqual(dictionary[1], 20);
            Assert.AreEqual(dictionary[2], 30);

            tracker.Redo();
            Assert.AreEqual(0, dictionary.Count);
        }
    }
}
