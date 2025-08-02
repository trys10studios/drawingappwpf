using DrawingAppWPF.Commands;
using DrawingAppWPF.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Ink;
using System.Windows.Input;

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

		private string _currentBrushType = "Pencil";
		public string CurrentBrushType
		{
			get => _currentBrushType;
			set
			{
				if (_currentBrushType != value)
				{
					_currentBrushType = value;
					OnPropertyChanged();
				}
			}
		}

		private DrawingAttributes _brushSettingsCurrent = new DrawingAttributes();
		public DrawingAttributes BrushSettingsCurrent
		{
			get => _brushSettingsCurrent;
			set
			{
				if (_brushSettingsCurrent != value)
				{
					_brushSettingsCurrent = value;
					OnPropertyChanged();
				}
			}
		}

		public RelayCommand<string> SetBrushCommand { get; }

		public event EventHandler<DrawingAttributes>? BrushSettingsUpdated;

		// Undo/Redo command properties
		public ICommand UndoCommand { get; }
		public ICommand RedoCommand { get; }

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

		public MainViewModel()
		{
			// Set initial brush type
			CurrentBrushType = "Pencil";

			// Initialize BrushSettings color to match default brush color
			BrushSettings.BrushColorAsColor = BrushSettings.GetDrawingAttributes(CurrentBrushType).Color;

			// Initialize current drawing attributes for the default brush type
			BrushSettingsCurrent = BrushSettings.GetDrawingAttributes(CurrentBrushType);

			// SetBrushCommand uses generic RelayCommand<string>
			SetBrushCommand = new RelayCommand<string>(brushType =>
			{
				CurrentBrushType = brushType;
				BrushSettingsCurrent = BrushSettings.GetDrawingAttributes(brushType);

				// Update the BrushSettings color to the new brush color
				BrushSettings.BrushColorAsColor = BrushSettingsCurrent.Color;
				BrushSettingsUpdated?.Invoke(this, BrushSettingsCurrent);
			});

			UndoCommand = new RelayCommand(
				() => UndoRequested?.Invoke(),
				() => CanUndo);

			RedoCommand = new RelayCommand(
				() => RedoRequested?.Invoke(),
				() => CanRedo);
		}
	}
}
