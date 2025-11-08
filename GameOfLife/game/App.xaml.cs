using System.Configuration;
using System.Data;
using System.Windows;
using game.Commands;
using game.Models;

namespace game;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var cfg = new GameConfig();
    }
}

