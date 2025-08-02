using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace DrawingAppWPF.Models
{
	public class BrushSettings : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private Color _brushColor = Colors.Black;		
		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public Color BrushColorAsColor
		{
			get => _brushColor;
			set
			{
				if (_brushColor != value)
				{
					_brushColor = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(BrushColor)); // If you keep the string too
				}
			}
		}

		public string BrushColor
		{
			get => _brushColor.ToString();
			set
			{
				if (ColorConverter.ConvertFromString(value) is Color color && color != _brushColor)
				{
					_brushColor = color;
					OnPropertyChanged(nameof(BrushColor));
					OnPropertyChanged(nameof(BrushColorAsColor));
				}
			}
		}

		private double _brushSize = 3;
		public double BrushSize
		{
			get => _brushSize;
			set
			{
				if (_brushSize != value)
				{
					_brushSize = value;
					OnPropertyChanged();
				}
			}
		}

	}
}
