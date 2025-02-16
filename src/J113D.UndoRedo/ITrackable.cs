namespace J113D.UndoRedo
{
    internal interface ITrackable
    {
        public string? Origin { get; }

        public void Undo();

        public void Redo();
    }
}
