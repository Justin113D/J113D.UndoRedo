using System;
using System.Collections.Generic;
using System.Reflection;

namespace J113D.UndoRedo.Trackables
{
    internal readonly struct FieldChange : ITrackable
    {
        public string? Origin { get; }

        private readonly object _target;
        private readonly FieldInfo _fieldInfo;
        private readonly object? _oldValue;
        private readonly object? _newValue;

        public FieldChange(string? origin, object target, string fieldName, object? value)
        {
            Origin = origin;
            _target = target;

            Type? type = _target.GetType();
            while(type != null && type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            {
                type = type.BaseType;
            }

            if(type == null)
            {
                throw new ArgumentException($"Type \"{_target.GetType()}\" does not have a field called \"{fieldName}\" !");
            }

            _fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
            _newValue = value;
            _oldValue = _fieldInfo.GetValue(_target);
        }

        public readonly void Redo()
        {
            _fieldInfo.SetValue(_target, _newValue);
        }

        public readonly void Undo()
        {
            _fieldInfo.SetValue(_target, _oldValue);
        }

        public override bool Equals(object? obj)
        {
            return obj is FieldChange change &&
                   Origin == change.Origin &&
                   EqualityComparer<object>.Default.Equals(_target, change._target) &&
                   EqualityComparer<FieldInfo>.Default.Equals(_fieldInfo, change._fieldInfo) &&
                   EqualityComparer<object?>.Default.Equals(_oldValue, change._oldValue) &&
                   EqualityComparer<object?>.Default.Equals(_newValue, change._newValue);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin, _target, _fieldInfo, _oldValue, _newValue);
        }

        public override string ToString()
        {
            return $"[Field] {Origin} - {_target} // {_fieldInfo.Name} ({_oldValue} -> {_newValue})";
        }
    }
}
