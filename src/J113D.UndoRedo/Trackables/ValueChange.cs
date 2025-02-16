using System;
using System.Collections.Generic;

namespace J113D.UndoRedo.Trackables
{
    internal readonly struct ValueChange<T> : ITrackable
    {
        public string? Origin { get; }

        private readonly Action<T> _changeCallback;
        private readonly T _oldValue;
        private readonly T _newValue;

        public ValueChange(string? origin, Action<T> changeCallback, T oldValue, T newValue)
        {
            Origin = origin;
            _changeCallback = changeCallback;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public readonly void Undo()
        {
            _changeCallback.Invoke(_oldValue);
        }

        public readonly void Redo()
        {
            _changeCallback.Invoke(_newValue);
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueChange<T> change &&
                   Origin == change.Origin &&
                   EqualityComparer<Action<T>>.Default.Equals(_changeCallback, change._changeCallback) &&
                   EqualityComparer<T>.Default.Equals(_oldValue, change._oldValue) &&
                   EqualityComparer<T>.Default.Equals(_newValue, change._newValue);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin, _changeCallback, _oldValue, _newValue);
        }

        public override string ToString()
        {
            return $"[Value] {Origin} - {_oldValue} -> {_newValue}";
        }
    }
}
