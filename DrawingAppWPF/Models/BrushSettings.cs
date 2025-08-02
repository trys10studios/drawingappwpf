using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Ink;
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
					OnPropertyChanged(nameof(BrushColor));
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

		// Helper methods for color manipulation
		public static Color SetOpacity(Color color, double opacity)
		{
			opacity = Math.Max(0, Math.Min(1, opacity));
			byte alpha = (byte)(opacity * 255);
			return Color.FromArgb(alpha, color.R, color.G, color.B);
		}

		public static Color SetColorRGB(Color originalColor, Color newColor)
		{
			return Color.FromArgb(originalColor.A, newColor.R, newColor.G, newColor.B);
		}

		// Return configured DrawingAttributes based on brush type and current brush color & size
		public DrawingAttributes GetDrawingAttributes(string brushType)
		{
			var da = new DrawingAttributes();
			switch (brushType)
			{
				case "Pencil":
					da.Width = 2;
					da.Height = 2;
					da.Color = SetOpacity(SetColorRGB(BrushColorAsColor, Colors.DarkGray), 0.7);
					da.StylusTip = StylusTip.Rectangle;
					da.IsHighlighter = false;
					da.IgnorePressure = false;
					break;

				case "Pen":
					da.Width = 3;
					da.Height = 3;
					da.Color = SetOpacity(SetColorRGB(BrushColorAsColor, Colors.Black), 1.0);
					da.StylusTip = StylusTip.Ellipse;
					da.IsHighlighter = false;
					da.IgnorePressure = false;
					break;

				case "PaintBrush":
					da.Width = 20;
					da.Height = 10;
					da.Color = SetOpacity(SetColorRGB(BrushColorAsColor, Colors.Red), 0.5);
					da.StylusTip = StylusTip.Ellipse;
					da.IsHighlighter = false;
					da.IgnorePressure = false;
					break;

				case "Marker":
					da.Width = 12;
					da.Height = 12;
					da.Color = SetOpacity(SetColorRGB(BrushColorAsColor, Colors.Black), 0.8);
					da.StylusTip = StylusTip.Rectangle;
					da.IgnorePressure = false;
					da.IsHighlighter = true;
					break;

				case "Eraser":
					da.Width = 10;
					da.Height = 10;
					da.StylusTip = StylusTip.Ellipse;
					da.IgnorePressure = false;
					break;

				default:
					da.Width = BrushSize;
					da.Height = BrushSize;
					da.Color = BrushColorAsColor;
					da.StylusTip = StylusTip.Ellipse;
					da.IgnorePressure = false;
					break;
			}

			return da;
		}
	}
}
