using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using game.Commands;
using game.Models;

namespace game.Views
{
    public class BoardRenderer : FrameworkElement
    {
        private GameConfig cfg = new GameConfig();
        private Board _board;
        private int _cellSize = 10;
        private bool _isDrawing = false;
        private bool _drawState = true;

        public Brush CellBrush { get; set; } = Brushes.LimeGreen;
        public Brush BackgroundBrush { get; set; } = Brushes.Black;
        public double Zoom { get; set; } = 1.0;

        public BoardRenderer()
        {
            _board = new Board();
            Focusable = true;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
        }

        public void SetBoard(Board board)
        {
            _board = board;
        }

        public void Step(GameRules rules)
        {
            _board.Step(rules);
            InvalidateVisual();
        }

        public void Clear()
        {
            _board.Clear();
            InvalidateVisual();
        }

        public void Randomize()
        {
            _board.Randomize(_board.Width, _board.Height, cfg.Density);
            InvalidateVisual();
        }

        public void Save(string path) => _board.Save(path);
        public void Load(string path) { _board.Load(path); InvalidateVisual(); }

        public int CellSize
        {
            get => _cellSize;
            set { _cellSize = Math.Max(1, value); InvalidateVisual(); }
        }

        public Board Board => _board;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.PushTransform(new ScaleTransform(Zoom, Zoom));
            dc.DrawRectangle(BackgroundBrush, null, new Rect(0, 0, _board.Width * _cellSize, _board.Height * _cellSize));

            foreach (var cell in _board.Cells)
            {
                dc.DrawRectangle(CellBrush, null, new Rect(cell.X * _cellSize, cell.Y * _cellSize, _cellSize, _cellSize));
            }

            // Statystyki
            var text = $"Generation: {_board.Generation}\nBorn: {_board.Born}\nDied: {_board.Died}\nCells: {_board.Cells.Count()}";
            var ft = new FormattedText(
                text, System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Consolas"), 14, Brushes.Yellow, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            dc.DrawText(ft, new Point(5, 5));

            dc.Pop();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            _drawState = e.ChangedButton == MouseButton.Right;
            HandleMouse(e.GetPosition(this));
            CaptureMouse();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;

            if (e.LeftButton == MouseButtonState.Pressed) _drawState = false;
            else if (e.RightButton == MouseButtonState.Pressed) _drawState = true;
            else return;

            HandleMouse(e.GetPosition(this));
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
            ReleaseMouseCapture();
        }

        private void HandleMouse(Point p)
        {
            int x = (int)(p.X / (_cellSize * Zoom));
            int y = (int)(p.Y / (_cellSize * Zoom));

            if (_drawState) _board.SetAlive(x, y);
            else _board.SetDead(x, y);

            InvalidateVisual();
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(_board.Width * _cellSize * Zoom, _board.Height * _cellSize * Zoom);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(_board.Width * _cellSize * Zoom, _board.Height * _cellSize * Zoom);
        }

    }
}
