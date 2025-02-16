using System;
using System.Collections.Generic;
using System.Linq;

namespace J113D.UndoRedo.Trackables
{
    internal class TrackGroup : ITrackable
    {
        public string? Origin { get; }

        public List<ITrackable> Changes { get; }
        public List<Action> PostCallbacks { get; }
        public HashSet<(IInvokeNotifyPropertyChanged target, string propertyName)> PropertyChanges { get; }


        public TrackGroup(string? origin)
        {
            Origin = origin;
            Changes = [];
            PostCallbacks = [];
            PropertyChanges = [];
        }


        public void Redo()
        {
            foreach(ITrackable trackable in Changes)
            {
                trackable.Redo();
            }

            InvokePost();
        }

        public void Undo()
        {
            foreach(ITrackable trackable in Changes.Reverse<ITrackable>())
            {
                trackable.Undo();
            }

            InvokePost();
        }

        private void InvokePost()
        {
            foreach(Action a in PostCallbacks)
            {
                a.Invoke();
            }

            foreach((IInvokeNotifyPropertyChanged target, string propertyName) in PropertyChanges)
            {
                target.InvokePropertyChanged(propertyName);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is TrackGroup group &&
                   Origin == group.Origin &&
                   EqualityComparer<List<ITrackable>>.Default.Equals(Changes, group.Changes) &&
                   EqualityComparer<List<Action>>.Default.Equals(PostCallbacks, group.PostCallbacks) &&
                   EqualityComparer<HashSet<(IInvokeNotifyPropertyChanged target, string propertyName)>>.Default.Equals(PropertyChanges, group.PropertyChanges);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin, Changes, PostCallbacks, PropertyChanges);
        }

        public override string ToString()
        {
            return $"[Group] {Origin}";
        }
    }
}
