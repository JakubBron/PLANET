using System.Drawing;
using System.Formats.Asn1;
using System.IO;
using System.Text.Json;
using System.Windows.Media;
using game.Models;

namespace game.Commands
{
    public sealed class GameConfig
    {
        public int BoardWidth { get; set; } = 300;
        public int BoardHeight { get; set; } = 300;
        public double Density { get; set; } = 0.25;
        public int CellSize { get; set; } = 10;
        public double SimulationSpeedMs { get; set; } = 100;
        public int Zoom { get; set; } = 1;
        public bool DefaultPlaySoundSetting { get; set; } = true;
        public GameRules Rules { get; set; } = GameRules.FromString("B2/S23");

        public Brush CellBrush = Brushes.LimeGreen;
        public Brush BackgroundBrush = Brushes.Black;
        public CellShape DefaultCellShape = CellShape.Rectangle;

        public GameConfig()
        {

        }
    }
}