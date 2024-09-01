using System.ComponentModel;

namespace J113D.UndoRedo.Test
{
    internal class TestContainer : IInvokeNotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public string StringProperty { get; set; }

        public string stringField;

        public TestContainer()
        {
            StringProperty = string.Empty;
            stringField = string.Empty;
        }
    }
}
