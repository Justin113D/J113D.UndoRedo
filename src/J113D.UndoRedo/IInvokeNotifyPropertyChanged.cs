using System.ComponentModel;

namespace J113D.UndoRedo
{
    public interface IInvokeNotifyPropertyChanged : INotifyPropertyChanged
    {
        public void InvokePropertyChanged(string propertyName);
    }
}
