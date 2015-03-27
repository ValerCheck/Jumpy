using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Jumpy.Entities;

namespace Jumpy.Entity
{
    public class PlayerEntity
    {
        private Player _model;
        private FrameworkElement _view;

        public PlayerEntity(double elementSize, int logicX, int logicY)
        {
            _model = new Player(new ModelPoint{ X = logicX, Y =logicY});

            _view = new Ellipse
            {
                Width = elementSize,
                Height = elementSize,
                Fill = Brushes.Green,
            };
        }

        public PlayerEntity(Player model, FrameworkElement view)
        {
            _model = model;
            _view = view;
        }

        public Player Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public FrameworkElement View
        {
            get { return _view; }
            set { _view = value; }
        }
    }
}
