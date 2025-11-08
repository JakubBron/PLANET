using System;
using System.Windows;
using System.Windows.Threading;
using game.Helpers;
using Microsoft.Win32;
using game.Models;

namespace game.Views
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private GameRules _rules;

        public MainWindow()
        {
           
            InitializeComponent();

            _rules = GameRules.FromString("B3/S23");

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += (s, e) => BoardCanvas.Step(_rules);

            BoardCanvas.Randomize();
        }

        private void Start_Click(object sender, RoutedEventArgs e) => _timer.Start();
        private void Stop_Click(object sender, RoutedEventArgs e) => _timer.Stop();
        private void Step_Click(object sender, RoutedEventArgs e) => BoardCanvas.Step(_rules);
        private void Clear_Click(object sender, RoutedEventArgs e) => BoardCanvas.Clear();
        private void Random_Click(object sender, RoutedEventArgs e) => BoardCanvas.Randomize();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Text file|*.txt" };
            if (dlg.ShowDialog() == true)
                BoardCanvas.Save(dlg.FileName);
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Text file|*.txt" };
            if (dlg.ShowDialog() == true)
                BoardCanvas.Load(dlg.FileName);
        }

        private void CellSizeBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (BoardCanvas == null)
            {
                return;
            }
            if (int.TryParse(CellSizeBox.Text, out int size) && size > 0)
                BoardCanvas.CellSize = size;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BoardCanvas == null)
            {
                return;
            }
            BoardCanvas.Zoom = e.NewValue;
        }

        private void RulesBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                _rules = GameRules.FromString(RulesBox.Text);
            }
            catch { /* ignore invalid input */ }
        }
    }
}
