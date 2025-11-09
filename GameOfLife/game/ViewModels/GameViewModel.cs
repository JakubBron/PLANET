using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using game.Commands;
using game.Models;

namespace game.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private readonly Board _board;
        private GameRules _rules;
        private static readonly GameConfig _cfg = new GameConfig();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public GameViewModel()
        {
            _rules = _cfg.Rules;
            _board = new Board();
            _board.Randomize(_cfg.BoardWidth, _cfg.BoardHeight, _cfg.Density);

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_cfg.SimulationSpeedMs) };
            _timer.Tick += (_, _) => Step();

            StartCommand = new RelayCommand(_ => Start());
            StopCommand = new RelayCommand(_ => Stop());
            StepCommand = new RelayCommand(_ => Step());
            ClearCommand = new RelayCommand(_ => Clear());
            RandomizeCommand = new RelayCommand(_ => Randomize());
            PlacePatternCommand = new RelayCommand(_ => PlacePattern());
        }

        public Board Board => _board;

        private double _zoom = _cfg.Zoom;
        public double Zoom
        {
            get => _zoom;
            set { _zoom = value; OnPropertyChanged(); }
        }

        
        public string Rules
        {
            get => _cfg.Rules.ToString();
            set
            {
                try
                {
                    _rules = GameRules.FromString(value);
                    OnPropertyChanged();
                }
                catch { }
            }
        }

        public int CellSize
        {
            get => _cfg.CellSize;
            set
            {
                if (value > 0)
                {
                    OnPropertyChanged();
                }
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand StepCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand RandomizeCommand { get; }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
        public void Step() => _board.Step(_rules);
        public void Clear() => _board.Clear();
        public void Randomize() => _board.Randomize(_cfg.BoardWidth, _cfg.BoardHeight, _cfg.Density);

        // TODO: refactor placement of patterns

        private int _placementX = 0;
        public int PlacementX
        {
            get => _placementX;
            set { _placementX = value; OnPropertyChanged(); }
        }

        private int _placementY = 0;
        public int PlacementY
        {
            get => _placementY;
            set { _placementY = value; OnPropertyChanged(); }
        }

        // Selected preset structure
        private PresetFancyStructures _selectedPreset;
        public PresetFancyStructures SelectedPreset
        {
            get => _selectedPreset;
            set { _selectedPreset = value; OnPropertyChanged(); }
        }

        // List of all presets
        public List<PresetFancyStructures> Presets { get; } = PresetFancyStructures.GetAllPatterns().Values.ToList();

        // Command to place the pattern on the board
        public ICommand PlacePatternCommand { get; }

        private void PlacePattern()
        {
            if (Board == null || SelectedPreset == null) return;

            foreach (var point in SelectedPreset.Points)
            {
                int x = (int)point.X + PlacementX;
                int y = (int)point.Y + PlacementY;

                if (x >= 0 && x < Board.Width && y >= 0 && y < Board.Height)
                {
                    Board.SetAlive(x, y);
                }
            }

            // Notify view that board changed
            OnPropertyChanged(nameof(Board));
        }
    }
}
