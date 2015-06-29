using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Jumpy.Entities;
using Jumpy.Model;
using Newtonsoft.Json;
using ViewModel;
using System;

namespace Jumpy.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        #region Constants

        private const double VelocityHorizontal = 5;
        private const double VelocityJump = 15;
        private const double Gravity = 0.7;
        private const double MaxFallSpeed = 15;

        #endregion

        private CompositeCollection _actors = new CompositeCollection();
        
        private PlayerViewModel _player;
        private double _x;
        private double _y;
        private bool _isJumping;
        private double _gravity = 2;
        private double _velocity = 20;

        public double ViewWidth { get; set; }
        public double ViewHeight { get; set; }
        public double ViewSize { get; set; }
        public double PhysicalZeroPointX { get; set; }
        public double PhysicalZeroPointY { get; set; }
        private Directions _direction;
        private double _verticalSpeed;
        private double _horizontalSpeed;
        private Level _currentLevel;

        public void LoadLevel()
        {
            _currentLevel = JsonConvert.DeserializeObject<Level>(File.ReadAllText(@"./Jumpy/levelmaps.json"));
        }

        public GameViewModel()
        {
            LoadLevel();
            RenderLevel();
            CompositionTarget.Rendering += GameLoop;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            MovePlayer();
            HandleKeyboard();
        }

        private void HandleKeyboard()
        {
            var leftK = Keyboard.IsKeyDown(Key.Left);
            var rightK = Keyboard.IsKeyDown(Key.Right);
            var upK = Keyboard.IsKeyDown(Key.Up);
            var downK = Keyboard.IsKeyDown(Key.Down);
            if (!leftK & !rightK & !upK & !downK) _direction = (int)Directions.Default;
        }

        public void RenderLevel()
        {
            var elementSize = 32;
            
            for (var y = 0; y < _currentLevel.Height; y++)
            {
                for (var x = 0; x < _currentLevel.Width; x++)
                {
                    if (_currentLevel.Map[y, x] != (int)EntityType.Brick) continue;
                    var brickVm = new BrickViewModel(elementSize, x, y);
                    Actors.Add(brickVm);
                }
            }

            var playerVm = new PlayerViewModel(elementSize,elementSize, _currentLevel.PlayerPosition[0],
                _currentLevel.PlayerPosition[1]);

            Player = playerVm;
            Actors.Add(playerVm);
        }

        public CompositeCollection Actors
        {
            get { return _actors; }
            set
            {
                _actors = value;
                RaisePropertyChanged("Actors");
            }
        }

        public PlayerViewModel Player
        {
            get { return _player; }
            set
            {
                _player = value;
                RaisePropertyChanged("Player");
            }
        }

        private void MovePlayer()
        {
            Player.PrevX += _horizontalSpeed;
            Player.PrevY += _verticalSpeed;

            if (_direction == Directions.Left)
            {
                if (Player.PrevX > 0 && !_isJumping) _horizontalSpeed = -5;
                if (_isJumping) _horizontalSpeed = -1;
            }
            if (_direction == Directions.Right)
            {
                if (Player.PrevX < _currentLevel.Width * 32 && !_isJumping) _horizontalSpeed = 5;
                if (_isJumping) _horizontalSpeed = 1;
            }
            if (_direction == Directions.Up)
            {
                if (!_isJumping) Jump();
            }
            if (_direction == Directions.Default)
            {
                if (!_isJumping)
                {
                    _horizontalSpeed = 0;
                    _verticalSpeed = 0;
                }
                if (_isJumping && Math.Abs(_horizontalSpeed) > 0) _horizontalSpeed /= 1.1;
            }

            if (_isJumping) Fall();

            Player.X = Player.PrevX;
            Player.Y = Player.PrevY; 
        }

        private void Jump()
        {
            if (_isJumping || !_verticalSpeed.Equals(0)) return;
            _isJumping = true;
            _verticalSpeed = -10;
        }

        private void Fall()
        {
            if (_verticalSpeed < MaxFallSpeed) _verticalSpeed += Gravity;
            else if (_verticalSpeed > MaxFallSpeed) _verticalSpeed = MaxFallSpeed;
            
            foreach (var actor in _actors)
            {
                if (actor.GetType() != typeof(BrickViewModel)) continue;
                if (CheckCollision(Player, (IGameElement)actor) && (_verticalSpeed > 0) && Player.PrevY < ((IGameElement) actor).Y)
                {
                    Player.PrevY = ((IGameElement) actor).Y - Player.Height;
                    //Player.PrevY -= (Player.PrevY + Player.Height) - ((IGameElement) actor).Y;
                    _isJumping = false;
                    _verticalSpeed = 0;
                }
            }
        }
        
        private bool CheckBorders(int x, int y)
        {
            var leftBorder = (x < 0);
            var rightBorder = (x >= _currentLevel.Width);
            var topBorder = (y < 0);
            var bottomBorder = (y >= _currentLevel.Height);
            return ((leftBorder || rightBorder) || (topBorder || bottomBorder));
        }

        private bool CheckCollision(IGameElement obj1, IGameElement obj2)
        {
            return CheckVerticalCollision(obj1, obj2) && CheckHorizontalCollision(obj1, obj2);

        }

        private bool CheckVerticalCollision(IGameElement obj1, IGameElement obj2)
        {
            return obj1.PrevY < obj2.PrevY + obj2.Height && obj1.PrevY + obj1.Height > obj2.PrevY;
        }

        private bool CheckHorizontalCollision(IGameElement obj1, IGameElement obj2)
        {
            return obj1.PrevX < obj2.PrevX + obj2.Width && obj1.PrevX + obj1.Width > obj2.PrevX;
        }

        private bool IsSpaceUnder()
        {
            //var underActors = GetActorsUnderPlayer();
            //return underActors[1] == (int)EntityType.Space;
            return true;
        }

        private bool IsSolidObjectUnder()
        {
            //var underActors = GetActorsUnderPlayer();
            //return underActors[1] == (int)EntityType.Brick;
            return false;
        }

        #region Commands Actions

        private void ChangeDirectionLeftAction()
        {
            _direction = Keyboard.IsKeyDown(Key.Left) ? Directions.Left : Directions.Default;
        }

        private void ChangeDirectionRightAction()
        {
            _direction = Keyboard.IsKeyDown(Key.Right) ? Directions.Right : Directions.Default;
        }

        private void JumpAction()
        {
            if (!_isJumping) _direction = Directions.Up;
        }

        private void ChangeDirectionDownAction()
        {
            if (Player.Y > 0) Player.Y -= (_velocity + 50);
        }

        #endregion

        #region Commands

        public ICommand MoveLeft
        {
            get { return new RelayCommand(p => ChangeDirectionLeftAction()); }
        }

        public ICommand MoveRight
        {
            get { return new RelayCommand(p => ChangeDirectionRightAction()); }
        }

        public ICommand MoveUp
        {
            get { return new RelayCommand(p => JumpAction()); }
        }

        public ICommand MoveDown
        {
            get { return new RelayCommand(p => ChangeDirectionDownAction()); }
        }

        #endregion
    }
}
