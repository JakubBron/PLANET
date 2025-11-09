using System;
using System.Collections.Generic;
using System.Windows;

namespace game.Models
{
    public class PresetFancyStructures
    {
        public PresetFancyStructures(string name, string description, bool[,] pattern)
        {
            Name = name;
            Description = description;
            Pattern = pattern;
            Width = pattern.GetLength(0);
            Height = pattern.GetLength(1);


            Points = new List<Point>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (pattern[x, y]) Points.Add(new Point(x, y));
                }
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool[,] Pattern { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<Point> Points { get; private set; }

        public static Dictionary<string, PresetFancyStructures> GetAllPatterns()
        {
            var patterns = new Dictionary<string, PresetFancyStructures>();

            // Still Lifes
            patterns["Block"] = CreateBlock();
            patterns["Beehive"] = CreateBeehive();
            patterns["Loaf"] = CreateLoaf();

            // Oscillators
            patterns["Blinker"] = CreateBlinker();
            patterns["Beacon"] = CreateBeacon();

            // Spaceships
            patterns["Glider"] = CreateGlider();

            return patterns;
        }

        private static PresetFancyStructures CreateBlock()
        {
            var pattern = new[,]
            {
                { true, true },
                { true, true },
            };
            return new PresetFancyStructures("Block", "Simplest still life (2x2 square)", pattern);
        }

        private static PresetFancyStructures CreateBeehive()
        {
            var pattern = new[,]
            {
                { false, true, false },
                { true, false, true },
            };
            return new PresetFancyStructures("Beehive", "Common still life", pattern);
        }

        private static PresetFancyStructures CreateLoaf()
        {
            var pattern = new[,]
            {
                { false, true, false },
                { true, false, true },
                { false, true, false },
            };
            return new PresetFancyStructures("Loaf", "Still life pattern", pattern);
        }

        private static PresetFancyStructures CreateBlinker()
        {
            var pattern = new[,]
            {
                { true, true, true },
            };
            return new PresetFancyStructures("Blinker", "Period 2 oscillator (simplest)", pattern);
        }

        private static PresetFancyStructures CreateBeacon()
        {
            var pattern = new[,]
            {
                { true, true },
                { false, false },
            };
            return new PresetFancyStructures("Beacon", "Period 2 oscillator", pattern);
        }

        private static PresetFancyStructures CreateGlider()
        {
            var pattern = new[,]
            {
                { false, true, false },
                { false, false, true },
                { true, true, true },
            };
            return new PresetFancyStructures("Glider", "Smallest spaceship (period 4, moves diagonally)", pattern);
        }
    }
}
