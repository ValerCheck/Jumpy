using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Jumpy.Entities;
using Jumpy.Entity;
using Newtonsoft.Json;
using ViewModel;
using System.Collections.Generic;
using System;

namespace Jumpy.ViewModel
{
    public enum Directions
    {
        Left = 1,
        Up = 2,
        Right = 3,
        Down = 4,
        Default = 0
    }

    public class GameViewModel : ViewModelBase
    {
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
        private int _direction;
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
            HandleKeyboard();
            MovePlayer();
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
                    if (_currentLevel.Map[y, x] != (int)MainWindow.EntityType.Brick) continue;
                    var brickVm = new BrickViewModel(elementSize, x, y);
                    Actors.Add(brickVm);
                }
            }

            var playerVm = new PlayerViewModel(elementSize, _currentLevel.PlayerPosition[0],
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
            if (_direction == (int)Directions.Left)
            {
                if (Player.ScreenX > 0 && !_isJumping) _horizontalSpeed = -1;
                if (_isJumping) _horizontalSpeed = -1;
            }
            if (_direction == (int)Directions.Right)
            {
                if (PhysicalX < _currentLevel.Width * 32 && !_isJumping) _horizontalSpeed = 3;
                if (_isJumping) _horizontalSpeed = 10;
            }
            if (_direction == (int)Directions.Up)
            {
                //if (CanJump()) Jump();
            }
            if (_direction == (int)Directions.Default)
            {
                if (!_isJumping)
                {
                    _horizontalSpeed = 0;
                    _verticalSpeed = 0;
                }
                if (_isJumping && Math.Abs(_horizontalSpeed) > 0) _horizontalSpeed /= 1.1;
            }

            //if (_isJumping) Fall();

            Player.ScreenX += _horizontalSpeed;
            //PhysicalY += _verticalSpeed;
        }

        public double PhysicalX
        {
            get { return _x; }
            set
            {
                _x = value;
                //LogicX = Convert.ToInt16(Math.Round( (PhysicalX - PhysicalZeroPointX) / ViewSize));
                RaisePropertyChanged("PhysicalX");
            }
        }

        public double PhysicalY
        {
            get { return _y; }
            set
            {
                _y = value;
                //LogicY = Convert.ToInt16(Math.Round( (PhysicalY - PhysicalZeroPointY) / ViewSize));
                RaisePropertyChanged("PhysicalY");
            }
        }

        public int LogicX
        {
            get
            {
                return Player == null ? 0 : Player.Player.X;
            }
            set
            {
                Player.Player.X = value;
                RaisePropertyChanged("LogicX");
            }
        }

        public int LogicY
        {
            get { return Player == null ? 0 : Player.Player.Y; }
            set
            {
                Player.Player.Y = value;
                RaisePropertyChanged("LogicY");
            }
        }

        private void MoveLeftAction()
        {
            //if (Player.Player.X > 0)
              //  PhysicalX -= _velocity;
            
            if (Keyboard.IsKeyDown(Key.Left))
            {
                _direction = Keyboard.IsKeyDown(Key.Right) ? (int)Directions.Default : (int)Directions.Left;
            }
        }

        private void MoveRightAction()
        {
            //if (PhysicalX < 1000)
            //    PhysicalX += _velocity;
            if (Keyboard.IsKeyDown(Key.Right))
            {
                _direction = Keyboard.IsKeyDown(Key.Left) ? (int)Directions.Default : (int)Directions.Right;
            }
            //_direction = (int) Directions.Default;
        }

        private void JumpAction()
        {
            //_isJumping = true;
            //while (_isJumping)
            //{
            //    if (_y + _velocity > 0)
            //    {
            //        PhysicalY += _velocity;
            //        _velocity -= _gravity;
            //    }
            //    else
            //    {
            //        PhysicalY = 0;
            //        _isJumping = false;
            //    }
            //}
            
            //_velocity = 20;
        }

        private void MoveDownAction()
        {
            if (PhysicalY > 0)
                PhysicalY -= _velocity;
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
