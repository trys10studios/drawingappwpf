using DrawingAppWPF.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DrawingAppWPF.Commands;

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

		// Undo/Redo command properties
		public ICommand? UndoCommand { get; private set; }
		public ICommand? RedoCommand { get; private set; }

		// Undo/Redo state properties (for button enable/disable)
		private bool _canUndo;
		public bool CanUndo
		{
			get => _canUndo;
			set
			{
				if (_canUndo != value)
				{
					_canUndo = value;
					OnPropertyChanged();
				}
			}
		}

		private bool _canRedo;
		public bool CanRedo
		{
			get => _canRedo;
			set
			{
				if (_canRedo != value)
				{
					_canRedo = value;
					OnPropertyChanged();
				}
			}
		}

		// Events the View will subscribe to
		public event Action? UndoRequested;
		public event Action? RedoRequested;

		// Constructor
		public MainViewModel()
		{
			UndoCommand = new RelayCommand(
				execute: () => UndoRequested?.Invoke(),
				canExecute: () => CanUndo);

			RedoCommand = new RelayCommand(
				execute: () => RedoRequested?.Invoke(),
				canExecute: () => CanRedo);
		}
	}
}
