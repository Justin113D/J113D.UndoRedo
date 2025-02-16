namespace J113D.UndoRedo.Test
{
    [TestClass]
    public class TrackerTests
    {
        [TestMethod]
        public void Tracker_CanUndoRedo()
        {
            ChangeTracker tracker = new();

            Assert.IsFalse(tracker.CanUndo);
            Assert.IsFalse(tracker.CanRedo);

            tracker.BlankChange();

            Assert.IsTrue(tracker.CanUndo);
            Assert.IsFalse(tracker.CanRedo);

            tracker.Undo();

            Assert.IsFalse(tracker.CanUndo);
            Assert.IsTrue(tracker.CanRedo);

            tracker.Redo();

            Assert.IsTrue(tracker.CanUndo);
            Assert.IsFalse(tracker.CanRedo);
        }

        [TestMethod]
        public void Tracker_Reset()
        {
            ChangeTracker tracker = new();

            Assert.IsFalse(tracker.CanUndo);

            tracker.BlankChange();

            Assert.IsTrue(tracker.CanUndo);

            tracker.Reset();

            Assert.IsFalse(tracker.CanUndo);
        }
    }
}
