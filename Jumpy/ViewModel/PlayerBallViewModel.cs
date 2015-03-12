using System.Security.RightsManagement;
using System.Threading;
using System.Windows.Input;
using ViewModel;

namespace Jumpy.ViewModel
{
    public class PlayerBallViewModel:ViewModelBase
    {
        private double _x = 0;
        private double _y = 400;
        private bool isJumping = false;
        private double _gravity = 2;
        private double _velocity = 20;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                RaisePropertyChanged("X");
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                RaisePropertyChanged("Y");
            }
        }

        private void MoveLeftAction()
        {
            if (X > 0)
                X -= _velocity;
        }

        private void MoveRightAction()
        {
            if (X < 1000)
                X += _velocity;
        }

        private void JumpAction()
        {
            isJumping = true;
            while (isJumping)
            {
                for(var i=0;i<100;i++)
                    continue;
                if (_y + _velocity > 0)
                {
                    Y += _velocity;
                    _velocity -= _gravity;
                }
                else
                {
                    Y = 0;
                    isJumping = false;
                }
            }
            
            _velocity = 20;
        }

        private void MoveDownAction()
        {
            if (Y > 0)
                Y -= _velocity;
        }

        public ICommand MoveLeft
        {
            get{return new RelayCommand(p=>MoveLeftAction());}
        }

        public ICommand MoveRight
        {
            get{return new RelayCommand(p=>MoveRightAction());}
        }

        public ICommand MoveUp
        {
            get{return new RelayCommand(p=>JumpAction());}
        }

        public ICommand MoveDown
        {
            get{return new RelayCommand(p=>MoveDownAction());}
        }
    }
}
