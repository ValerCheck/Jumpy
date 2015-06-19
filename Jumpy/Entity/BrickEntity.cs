using System.Windows;
using Jumpy.Entities;

namespace Jumpy.Entity
{
    public class BrickEntity
    {
        private Brick _model;
        private double _screenX;
        private double _screenY;

        public BrickEntity(double elementSize, int logicX, int logicY)
        {
            _model = new Brick(new ModelPoint {X = logicX, Y = logicY} );
            _screenX = logicX*elementSize;
            _screenY = logicY*elementSize;
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

        public Brick Model
        {
            get { return _model; }
            set { _model = value; }
        }
    }
}
