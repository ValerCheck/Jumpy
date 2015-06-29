using Jumpy.Model;
using ViewModel;

namespace Jumpy.ViewModel
{
    public class BrickViewModel : ViewModelBase, IGameElement
    {
        private Brick _brick;
        private double _screenX;
        private double _screenY;
        private double _screenWidth;
        private double _screenHeight;

        public BrickViewModel(){}

        public Brick Brick
        {
            get { return _brick; }
            set
            {
                _brick = value;
                RaisePropertyChanged("Brick");
            }
        }

        public BrickViewModel(double size,int x, int y)
        {
            Brick = new Brick(new ModelPoint { X = x, Y = y });
            Width = size;
            Height = size;
            X = size*x;
            Y = size*y;
            PrevX = X;
            PrevY = Y;
        }

        public double X
        {
            get { return _screenX; }
            set
            {
                _screenX = value;
                RaisePropertyChanged("X");
            }
        }

        public double Y
        {
            get { return _screenY; }
            set
            {
                _screenY = value;
                RaisePropertyChanged("Y");
            }
        }

        public double PrevX { get; set; }
        public double PrevY { get; set; }

        public double Width
        {
            get { return _screenWidth; }
            set
            {
                _screenWidth = value;
                RaisePropertyChanged("Width");
            }
        }

        public double Height
        {
            get { return _screenHeight; }
            set
            {
                _screenHeight = value;
                RaisePropertyChanged("Height");
            }
        }
    }
}
