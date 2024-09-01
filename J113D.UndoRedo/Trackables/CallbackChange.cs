using System;
using System.Collections.Generic;

namespace J113D.UndoRedo.Trackables
{
    internal readonly struct CallbackChange : ITrackable
    {
        public string? Origin { get; }

        private readonly Action _redoCallback;
        private readonly Action _undoCallback;

        public CallbackChange(string? origin, Action redoCallback, Action undoCallback)
        {
            Origin = origin;
            _redoCallback = redoCallback;
            _undoCallback = undoCallback;
        }

        public readonly void Redo()
        {
            _redoCallback.Invoke();
        }

        public readonly void Undo()
        {
            _undoCallback();
        }

        public override bool Equals(object? obj)
        {
            return obj is CallbackChange change &&
                   Origin == change.Origin &&
                   EqualityComparer<Action>.Default.Equals(_redoCallback, change._redoCallback) &&
                   EqualityComparer<Action>.Default.Equals(_undoCallback, change._undoCallback);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin, _redoCallback, _undoCallback);
        } 

        public override string ToString()
        {
            return $"[Callback] {Origin}";
        }
    }
}
