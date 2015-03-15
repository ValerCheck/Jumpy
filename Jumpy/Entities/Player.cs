namespace Jumpy.Entities
{
    public class Player : Actor
    {
        public int[,] LogicPosition { get; set; }

        public Player(int[,] logicPosition)
        {
            LogicPosition = logicPosition;
        }
        
    }
}
