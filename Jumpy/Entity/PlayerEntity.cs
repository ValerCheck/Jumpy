using Jumpy.Entities;

namespace Jumpy.Entity
{
    public class PlayerEntity
    {
        private Player _model;
        private double _screenX;
        private double _screenY;

        public PlayerEntity(double elementSize, int logicX, int logicY)
        {
            _model = new Player(new ModelPoint{ X = logicX, Y =logicY});
            ScreenX = elementSize*logicX;
            ScreenY = elementSize*logicY;
        }
        
        public Player Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public double ScreenX
        {
            get { return _screenX; }
            set { _screenX = value; }
        }

        public double ScreenY
        {
            get { return _screenY; }
            set { _screenY = value; }
        }
    }
}
