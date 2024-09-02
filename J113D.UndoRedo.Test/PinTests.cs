namespace J113D.UndoRedo.Test
{
    [TestClass]
    public class PinTests
    {
        [TestMethod]
        public void Pin_UndoRedo()
        {
            ChangeTracker tracker = new();
            ChangeTracker.Pin pin = tracker.PinCurrent();

            Assert.IsTrue(pin.IsValid);

            tracker.BlankChange();
            Assert.IsFalse(pin.IsValid);

            tracker.Undo();
            Assert.IsTrue(pin.IsValid);

            tracker.Redo();
            Assert.IsFalse(pin.IsValid);
        }

        [TestMethod]
        public void Pin_Reset()
        {
            ChangeTracker tracker = new();
            ChangeTracker.Pin pin = tracker.PinCurrent();

            tracker.Reset();
            Assert.IsTrue(pin.IsValid);

            tracker.BlankChange();
            tracker.Reset();

            Assert.IsFalse(pin.IsValid);
        }

        [TestMethod]
        public void Pin_LimitShift_Init()
        {
            ChangeTracker tracker = new(5);
            ChangeTracker.Pin pin = tracker.PinCurrent();

            for(int i = 0; i < tracker.ChangeLimit + 1; i++)
            {
                tracker.BlankChange();
            }

            while(tracker.CanUndo)
            {
                tracker.Undo();
            }

            Assert.IsFalse(pin.IsValid);
        }


        [TestMethod]
        public void Pin_LimitShift_Middle()
        {
            ChangeTracker tracker = new(5);

            for(int i = 0; i < tracker.ChangeLimit; i++)
            {
                tracker.BlankChange();
            }

            ChangeTracker.Pin pin = tracker.PinCurrent();

            for(int i = 0; i < tracker.ChangeLimit; i++)
            {
                tracker.BlankChange();
            }

            while(tracker.CanUndo)
            {
                tracker.Undo();
            }

            Assert.IsTrue(pin.IsValid);
        }

        [TestMethod]
        public void Pin_LimitShift_After()
        {
            ChangeTracker tracker = new(5);

            for(int i = 0; i < tracker.ChangeLimit + 1; i++)
            {
                tracker.BlankChange();
            }

            ChangeTracker.Pin pin = tracker.PinCurrent();
            Assert.IsTrue(pin.IsValid);
        }
    }
}
