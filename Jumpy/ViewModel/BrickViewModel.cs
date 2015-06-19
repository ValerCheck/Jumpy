using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jumpy.Entities;
using ViewModel;

namespace Jumpy.ViewModel
{
    public class BrickViewModel : ViewModelBase
    {
        private Brick _brick;
        private double _screenX;
        private double _screenY;

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
            ScreenX = size*x;
            ScreenY = size*y;
        }

        public double ScreenX
        {
            get { return _screenX; }
            set
            {
                _screenX = value;
                RaisePropertyChanged("ScreenX");
            }
        }

        public double ScreenY
        {
            get { return _screenY; }
            set
            {
                _screenY = value;
                RaisePropertyChanged("ScreenY");
            }
        }
    }
}
