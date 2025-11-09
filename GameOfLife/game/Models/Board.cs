using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using game.Commands;

namespace game.Models
{
    public class Board: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly GameConfig _cfg = new GameConfig();
        private HashSet<(int x, int y)> _alive = new HashSet<(int x, int y)>();
        public IEnumerable<Cell> Cells => _alive.Select(a => new Cell(a.x, a.y));
        public int Width { get; private set; }
        public int Height { get; private set; }
        public double Density { get; private set; }

        private int _generation = 0;
        private int _born = 0;
        private int _died = 0;
        public int Generation
        {
            get => _generation;
            private set { _generation = value; OnPropertyChanged(nameof(Generation)); }
        }

        public int Born
        {
            get => _born;
            private set { _born = value; OnPropertyChanged(nameof(Born)); }
        }

        public int Died
        {
            get => _died;
            private set { _died = value; OnPropertyChanged(nameof(Died)); }
        }


        public Board(int? width = null, int? height = null)
        {
            Width = width ?? _cfg.BoardWidth;
            Height = height ?? _cfg.BoardHeight;
            Randomize(Width, Height);
        }

        public void SetAlive(int x, int y)
        {
            if (0 <= x && x < Width && 0 <= y && y < Height)
            {
                _alive.Add((x, y));
            }
        } 
        public void SetDead(int x, int y) => _alive.Remove((x, y));

        public void Clear()
        {
            _alive.Clear();
            Generation = 0;
            Born = 0;
            Died = 0;
        }

        public void Randomize(int? width = null, int? height = null, double? density = null)
        {
            Width = width ?? _cfg.BoardWidth;
            Height = height ?? _cfg.BoardHeight;
            Density = density ?? _cfg.Density;

            Clear();
            var rnd = new Random();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (rnd.NextDouble() < Density)
                    {
                        SetAlive(x, y);
                    }
                }
            }
        }

        public void Step(GameRules rules)
        {
            var newAlive = new HashSet<(int x, int y)>();
            var neighborCounts = new Dictionary<(int x, int y), int>();

            foreach (var (x, y) in _alive)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        if (!(0 <= x + dx && x + dx < Width && 0 <= y + dy && y + dy < Height))
                        {
                            continue;
                        }

                        var pos = (x + dx, y + dy);

                        if (!neighborCounts.ContainsKey(pos))
                        {
                            neighborCounts[pos] = 0;
                        }
                        neighborCounts[pos]++;
                    }
                }
            }

            Born = 0;
            Died = 0;

            foreach (var kv in neighborCounts)
            {
                bool alive = _alive.Contains(kv.Key);
                int count = kv.Value;
                if (!alive && rules.Birth.Contains(count))
                {
                    newAlive.Add(kv.Key);
                    Born++;
                }
                else if (alive && rules.Survival.Contains(count))
                {
                    newAlive.Add(kv.Key);
                }
                else if (alive)
                {
                    Died++;
                }
            }

            _alive = newAlive;
            Generation++;
            Born = Born;
            Died = Died;
            OnPropertyChanged(nameof(Cells));
        }

        public void Save(string path)
        {
            using var sw = new StreamWriter(path);
            sw.WriteLine($"{Width} {Height}");
            foreach (var c in _alive)
                sw.WriteLine($"{c.x} {c.y}");
        }

        public void Load(string path)
        {
            using var sr = new StreamReader(path);
            var line = sr.ReadLine();
            var parts = line.Split(' ');
            Width = int.Parse(parts[0]);
            Height = int.Parse(parts[1]);
            _alive.Clear();

            while (!sr.EndOfStream)
            {
                var p = sr.ReadLine().Split(' ');
                int x = int.Parse(p[0]);
                int y = int.Parse(p[1]);
                SetAlive(x, y);
            }
        }
    }
}
