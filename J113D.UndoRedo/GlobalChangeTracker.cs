using System;
using System.Diagnostics;

namespace J113D.UndoRedo
{
    [DebuggerStepThrough]
    public static class GlobalChangeTracker
    {
        public static ChangeTracker ActiveChangeTracker { get; private set; } = new();

        /// <summary>
        /// Whether there are any changes that can be undone
        /// </summary>
        public static bool CanUndo => ActiveChangeTracker.CanUndo;

        /// <summary>
        /// Whether there are any changes that can be redone
        /// </summary>
        public static bool CanRedo => ActiveChangeTracker.CanRedo;

        /// <summary>
        /// Maximum number of changes that can be tracked at the same time.
        /// <br/> Setting 0 disables the limit.
        /// <br/> Changing to a value above 0 will reset the currently tracked changes.
        /// </summary>
        public static int ChangeLimit => ActiveChangeTracker.ChangeLimit;

        public static void UseTracker(this ChangeTracker tracker)
        {
            ActiveChangeTracker = tracker;
        }

        /// <summary>
        /// Creates a pin for the current state of the tracker. Can be used to check if a change has been tracked, undone or redone.
        /// </summary>
        public static ChangeTracker.Pin PinCurrent()
        {
            return ActiveChangeTracker.PinCurrent();
        }


        /// <summary>
        /// Resets the tracked changes.
        /// </summary>
        public static void ResetTracker()
        {
            ActiveChangeTracker.Reset();
        }

        /// <summary>
        /// Undoes the last change.
        /// </summary>
        public static bool UndoChange()
        {
            return ActiveChangeTracker.Undo();
        }

        /// <summary>
        /// Redoes the last undone change.
        /// </summary>
        public static bool RedoChange()
        {
            return ActiveChangeTracker.Redo();
        }


        /// <summary>
        /// Groups all incoming changes into one change until <see cref="EndChangeGroup"/> is called. <br/>
        /// Groups get nested.
        /// </summary>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public static void BeginChangeGroup(string? origin = null)
        {
            ActiveChangeTracker.BeginGroup(origin);
        }

        /// <summary>
        /// Adds an action callback to execute after a group undo/redo. <br/>
        /// Immediately invokes the callback.
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException">Thrown if no grouping is active</exception>
        public static void AddChangeGroupPostCallback(Action callback)
        {
            ActiveChangeTracker.AddGroupPostCallback(callback);
        }

        /// <summary>
        /// Adds a property changed event to be invoked after a group is undone/redone. <br/>
        /// Immediately invokes the event.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <exception cref="InvalidOperationException">Thrown if no grouping is active</exception>
        public static void AddChangeGroupInvokePropertyChanged(this IInvokeNotifyPropertyChanged target, string propertyName)
        {
            ActiveChangeTracker.AddGroupInvokePropertyChanged(target, propertyName);
        }

        /// <summary>
        /// Closes the active grouping and adds it to the tracklist or parent group.
        /// </summary>
        /// <param name="discard">Undoes any changes done by the grouping and does not add it to the tracklist/parent group.</param>
        public static void EndChangeGroup(bool discard = false)
        {
            ActiveChangeTracker.EndGroup(discard);
        }


        /// <summary>
        /// Tracks undo/redo callbacks. <br/>
        /// Immediately invokes <paramref name="redo"/>.
        /// </summary>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public static void TrackCallbackChange(Action redo, Action undo, string? origin = null)
        {
            ActiveChangeTracker.TrackCallbackChange(redo, undo, origin);
        }

        /// <summary>
        /// Tracks a value change via callback. <br/>
        /// Immediately invokes <paramref name="changeCallback"/> with <paramref name="newValue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="changeCallback"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public static void TrackValueChange<T>(Action<T> changeCallback, T oldValue, T newValue, string? origin = null)
        {
            ActiveChangeTracker.TrackValueChange(changeCallback, oldValue, newValue, origin);
        }

        /// <summary>
        /// Tracks the change of a property on a target object. The old value gets automatically recorded.
        /// Immediately sets <paramref name="value"/> to the property.
        /// </summary>
        /// <param name="target">Object of which a property should be changed.</param>
        /// <param name="propertyName">Name of the property on the target.</param>
        /// <param name="value">New property value.</param>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public static void TrackPropertyChange(object target, string propertyName, object? value, string? origin = null)
        {
            ActiveChangeTracker.TrackPropertyChange(target, propertyName, value, origin);
        }

        /// <summary>
        /// Tracks the change of a field on a target object. The old value gets automatically recorded.
        /// Immediately sets <paramref name="value"/> to the field.
        /// </summary>
        /// <param name="target">Object of which a field should be  changed.</param>
        /// <param name="fieldName">Name of the field on the target.</param>
        /// <param name="value">New field value.</param>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public static void TrackFieldChange(object target, string fieldName, object? value, string? origin = null)
        {
            ActiveChangeTracker.TrackFieldChange(target, fieldName, value, origin);
        }

        /// <summary>
        /// Tracks a change that does nothing.
        /// </summary>
        public static void BlankChange(string? origin = null)
        {
            ActiveChangeTracker.BlankChange(origin);
        }

    }
}
