using DrawingAppWPF.Commands;
using DrawingAppWPF.ViewModels;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;

namespace DrawingAppWPF
{
	public partial class MainWindow : Window
	{
		public MainViewModel ViewModel { get; set; }

		private Stack<Stroke> undoStack = new Stack<Stroke>();
		private Stack<Stroke> redoStack = new Stack<Stroke>();

		public MainWindow()
		{
			InitializeComponent();

			ViewModel = new MainViewModel();
			DataContext = ViewModel;

			// Initialize the InkCanvas brush using current settings
			UpdateInkCanvasBrush();

			// Undo/Redo wiring
			ViewModel.UndoRequested += OnUndoRequested;
			ViewModel.RedoRequested += OnRedoRequested;

			DrawingInkCanvas.StrokeCollected += DrawingInkCanvas_StrokeCollected;

			// Listen for brush color or size changes in ViewModel to update InkCanvas
			ViewModel.BrushSettings.PropertyChanged += BrushSettings_PropertyChanged;

			// Listen for brush type changes to update InkCanvas brush
			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		private void BrushSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ViewModel.BrushSettings.BrushColorAsColor))
			{
				// Get the brush attributes for the current brush type (size, tip, base opacity)
				var baseAttributes = ViewModel.BrushSettings.GetDrawingAttributes(ViewModel.CurrentBrushType);

				// Now override just the color's RGB but keep the original alpha (opacity)
				var newColor = ViewModel.BrushSettings.BrushColorAsColor;
				byte originalAlpha = baseAttributes.Color.A;
				var colorWithOpacity = System.Windows.Media.Color.FromArgb(originalAlpha, newColor.R, newColor.G, newColor.B);

				baseAttributes.Color = colorWithOpacity;

				// Apply to InkCanvas
				DrawingInkCanvas.DefaultDrawingAttributes = baseAttributes;
			}
			else if (e.PropertyName == nameof(ViewModel.BrushSettings.BrushSize))
			{
				// Also update size if brush size changes
				var baseAttributes = ViewModel.BrushSettings.GetDrawingAttributes(ViewModel.CurrentBrushType);

				DrawingInkCanvas.DefaultDrawingAttributes.Width = baseAttributes.Width;
				DrawingInkCanvas.DefaultDrawingAttributes.Height = baseAttributes.Height;
			}
		}

		private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ViewModel.CurrentBrushType))
			{
				UpdateInkCanvasBrush();
			}
		}

		private void UpdateInkCanvasBrush()
		{
			if (ViewModel.CurrentBrushType == "Eraser")
			{
				DrawingInkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
			}
			else
			{
				// Get new DrawingAttributes for current brush type, color and size
				DrawingInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
				var da = ViewModel.BrushSettings.GetDrawingAttributes(ViewModel.CurrentBrushType);
				DrawingInkCanvas.DefaultDrawingAttributes = da;
			}
		}

		private void DrawingInkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
		{
			undoStack.Push(e.Stroke);
			redoStack.Clear();
			UpdateCanUndoRedo();
		}

		private void OnUndoRequested()
		{
			if (undoStack.Count > 0)
			{
				Stroke lastStroke = undoStack.Pop();
				DrawingInkCanvas.Strokes.Remove(lastStroke);
				redoStack.Push(lastStroke);
				UpdateCanUndoRedo();
			}
		}

		private void OnRedoRequested()
		{
			if (redoStack.Count > 0)
			{
				Stroke redoStroke = redoStack.Pop();
				DrawingInkCanvas.Strokes.Add(redoStroke);
				undoStack.Push(redoStroke);
				UpdateCanUndoRedo();
			}
		}

		private void UpdateCanUndoRedo()
		{
			ViewModel.CanUndo = undoStack.Count > 0;
			ViewModel.CanRedo = redoStack.Count > 0;
			(ViewModel.UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(ViewModel.RedoCommand as RelayCommand)?.RaiseCanExecuteChanged();
		}
	}
}
