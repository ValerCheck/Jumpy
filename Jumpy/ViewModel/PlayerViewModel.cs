using System;
using Jumpy.Entities;
using Jumpy.Model;
using ViewModel;

namespace Jumpy.ViewModel
{
    public class PlayerViewModel : ViewModelBase, IGameElement
    {
        private Player _player;
        private double _screenX;
        private double _screenY;
        private double _screenWidth;
        private double _screenHeight;

        public PlayerViewModel(){}

        public PlayerViewModel(double width, double height,int x, int y)
        {
            Player = new Player(new ModelPoint { X = x, Y = y });
            Width = width;
            Height = width;
            X = width*x;
            Y = height*y;
            PrevX = X;
            PrevY = Y;
        }

        public Player Player
        {
            get { return _player; }
            set
            {
                _player = value;
                RaisePropertyChanged("Player");
            }
        }

        public double PrevX { get; set; }
        public double PrevY { get; set; }

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
