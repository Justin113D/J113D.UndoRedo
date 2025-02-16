using System;
using System.Collections.Generic;
using System.Reflection;

namespace J113D.UndoRedo.Trackables
{
    internal readonly struct PropertyChange : ITrackable
    {
        public string? Origin { get; }

        private readonly object _target;
        private readonly PropertyInfo _propertyInfo;
        private readonly object? _oldValue;
        private readonly object? _newValue;

        public PropertyChange(string? origin, object target, string propertyName, object? value)
        {
            Origin = origin;
            _target = target;

            Type? type = _target.GetType();
            while(type != null && type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            {
                type = type.BaseType;
            }

            if(type == null)
            {
                throw new ArgumentException($"Type \"{_target.GetType()}\" does not have a property called \"{propertyName}\" !");
            }

            _propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
            _newValue = value;
            _oldValue = _propertyInfo.GetValue(_target);
        }

        public readonly void Redo()
        {
            _propertyInfo.SetValue(_target, _newValue);
        }

        public readonly void Undo()
        {
            _propertyInfo.SetValue(_target, _oldValue);
        }

        public override bool Equals(object? obj)
        {
            return obj is PropertyChange change &&
                   Origin == change.Origin &&
                   EqualityComparer<object>.Default.Equals(_target, change._target) &&
                   EqualityComparer<PropertyInfo>.Default.Equals(_propertyInfo, change._propertyInfo) &&
                   EqualityComparer<object?>.Default.Equals(_oldValue, change._oldValue) &&
                   EqualityComparer<object?>.Default.Equals(_newValue, change._newValue);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin, _target, _propertyInfo, _oldValue, _newValue);
        }

        public override string ToString()
        {
            return $"[Property] {Origin} - {_target} // {_propertyInfo.Name} ({_oldValue} -> {_newValue})";
        }
    }
}
