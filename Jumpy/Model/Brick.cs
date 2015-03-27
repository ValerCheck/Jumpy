namespace Jumpy.Entities
{
    public class Brick : IActor
    {
        public ModelPoint LogicPosition { get; set; }

        public int X
        {
            get
            {
                return LogicPosition.X;
                
            }
            set { LogicPosition.X = value; }
        }

        public int Y { get; set; }
        public bool IsSolid { get; private set; }

        public Brick(ModelPoint logicPosition)
        {
            IsSolid = true;
            LogicPosition = logicPosition;
        }
    }
}
