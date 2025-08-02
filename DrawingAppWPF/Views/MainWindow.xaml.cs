using DrawingAppWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using DrawingAppWPF.Commands;
using System.ComponentModel;

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

			var inkDA = new DrawingAttributes
			{
				Color = ViewModel.BrushSettings.BrushColorAsColor,
				Width = ViewModel.BrushSettings.BrushSize,
				Height = ViewModel.BrushSettings.BrushSize,
				FitToCurve = true,
				IgnorePressure = false
			};

			DrawingInkCanvas.DefaultDrawingAttributes = inkDA;

			// Wire up Undo/Redo events from ViewModel
			ViewModel.UndoRequested += OnUndoRequested;
			ViewModel.RedoRequested += OnRedoRequested;

			// Track new strokes to manage undo stack
			DrawingInkCanvas.StrokeCollected += DrawingInkCanvas_StrokeCollected;

			// Listen for brush color or size changes in ViewModel to update InkCanvas
			ViewModel.BrushSettings.PropertyChanged += BrushSettings_PropertyChanged;
		}
		private void BrushSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ViewModel.BrushSettings.BrushColorAsColor) ||
				e.PropertyName == nameof(ViewModel.BrushSettings.BrushSize))
			{
				// Update the InkCanvas DrawingAttributes accordingly
				DrawingInkCanvas.DefaultDrawingAttributes.Color = ViewModel.BrushSettings.BrushColorAsColor;
				DrawingInkCanvas.DefaultDrawingAttributes.Width = ViewModel.BrushSettings.BrushSize;
				DrawingInkCanvas.DefaultDrawingAttributes.Height = ViewModel.BrushSettings.BrushSize;
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
