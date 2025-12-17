namespace GameCore.Controllers.MazeBuilder
{
    public class MazeCell
    {
        public int X { get; }
        public int Y { get; }

        public MazeCellType Type { get; private set; }

        public bool IsWalkable => Type != MazeCellType.Wall;

        public MazeCell(int x, int y, MazeCellType type)
        {
            X = x;
            Y = y;
            Type = type;
        }

        public void SetType(MazeCellType type)
        {
            Type = type;
        }
    }
}