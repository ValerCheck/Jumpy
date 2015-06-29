using Jumpy.Entities;

namespace Jumpy.Model
{
    public class Brick
    {
        public ModelPoint Position { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        
        public double Width { get; set; }
        public double Height { get; set; }
        
        public bool IsSolid { get; private set; }

        public Brick(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Brick(ModelPoint logicPosition)
        {
            IsSolid = true;
            Position = logicPosition;
        }
    }
}
