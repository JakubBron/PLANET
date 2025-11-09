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
        private static readonly GameConfig cfg = new GameConfig();

        public event PropertyChangedEventHandler PropertyChanged;

        public GameViewModel()
        {
            _rules = cfg.Rules;
            _board = new Board();
            _board.Randomize(cfg.BoardWidth, cfg.BoardHeight, cfg.Density);

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(cfg.SimulationSpeedMs) };
            _timer.Tick += (_, _) => Step();

            StartCommand = new RelayCommand(_ => Start());
            StopCommand = new RelayCommand(_ => Stop());
            StepCommand = new RelayCommand(_ => Step());
            ClearCommand = new RelayCommand(_ => Clear());
            RandomizeCommand = new RelayCommand(_ => Randomize());
        }

        public Board Board => _board;

        private double _zoom = cfg.Zoom;
        public double Zoom
        {
            get => _zoom;
            set { _zoom = value; OnPropertyChanged(); }
        }

        
        public string Rules
        {
            get => cfg.Rules.ToString();
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
            get => cfg.CellSize;
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
        public void Randomize() => _board.Randomize(cfg.BoardWidth, cfg.BoardHeight, cfg.Density);

        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
