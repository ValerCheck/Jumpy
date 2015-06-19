using System.Windows;
using Jumpy.Entities;
using ViewModel;

namespace Jumpy.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private Player _player;
        private double _screenX;
        private double _screenY;

        public PlayerViewModel(){}

        public PlayerViewModel(double size,int x, int y)
        {
            Player = new Player(new ModelPoint { X = x, Y = y });
            ScreenX = size*x;
            ScreenY = size*y;
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
