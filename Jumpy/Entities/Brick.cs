namespace Jumpy.Entities
{
    public class Brick : Actor
    {
        public int[,] LogicPosition { get; set; }

        public Brick(int[,] logicPosition)
        {
            LogicPosition = logicPosition;
        }
    }
}
