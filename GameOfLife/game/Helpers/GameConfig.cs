using System.Formats.Asn1;
using System.IO;
using System.Text.Json;
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
        public GameRules Rules { get; set; } = GameRules.FromString("B2/S23");

        public GameConfig()
        {

        }
    }
}