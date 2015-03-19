namespace Jumpy.Entities
{
    public class Player : IActor
    {
        public int[] LogicPosition { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Player(int[] logicPosition)
        {
            LogicPosition = logicPosition;
            X = logicPosition[0];
            Y = logicPosition[1];
        }
    }
}
