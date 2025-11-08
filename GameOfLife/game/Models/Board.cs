using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Automation;
using game.Helpers;

namespace game.Models
{
    public class Board
    {
        private GameConfig cfg = GameConfig.Instance;
        private HashSet<(int x, int y)> _alive = new HashSet<(int x, int y)>();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Generation { get; private set; } = 0;
        public int Born { get; private set; } = 0;
        public int Died { get; private set; } = 0;

        public IEnumerable<Cell> Cells => _alive.Select(a => new Cell(a.x, a.y, true));

        public Board(int? width = null, int? height = null)
        {
            Width = width ?? cfg.BoardWidth;
            Height = height ?? cfg.BoardHeight;
        }

        public void SetAlive(int x, int y)
        {
            if (0 <= x && x <= Width && 0 <= y && y <= Height)
            {
                _alive.Add((x, y));
            }
        } 
        public void SetDead(int x, int y) => _alive.Remove((x, y));
        public bool IsAlive(int x, int y) => _alive.Contains((x, y));

        public void Clear()
        {
            _alive.Clear();
            Generation = 0;
            Born = 0;
            Died = 0;
        }

        public void Randomize(int? width = null, int? height = null, double? density = null)
        {

            Width = width ?? cfg.BoardWidth;
            Height = height ?? cfg.BoardHeight;
            density = density ?? cfg.Density;

            Clear();
            var rnd = new Random();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (rnd.NextDouble() < density)
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
        }

        // Zapis i odczyt do pliku
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
