using J113D.UndoRedo.Trackables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J113D.UndoRedo
{
    /// <summary>
    /// Undo-Redo Change system
    /// </summary>
    public class ChangeTracker
    {
        #region Private

        private long _resets;
        private long _capacityCausedShifts;
        private int _currentChangeIndex;
        private readonly Stack<TrackGroup> _activeGroups;
        private readonly List<ITrackable> _trackedChanges;

        #endregion

        /// <summary>
        /// Used for checking if a change tracker is on the same change-state
        /// </summary>
        public readonly struct Pin
        {
            private readonly ChangeTracker _tracker;
            private readonly ITrackable? _tracking;
            private readonly int _index;
            private readonly long _resets;

            public bool IsValid
            {
                get
                {
                    if(_tracker._resets != _resets)
                    {
                        return false;
                    }

                    int realChangeIndex = (int)(_index - _tracker._capacityCausedShifts);

                    if(realChangeIndex != _tracker._currentChangeIndex)
                    {
                        return false;
                    }
                    else if(realChangeIndex >= 0)
                    {
                        return _tracker._trackedChanges[realChangeIndex].Equals(_tracking);
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            internal Pin(ChangeTracker tracker)
            {
                _tracker = tracker;
                _index = tracker._currentChangeIndex;
                _tracking = _index == -1 ? null : tracker._trackedChanges[_index];
                _resets = tracker._resets;
            }
        }


        /// <summary>
        /// Whether there are any changes that can be undone
        /// </summary>
        public bool CanUndo => _currentChangeIndex > -1;

        /// <summary>
        /// Whether there are any changes that can be redone
        /// </summary>
        public bool CanRedo => _currentChangeIndex + 1 < _trackedChanges.Count;

        /// <summary>
        /// Maximum number of changes that can be tracked at the same time.
        /// <br/> Setting 0 disables the limit.
        /// <br/> Changing to a value above 0 will reset the currently tracked changes.
        /// </summary>
        public int ChangeLimit
        {
            get => _trackedChanges.Capacity;
            set
            {
                if(ChangeLimit == value)
                {
                    return;
                }
                else if(value > 0)
                {
                    Reset();
                }

                _trackedChanges.Capacity = ChangeLimit;
            }
        }


        /// <summary>
        /// Creates a new change tracker
        /// </summary>
        public ChangeTracker(int capacity = 0)
        {
            _activeGroups = [];
            _trackedChanges = [];
            _trackedChanges.Capacity = capacity;
            _currentChangeIndex = -1;
        }


        private void AddTrackable(ITrackable trackable)
        {
            int removeStart = _currentChangeIndex + 1;
            int removeCount = _trackedChanges.Count - removeStart;
            if(removeCount > 0)
            {
                _trackedChanges.RemoveRange(removeStart, removeCount);
            }

            if(_trackedChanges.Capacity > 0 && _trackedChanges.Count == _trackedChanges.Capacity)
            {
                _trackedChanges.RemoveAt(0);
                _capacityCausedShifts++;
            }
            else
            {
                _currentChangeIndex++;
            }

            _trackedChanges.Add(trackable);
        }

        /// <summary>
        /// Creates a pin for the current state of the tracker. Can be used to check if a change has been tracked, undone or redone.
        /// </summary>
        public Pin PinCurrent()
        {
            return new(this);
        }


        /// <summary>
        /// Resets the tracked changes. <br/>
        /// If no changes have been tracked, no reset will be performed.
        /// </summary>
        public void Reset()
        {
            if(_activeGroups.Count > 0)
            {
                throw new InvalidOperationException("Programming error: Cannot perform reset while grouping is active!\nMake sure to close every grouping using EndGroup()!");
            }

            if(_trackedChanges.Count == 0)
            {
                return;
            }

            _trackedChanges.Clear();
            _currentChangeIndex = -1;
            _capacityCausedShifts = 0;
            _resets++;
        }

        /// <summary>
        /// Undoes the last change.
        /// </summary>
        public bool Undo()
        {
            if(_activeGroups.Count > 0)
            {
                throw new InvalidOperationException("Programming error: Cannot perform undo while grouping is active!\nMake sure to close every grouping using EndGroup()!");
            }

            if(_currentChangeIndex == -1)
            {
                return false;
            }

            _trackedChanges[_currentChangeIndex].Undo();
            _currentChangeIndex--;
            return true;
        }

        /// <summary>
        /// Redoes the last undone change.
        /// </summary>
        public bool Redo()
        {
            if(_activeGroups.Count > 0)
            {
                throw new InvalidOperationException("Programming error: Cannot perform redo while grouping is active\nMake sure to close every grouping using EndGroup()!");
            }

            if(_currentChangeIndex == _trackedChanges.Count - 1)
            {
                return false;
            }

            _currentChangeIndex++;
            _trackedChanges[_currentChangeIndex].Redo();
            return true;
        }


        /// <summary>
        /// Groups all incoming changes into one change until <see cref="EndGroup"/> is called. <br/>
        /// Groups get nested.
        /// </summary>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public void BeginGroup(string? origin = null)
        {
            _activeGroups.Push(new TrackGroup(origin));
        }

        /// <summary>
        /// Adds an action callback to execute after a group undo/redo. <br/>
        /// Immediately invokes the callback.
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException">Thrown if no grouping is active</exception>
        public void AddGroupPostCallback(Action callback)
        {
            if(!_activeGroups.TryPeek(out TrackGroup? group))
            {
                throw new InvalidOperationException("No grouping active!");
            }

            group.PostCallbacks.Add(callback);
            callback.Invoke();
        }

        /// <summary>
        /// Adds a property changed event to be invoked after a group is undone/redone. <br/>
        /// Immediately invokes the event.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <exception cref="InvalidOperationException">Thrown if no grouping is active</exception>
        public void AddGroupInvokePropertyChanged(IInvokeNotifyPropertyChanged target, string propertyName)
        {
            if(!_activeGroups.TryPeek(out TrackGroup? group))
            {
                throw new InvalidOperationException("No grouping active!");
            }

            group.PropertyChanges.Add((target, propertyName));
            target.InvokePropertyChanged(propertyName);
        }

        /// <summary>
        /// Closes the active grouping and adds it to the tracklist or parent group.
        /// </summary>
        /// <param name="discard">Undoes any changes done by the grouping and does not add it to the tracklist/parent group.</param>
        public void EndGroup(bool discard = false)
        {
            if(!_activeGroups.TryPop(out TrackGroup? group))
            {
                throw new InvalidOperationException("No grouping active!");
            }

            if(group.Changes.Count == 0
                && group.PostCallbacks.Count == 0
                && group.PropertyChanges.Count == 0)
            {
                return;
            }

            if(discard)
            {
                group.Undo();
                return;
            }

            if(!_activeGroups.TryPeek(out TrackGroup? parentGroup))
            {
                AddTrackable(
                    group.Changes.Count == 1
                    && group.PostCallbacks.Count == 0
                    && group.PropertyChanges.Count == 0
                    ? group.Changes[0]
                    : group);
            }
            else if(group.PostCallbacks.Count == 0
                && group.PropertyChanges.Count == 0)
            {
                parentGroup.Changes.AddRange(group.Changes);
            }
            else
            {
                parentGroup.Changes.Add(group);
            }
        }


        private void TrackChange(ITrackable trackable)
        {
            trackable.Redo();

            if(_activeGroups.TryPeek(out TrackGroup? group))
            {
                group.Changes.Add(trackable); 
            }
            else
            {
                AddTrackable(trackable);
            }
        }

        /// <summary>
        /// Tracks undo/redo callbacks. <br/>
        /// Immediately invokes <paramref name="redo"/>.
        /// </summary>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public void TrackCallbackChange(Action redo, Action undo, string? origin = null)
        {
            TrackChange(new CallbackChange(origin, redo, undo));
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
        public void TrackValueChange<T>(Action<T> changeCallback, T oldValue, T newValue, string? origin = null)
        {
            TrackChange(new ValueChange<T>(origin, changeCallback, oldValue, newValue));
        }

        /// <summary>
        /// Tracks the change of a property on a target object. The old value gets automatically recorded.
        /// Immediately sets <paramref name="value"/> to the property.
        /// </summary>
        /// <param name="target">Object of which a property should be changed.</param>
        /// <param name="propertyName">Name of the property on the target.</param>
        /// <param name="value">New property value.</param>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public void TrackPropertyChange(object target, string propertyName, object? value, string? origin = null)
        {
            TrackChange(new PropertyChange(origin, target, propertyName, value));
        }

        /// <summary>
        /// Tracks the change of a field on a target object. The old value gets automatically recorded.
        /// Immediately sets <paramref name="value"/> to the field.
        /// </summary>
        /// <param name="target">Object of which a field should be  changed.</param>
        /// <param name="fieldName">Name of the field on the target.</param>
        /// <param name="value">New field value.</param>
        /// <param name="origin">Name of the origin (for debugging purposes).</param>
        public void TrackFieldChange(object target, string fieldName, object? value, string? origin = null)
        {
            TrackChange(new FieldChange(origin, target, fieldName, value));
        }

        /// <summary>
        /// Tracks a change that does nothing.
        /// </summary>
        public void BlankChange(string? origin = null)
        {
            TrackChange(new BlankChange(origin));
        }

    }
}
