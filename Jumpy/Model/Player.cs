namespace Jumpy.Model
{
    public class Player
    {
        public ModelPoint Position { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public bool IsSolid { get; private set; }

        public Player(ModelPoint position)
        {
            IsSolid = false;
            Position = position;
        }
    }
}
