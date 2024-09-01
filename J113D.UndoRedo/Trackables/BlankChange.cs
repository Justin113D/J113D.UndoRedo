using System;

namespace J113D.UndoRedo.Trackables
{
    internal readonly struct BlankChange : ITrackable
    {
        public string? Origin { get; }

        public BlankChange(string? origin)
        {
            Origin = origin;
        }

        public readonly void Redo() { }

        public readonly void Undo() { }

        public override bool Equals(object? obj)
        {
            return obj is BlankChange change &&
                   Origin == change.Origin;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin);
        } 

        public override string ToString()
        {
            return $"[Blank] {Origin}";
        }
    }
}
