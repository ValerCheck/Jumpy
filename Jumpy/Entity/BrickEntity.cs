using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Jumpy.Entities;

namespace Jumpy.Entity
{
    public class BrickEntity
    {
        private Brick _model;
        private FrameworkElement _view;

        public BrickEntity(double elementSize, int logicX, int logicY)
        {
            _view = new Border
            {
                Width = elementSize,
                Height = elementSize,
                Background = Brushes.Brown,
                BorderBrush = Brushes.Brown,
            };
            _model = new Brick(new ModelPoint {X = logicX, Y = logicY} );
        }

        public BrickEntity(Brick model, FrameworkElement view)
        {
            _model = model;
            _view = view;
        }

        public Brick Model
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
