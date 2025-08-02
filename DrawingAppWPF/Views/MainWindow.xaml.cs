using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DrawingAppWPF.ViewModels;
using DrawingAppWPF.Models;

namespace DrawingAppWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// Basic Drawing App with Pressure Sensitivity and Mouse Drawing Fallback
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool isDrawing = false;
		private Polyline currentLine = new Polyline();		public MainViewModel ViewModel { get; }

		public MainWindow()
		{
			InitializeComponent();
			ViewModel = new MainViewModel();
			DataContext = ViewModel;
		}

		private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				isDrawing = true;

				currentLine = new Polyline
				{
					Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ViewModel.BrushSettings.BrushColor)),
					StrokeThickness = ViewModel.BrushSettings.BrushSize,
					StrokeStartLineCap = PenLineCap.Round,
					StrokeEndLineCap = PenLineCap.Round
				};

				currentLine.Points.Add(e.GetPosition(DrawingCanvas));
				DrawingCanvas.Children.Add(currentLine);
			}
		}

		private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (isDrawing && currentLine != null)
			{
				currentLine.Points.Add(e.GetPosition(DrawingCanvas));
			}
		}

		private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			isDrawing = false;
		}
	}
}