using System;

namespace J113D.UndoRedo.Test
{
    [TestClass]
    public class GroupTests
    {
        [TestMethod]
        public void Group_Base()
        {
            ChangeTracker tracker = new();

            tracker.BeginGroup();
            tracker.BlankChange();
            tracker.BlankChange();
            tracker.EndGroup();

            Assert.IsTrue(tracker.CanUndo);
        }

        [TestMethod]
        public void Group_Nested()
        {
            ChangeTracker tracker = new();

            tracker.BeginGroup();
            tracker.BlankChange();
            tracker.BlankChange();

            tracker.BeginGroup();
            tracker.BlankChange();
            tracker.BlankChange();

            tracker.EndGroup();
            Assert.IsFalse(tracker.CanUndo);

            tracker.EndGroup();
            Assert.IsTrue(tracker.CanUndo);
        }

        [TestMethod]
        public void Group_InvokePropertyChanged()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            bool propertyChanged = false;
            container.PropertyChanged += (s, e) => propertyChanged = true;

            tracker.BeginGroup();
            tracker.AddGroupInvokePropertyChanged(container, nameof(TestContainer.StringProperty));
            
            Assert.IsTrue(propertyChanged);
            propertyChanged = false;

            tracker.EndGroup();
            Assert.IsFalse(propertyChanged);

            tracker.Undo();
            Assert.IsTrue(propertyChanged);
            propertyChanged = false;

            tracker.Redo();
            Assert.IsTrue(propertyChanged);
            propertyChanged = false;
        }

        [TestMethod]
        public void Group_PostCallback()
        {
            ChangeTracker tracker = new();
            bool propertyChanged = false;

            tracker.BeginGroup();
            tracker.AddGroupPostCallback(() => propertyChanged = true);
            Assert.IsTrue(propertyChanged);
            propertyChanged = false;

            tracker.EndGroup();
            Assert.IsFalse(propertyChanged);

            tracker.Undo();
            Assert.IsTrue(propertyChanged);
            propertyChanged = false;

            tracker.Redo();
            Assert.IsTrue(propertyChanged);
            propertyChanged = false;
        }

        [TestMethod]
        public void Group_NestingOrder_Undo()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            int counter = 0;

            static void Nop()
            { }

            void AssertCounter(int target)
            {
                Assert.AreEqual(target, counter);
                counter++;
            }

            void AssertCallback(int target)
            {
                tracker.TrackCallbackChange(Nop, () => AssertCounter(target));
            }
            
            void AssertPostCallback(int target)
            {
                counter = target;
                tracker.AddGroupPostCallback(() => AssertCounter(target));
            }

            void AssertPropertyChanged(int target)
            {
                tracker.AddGroupInvokePropertyChanged(container, target.ToString());
                container.PropertyChanged += (s, e) =>
                {
                    if(e.PropertyName == target.ToString())
                    {
                        AssertCounter(target);
                    }
                };
            }

            tracker.BeginGroup();
            {
                AssertCallback(4);
                AssertPostCallback(5);
                AssertPropertyChanged(7);

                tracker.BeginGroup();
                {
                    AssertCallback(1);
                    AssertPostCallback(2);
                    AssertPropertyChanged(3);
                }

                tracker.EndGroup(false);

                AssertCallback(0);
                AssertPostCallback(6);
                AssertPropertyChanged(8);
            }

            tracker.EndGroup(false);

            counter = 0;
            tracker.Undo();
            Assert.AreEqual(counter, 9);
        }

        [TestMethod]
        public void Group_NestingOrder_Redo()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            bool assert = false;
            int counter = 0;

            static void Nop()
            { }

            void AssertCounter(int target)
            {
                if(!assert)
                {
                    return;
                }

                Assert.AreEqual(target, counter);
                counter++;
            }

            void AssertCallback(int target)
            {
                tracker.TrackCallbackChange(() => AssertCounter(target), Nop, target.ToString());
            }

            void AssertPostCallback(int target)
            {
                counter = target;
                tracker.AddGroupPostCallback(() => AssertCounter(target));
            }

            void AssertPropertyChanged(int target)
            {
                tracker.AddGroupInvokePropertyChanged(container, target.ToString());
                container.PropertyChanged += (s, e) =>
                {
                    if(e.PropertyName == target.ToString())
                    {
                        AssertCounter(target);
                    }
                };
            }

            tracker.BeginGroup();
            {
                AssertCallback(0);
                AssertPostCallback(5);
                AssertPropertyChanged(7);

                tracker.BeginGroup();
                {
                    AssertCallback(1);
                    AssertPostCallback(2);
                    AssertPropertyChanged(3);
                }

                tracker.EndGroup(false);

                AssertCallback(4);
                AssertPostCallback(6);
                AssertPropertyChanged(8);
            }

            tracker.EndGroup(false);

            tracker.Undo();
            counter = 0;
            assert = true;
            tracker.Redo();
            Assert.AreEqual(counter, 9);
        }

        [TestMethod]
        public void Group_InactiveExceptions()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            Assert.ThrowsException<InvalidOperationException>(() => tracker.AddGroupInvokePropertyChanged(container, string.Empty));
            Assert.ThrowsException<InvalidOperationException>(() => tracker.AddGroupPostCallback(() => { }));
        }

        [TestMethod]
        public void Group_ActiveExceptions()
        {
            ChangeTracker tracker = new();
            tracker.BeginGroup();

            Assert.ThrowsException<InvalidOperationException>(() => tracker.Undo());
            Assert.ThrowsException<InvalidOperationException>(() => tracker.Redo());
            Assert.ThrowsException<InvalidOperationException>(tracker.Reset);
        }

        [TestMethod]
        public void Group_Discard()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            string oldValue = container.StringProperty;
            string newValue = "Test";

            tracker.BeginGroup();
            tracker.TrackPropertyChange(container, nameof(TestContainer.StringProperty), newValue);
            tracker.EndGroup(discard: true);

            Assert.AreEqual(container.StringProperty, oldValue);
        }

        [TestMethod]
        public void Group_Discard_Nested()
        {
            ChangeTracker tracker = new();
            TestContainer container = new();

            string oldFieldValue = container.stringField;
            string newValue = "Test";

            tracker.BeginGroup();
            tracker.TrackPropertyChange(container, nameof(TestContainer.StringProperty), newValue);
            tracker.BeginGroup();
            tracker.TrackFieldChange(container, nameof(TestContainer.stringField), newValue);
            tracker.EndGroup(discard: true);
            tracker.EndGroup();

            Assert.AreEqual(container.stringField, oldFieldValue);
            Assert.AreEqual(container.StringProperty, newValue);
        }
    }
}
