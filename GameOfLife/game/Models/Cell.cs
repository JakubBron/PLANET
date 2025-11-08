namespace game.Models
{
    public struct Cell
    {
        public int X { get; }
        public int Y { get; }
        public bool IsAlive { get; set; }

        public Cell(int x, int y, bool alive = false)
        {
            X = x;
            Y = y;
            IsAlive = alive;
        }
    }
}