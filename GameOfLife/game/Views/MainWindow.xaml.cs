using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using game.Commands;
using Microsoft.Win32;
using game.Models;
using System.Windows.Media;
using System.Windows.Data;
using game.ViewModels;
using System.IO;
using System.Windows.Media.Imaging;

namespace game.Views
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private GameRules _rules;
        private int _newBoardWidth;
        private int _newBoardHeight;
        private readonly GameConfig _cfg = new GameConfig();
        private bool _isSimulationRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            ShapeComboBox.DataContext = BoardCanvas;
            DataContext = BoardCanvas;
            _rules = _cfg.Rules;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(_cfg.SimulationSpeedMs) };
            _timer.Tick += (s, e) => BoardCanvas.Step(_rules);
            SoundCheckbox.Checked += (s, e) => BoardCanvas.PlaySound = true;
            SoundCheckbox.Unchecked += (s, e) => BoardCanvas.PlaySound = false;
            SetSimulationState(false);
        }

        private void SetSimulationState(bool isRunning)
        {
            _isSimulationRunning = isRunning;

            StartButton.IsEnabled = !isRunning;
            StepButton.IsEnabled = !isRunning;
            ClearButton.IsEnabled = !isRunning;
            RandomButton.IsEnabled = !isRunning;
            ResizeButton.IsEnabled = !isRunning;

            CellSizeBox.IsEnabled = !isRunning;
            RulesBox.IsEnabled = !isRunning;
            BoardSizeUserX.IsEnabled = !isRunning;
            BoardSizeUserY.IsEnabled = !isRunning;
            ShapeComboBox.IsEnabled = !isRunning;
            CellColorComboBox.IsEnabled = !isRunning;
            PlacePatternButton.IsEnabled = !isRunning;

            StopButton.IsEnabled = true;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            SetSimulationState(true);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            SetSimulationState(false);
        } 
        private void Step_Click(object sender, RoutedEventArgs e) => BoardCanvas.Step(_rules);

        private void Clear_Click(object sender, RoutedEventArgs e) => BoardCanvas.Clear();
        private void Random_Click(object sender, RoutedEventArgs e) => BoardCanvas.Randomize();
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Text file|*.txt" };
            if (dlg.ShowDialog() == true)
            {
                BoardCanvas.Save(dlg.FileName);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Text file|*.txt" };
            if (dlg.ShowDialog() == true)
            {
                BoardCanvas.Load(dlg.FileName);
            }
        }

        private void CellSizeBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (BoardCanvas == null)
            {
                return;
            }

            if (int.TryParse(CellSizeBox.Text, out int size) && size > 0)
            {
                BoardCanvas.CellSize = size;
            }
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BoardCanvas == null || BoardScrollViewer == null)
            {
                return;
            }

            BoardCanvas.Zoom = e.NewValue;
            BoardCanvas.InvalidateMeasure();
            BoardCanvas.InvalidateVisual();

            BoardScrollViewer.ScrollToHorizontalOffset(0);
            BoardScrollViewer.ScrollToVerticalOffset(0);
        }

        private void RulesBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _rules = GameRules.FromString(RulesBox.Text);
        }

        private void ResizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_newBoardWidth <= 0 || _newBoardHeight <= 0)
            {
                return;
            }

            var newBoard = new Board(_newBoardWidth, _newBoardHeight);
            BoardCanvas.SetBoard(newBoard);

            BoardCanvas.InvalidateVisual();
        }

        private void BoardSizeBox_XValueChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(BoardSizeUserX.Text, out int value) && value > 0)
                _newBoardWidth = value;
        }

        private void BoardSizeBox_YValueChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(BoardSizeUserY.Text, out int value) && value > 0)
                _newBoardHeight = value;
        }

        private void CellColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoardCanvas == null)
            {
                return;
            }

            if (CellColorComboBox.SelectedItem is ComboBoxItem item)
            {
                var colorName = item.Content.ToString();
                var brush = (Brush)new BrushConverter().ConvertFromString(colorName);
                BoardCanvas.SetCellBrush(brush);
            }
        }
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_timer != null)
            {
                _timer.Interval = TimeSpan.FromMilliseconds(SpeedSlider.Value);
            }
        }

        private void PlacePattern_Click(object sender, RoutedEventArgs e)
        {
            if (PresetComboBox.SelectedItem is ComboBoxItem item && int.TryParse(XTextBox.Text, out int x) && int.TryParse(YTextBox.Text, out int y))
            {
                BoardCanvas.PlacePattern(item.Content.ToString(), x, y);
            }
        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var width = (int)this.ActualWidth;
                var height = (int)this.ActualHeight;

                var renderBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);
                renderBitmap.Render(this);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                var dlg = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png",
                    FileName = $"GameOfLife_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                };

                if (dlg.ShowDialog() == true)
                {
                    using (var fs = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error taking screenshot:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
