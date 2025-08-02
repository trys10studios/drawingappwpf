using DrawingAppWPF.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DrawingAppWPF.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public BrushSettings BrushSettings { get; set; } = new BrushSettings();

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}