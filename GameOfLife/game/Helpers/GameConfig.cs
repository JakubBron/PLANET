using System.Formats.Asn1;
using System.IO;
using System.Text.Json;
using game.Models;

namespace game.Helpers
{
    public sealed class GameConfig
    {
        private static GameConfig _instance;
        public static GameConfig Instance => _instance ??= Load();

        public int BoardWidth { get; set; } = 200;
        public int BoardHeight { get; set; } = 200;
        public double Density { get; set; } = 0.25;
        public int CellSize { get; set; } = 6;
        public double SimulationSpeedMs { get; set; } = 100;
        public GameRules Rules { get; set; } = GameRules.FromString("B2/S23");

        private static string ConfigPath => Path.Combine(AppContext.BaseDirectory, "config.json");

        private static GameConfig Load()
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<GameConfig>(json) ?? new GameConfig();
            }
            return new GameConfig();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }
}