namespace J113D.UndoRedo.Test
{
    [TestClass]
    public class TrackableTests
    {
        [TestMethod]
        public void Property_UndoRedo()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            string oldValue = container.StringProperty;
            string newValue = "NewValue";

            tracker.TrackPropertyChange(container, nameof(container.StringProperty), newValue);

            Assert.AreEqual(container.StringProperty, newValue);

            tracker.Undo();
            Assert.AreEqual(container.StringProperty, oldValue);

            tracker.Redo();
            Assert.AreEqual(container.StringProperty, newValue);
        }

        [TestMethod]
        public void Field_UndoRedo()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            string oldValue = container.stringField;
            string newValue = "NewValue";

            tracker.TrackFieldChange(container, nameof(container.stringField), newValue);

            Assert.AreEqual(container.stringField, newValue);

            tracker.Undo();
            Assert.AreEqual(container.stringField, oldValue);

            tracker.Redo();
            Assert.AreEqual(container.stringField, newValue);
        }

        [TestMethod]
        public void Value_UndoRedo()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            string oldValue = container.StringProperty;
            string newValue = "NewValue";

            tracker.TrackValueChange((v) => container.StringProperty = v, oldValue, newValue);

            Assert.AreEqual(container.StringProperty, newValue);

            tracker.Undo();
            Assert.AreEqual(container.StringProperty, oldValue);

            tracker.Redo();
            Assert.AreEqual(container.StringProperty, newValue);
        }

        [TestMethod]
        public void Callback_UndoRedo()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            string oldValue = container.StringProperty;
            string newValue = "NewValue";

            tracker.TrackCallbackChange(
                () => container.StringProperty = newValue, 
                () => container.StringProperty = oldValue);

            Assert.AreEqual(container.StringProperty, newValue);

            tracker.Undo();
            Assert.AreEqual(container.StringProperty, oldValue);

            tracker.Redo();
            Assert.AreEqual(container.StringProperty, newValue);
        }
    }
}
