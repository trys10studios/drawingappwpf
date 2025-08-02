using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DrawingAppWPF.Models
{
	public class BrushSettings : INotifyPropertyChanged
	{
		private double _brushSize = 5;
		public double BrushSize
		{
			get => _brushSize;
			set { _brushSize = value; OnPropertyChanged(); }
		}

		private string _brushColor = "#000000";
		public string BrushColor
		{
			get => _brushColor;
			set { _brushColor = value; OnPropertyChanged(); }
		}

		public event PropertyChangedEventHandler ?PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
