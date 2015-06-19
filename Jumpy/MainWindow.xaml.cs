using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jumpy
{
    public partial class MainWindow
    {

        #region Constants

        private const double VelocityHorizontal = 5;
        private const double VelocityJump = 15;
        private const double GravityAcceleration = 0.7;
        private const double MaxFallSpeed = 15;
        
        #endregion

        

        public enum EntityType
        {
            Space = 0,
            Brick = 1,
        }

        private int _direction;
        private double _verticalSpeed;
        private double _horizontalSpeed;
        
        private bool _isJumping;

        private Image _player;

        private Canvas _root;

        public Canvas RootCanvas
        {
            get { return _root ?? (_root = ((Grid) Content).Children.OfType<Canvas>().First()); }
        }

        public Image Player
        {
            get
            {
                return _player ?? (_player = (from c in RootCanvas.Children.Cast<Object>()
                    where c.GetType() == typeof (Image)
                    select c).Cast<Image>().Select(r => r)
                    .FirstOrDefault(c => (string) c.Tag == "player"));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private int?[] GetActorsUnderPlayer()
        {
            //int[] x = {LogicX - 1, LogicX, LogicX + 1};
            //var y = LogicY + 1;
            //var result = new int?[3];

            //result[0] = CheckBorders(x[0], y) ? null : result[0] = _currentLevel.Map[y, x[0]];
            //result[0] = CheckBorders(x[1], y) ? null : result[1] = _currentLevel.Map[y, x[1]];
            //result[0] = CheckBorders(x[2], y) ? null : result[2] = _currentLevel.Map[y, x[2]];

            //return result;
            return null;
        }

        private bool CheckBorders(int x, int y)
        {
            //var leftBorder = (x < 0);
            //var rightBorder = (x >= _currentLevel.Width);
            //var topBorder = (y < 0);
            //var bottomBorder = (y >= _currentLevel.Height);
            //return ((leftBorder || rightBorder) || (topBorder || bottomBorder));
            return true;
        }

        private bool IsSpaceUnder()
        {
            var underActors = GetActorsUnderPlayer();
            return underActors[1] == (int) EntityType.Space;
        }

        private bool IsSolidObjectUnder()
        {
            var underActors = GetActorsUnderPlayer();
            return underActors[1] == (int) EntityType.Brick;
        }

        private bool CanJump()
        {
            if (!_isJumping) return true;
            return false;
        }

        private void Jump()
        {
            if (_isJumping || !_verticalSpeed.Equals(0)) return;
            _isJumping = true;
            _verticalSpeed = -VelocityJump;
        }

        private void Fall()
        {
            if (_verticalSpeed < MaxFallSpeed) _verticalSpeed += GravityAcceleration;
            else if(_verticalSpeed > MaxFallSpeed) _verticalSpeed = MaxFallSpeed;
            if (!(_verticalSpeed > 0) || !IsSolidObjectUnder()) return;
            _isJumping = false;
            _verticalSpeed = 0;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            //if (Keyboard.IsKeyDown(Key.Left))
            //{
            //    _direction = Keyboard.IsKeyDown(Key.Right) ? (int) Directions.Default : (int) Directions.Left;
            //}
            //if (Keyboard.IsKeyDown(Key.Right))
            //{
            //    _direction = Keyboard.IsKeyDown(Key.Left) ? (int)Directions.Default : (int)Directions.Right;
            //}
            //if (e.Key == Key.Up)   _direction = (int) Directions.Up;
            //if (e.Key == Key.Down) _direction = (int) Directions.Down;

            //if (_pressedKeys.Contains(e.Key)) return;
            //_pressedKeys.Add(e.Key);
            //e.Handled = true;
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            //_pressedKeys.Remove(e.Key);
            //e.Handled = true;
            //if (_pressedKeys.Count == 0) _direction = (int) Directions.Default;
            //else
            //{
            //    if (Keyboard.IsKeyDown(Key.Left)) _direction = (int)Directions.Left;
            //    else if (Keyboard.IsKeyDown(Key.Right)) _direction = (int)Directions.Right;    
            //}
        }

        private void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                e.Handled = true;
            }
        }
    }
}