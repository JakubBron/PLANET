using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using game.Commands;
using game.Models;

namespace game.Views
{
    public class BoardRenderer : FrameworkElement
    {
        private static GameConfig cfg = new GameConfig();
        private Board _board;
        public Board Board => _board;
        private int _cellSize = cfg.CellSize;
        private bool _isDrawing = false;
        private bool _drawState = true;
        public Brush CellBrush { get; set; } = cfg.CellBrush;

        public Brush BackgroundBrush { get; set; } = cfg.BackgroundBrush;
        public double Zoom { get; set; } = cfg.Zoom;

        private CellShape _shapeOfCellChosenByUser = cfg.DefaultCellShape; 
        public CellShape CellShape  // shape name -> shape type (class) is converted automatically by WPF
        {
            get => _shapeOfCellChosenByUser;
            set
            {
                if (_shapeOfCellChosenByUser != value)
                {
                    _shapeOfCellChosenByUser = value;
                    InvalidateVisual();
                }
            }
        }
        public int CellSize
        {
            get => _cellSize;
            set { _cellSize = Math.Max(1, value); InvalidateVisual(); }
        }
        public void SetBoard(Board newBoard)
        {
            _board = newBoard;
        }

        public BoardRenderer()
        {
            _board = new Board();
            Focusable = true;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
        }

        /// ======================
        ///     Game control
        /// ======================
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

        public void Load(string path)
        {
            _board.Load(path); 
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

        public void PlacePattern(string presetName, int xOffset, int yOffset)
        {
            var preset = PresetFancyStructures.GetAllPatterns();
            bool[,] pattern;
            try
            {
                pattern = preset[presetName].Pattern;
            }
            catch (KeyNotFoundException)
            {
                return;
            }
            
            for (int y = 0; y < pattern.GetLength(1); y++)
            {
                for (int x = 0; x < pattern.GetLength(0); x++)
                {
                    if (pattern[x, y])
                    {
                        _board.SetAlive(x + xOffset, y + yOffset);
                    }
                }
            }
            InvalidateVisual();
        }

        /// =========================
        ///     Screen Rendering
        /// =========================
        public void SetCellBrush(Brush brush)
        {
            CellBrush = brush;
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.PushTransform(new ScaleTransform(Zoom, Zoom));

            // background
            dc.DrawRectangle(BackgroundBrush, null, new Rect(0, 0, _board.Width * _cellSize, _board.Height * _cellSize));

            // cells
            foreach (var cell in _board.Cells)
            {
                var rect = new Rect(cell.X * _cellSize, cell.Y * _cellSize, _cellSize, _cellSize);
                switch (CellShape)
                {
                    case CellShape.Rectangle:
                        dc.DrawRectangle(CellBrush, null, rect);
                        break;
                    case CellShape.Circle:
                        dc.DrawEllipse(CellBrush, null,
                            new Point(rect.X + _cellSize / 2.0, rect.Y + _cellSize / 2.0),
                            _cellSize / 2.0, _cellSize / 2.0);
                        break;
                    case CellShape.Triangle:
                        var p1 = new Point(rect.X + _cellSize / 2.0, rect.Y);
                        var p2 = new Point(rect.X, rect.Y + _cellSize);
                        var p3 = new Point(rect.X + _cellSize, rect.Y + _cellSize);
                        var triangle = new StreamGeometry();
                        using (var ctx = triangle.Open())
                        {
                            ctx.BeginFigure(p1, true, true);
                            ctx.LineTo(p2, true, false);
                            ctx.LineTo(p3, true, false);
                        }
                        dc.DrawGeometry(CellBrush, null, triangle);
                        break;
                }
            }

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
    }
}
