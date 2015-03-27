namespace Jumpy.Entities
{
    public class Player : IActor
    {
        public ModelPoint LogicPosition { get; set; }

        public int X
        {
            get { return LogicPosition.X; }
            set { LogicPosition.X = value; }
        }

        public int Y
        {
            get { return LogicPosition.Y; } 
            set { LogicPosition.Y = value; }
        }

        public bool IsSolid { get; private set; }

        public Player(ModelPoint logicPosition)
        {
            IsSolid = false;
            LogicPosition = logicPosition;
        }
    }
}
