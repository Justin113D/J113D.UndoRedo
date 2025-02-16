using System.ComponentModel;

namespace J113D.UndoRedo
{
	/// <summary>
	/// Notify property changed interface
	/// </summary>
	public interface IInvokeNotifyPropertyChanged : INotifyPropertyChanged
	{
		/// <summary>
		/// Method that gets invoked when a property gets undo/redo-ed
		/// </summary>
		/// <param name="propertyName">Name of the changed property</param>
		public void InvokePropertyChanged(string propertyName);
	}
}
