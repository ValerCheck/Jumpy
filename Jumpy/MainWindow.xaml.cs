using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Jumpy.Entities;
using Jumpy.Entity;
using Jumpy.ViewModel;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Data;

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

        private enum Directions
        {
            Left = 1,
            Up = 2,
            Right = 3,
            Down = 4,
            Default = 0
        }

        private enum EntityType
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

        private bool CheckCollision()
        {
            var rect1 = CreateElementRect(Player);
            var nearest = findNearest().ToList();
            if (!nearest.Any()) return true;
            foreach (var near in nearest)
            {
                var rect2 = CreateElementRect(near);
                rect1.Intersect(rect2);
            }
            return rect1.IsEmpty;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly List<Key> _pressedKeys = new List<Key>();
        private Level _currentLevel;

        public void LoadLevel()
        {
            _currentLevel = JsonConvert.DeserializeObject<Level>(File.ReadAllText(@"levelmaps.json"));
        }

        private void GameLoop(object sender, EventArgs e)
        {
            MainLoop();
        }

        private void AddElementToCanvas(FrameworkElement element, double left, double top)
        {
            RootCanvas.Children.Add(element);
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
        }

        public void RenderLevel()
        {
            var viewModel = (GameViewModel) DataContext;
            var elementSize = RootCanvas.ActualHeight / _currentLevel.Height;
            viewModel.ViewSize = elementSize;

            for (var x = 0; x < _currentLevel.Width; x++)
            {
                for (var y = 0; y < _currentLevel.Height; y++)
                {
                    if (_currentLevel.Map[y, x] != (int) EntityType.Brick) continue;
                    var brickEntity = new BrickEntity(elementSize, x, y);
                    AddElementToCanvas(brickEntity.View, elementSize * x, elementSize * y);
                    viewModel.Actors.Add(brickEntity.Model);
                }
            }

            var playerEntity = new PlayerEntity(elementSize, _currentLevel.PlayerPosition[0],
                _currentLevel.PlayerPosition[1]);

            RootCanvas.Children.Add(playerEntity.View);
            viewModel.Player = playerEntity.Model;

            PhysicalX = elementSize*_currentLevel.PlayerPosition[0];
            PhysicalY = elementSize*_currentLevel.PlayerPosition[1];

            playerEntity.View.SetBinding(
                dp : Canvas.LeftProperty, 
                binding : new Binding("PhysicalX") { Source = viewModel });
            playerEntity.View.SetBinding(
                dp : Canvas.TopProperty, 
                binding : new Binding("PhysicalY") { Source = viewModel });
        }

        private IEnumerable<FrameworkElement> findNearest()
        {
            var allObstacles = (from child in RootCanvas.Children.Cast<Object>()
                where child is Rectangle
                select child).Cast<FrameworkElement>();
            var nearest = new List<FrameworkElement>();
            var playerRect = CreateElementRect(Player);
            foreach (var wall in allObstacles)
            {
                var wallRect = CreateElementRect(wall);
                if(playerRect.IntersectsWith(wallRect))
                    nearest.Add(wall);
            }
            return nearest;
        }

        private static Rect CreateElementRect(FrameworkElement element)
        {
            var rect = new Rect
            {
                X = Canvas.GetLeft(element),
                Y = Canvas.GetBottom(element),
                Width = element.Width,
                Height = element.Height
            };
            return rect;
        }

        private void MainLoop()
        {
            MovePlayer();
        }

        private int?[] GetActorsUnderPlayer()
        {
            int[] x = {LogicX - 1, LogicX, LogicX + 1};
            var y = LogicY + 1;
            var result = new int?[3];

            result[0] = CheckBorders(x[0], y) ? null : result[0] = _currentLevel.Map[y, x[0]];
            result[0] = CheckBorders(x[1], y) ? null : result[1] = _currentLevel.Map[y, x[1]];
            result[0] = CheckBorders(x[2], y) ? null : result[2] = _currentLevel.Map[y, x[2]];

            return result;
        }

        private bool CheckBorders(int x, int y)
        {
            var leftBorder = (x < 0);
            var rightBorder = (x >= _currentLevel.Width);
            var topBorder = (y < 0);
            var bottomBorder = (y >= _currentLevel.Height);
            return ((leftBorder || rightBorder) || (topBorder || bottomBorder));
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

        private void MovePlayer()
        {
            if (_direction == (int) Directions.Left)
            {
                if (PhysicalX > 0 && !_isJumping) _horizontalSpeed = -VelocityHorizontal;
                if (_isJumping) _horizontalSpeed = -VelocityHorizontal;
            }
            if (_direction == (int) Directions.Right)
            {
                if (PhysicalX < RootCanvas.ActualWidth && !_isJumping) _horizontalSpeed = VelocityHorizontal;
                if (_isJumping) _horizontalSpeed = VelocityHorizontal;
            }
            if (_direction == (int) Directions.Up)
            {
                if (CanJump()) Jump();
            }
            if (_direction == (int) Directions.Default)
            {
                if (!_isJumping)
                {
                    _horizontalSpeed = 0;
                    _verticalSpeed = 0;
                }
                if(_isJumping && Math.Abs(_horizontalSpeed) > 0) _horizontalSpeed /= 1.1;
            }

            if (_isJumping) Fall();

            PhysicalX += _horizontalSpeed;
            PhysicalY += _verticalSpeed;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Left))
            {
                _direction = Keyboard.IsKeyDown(Key.Right) ? (int) Directions.Default : (int) Directions.Left;
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                _direction = Keyboard.IsKeyDown(Key.Left) ? (int)Directions.Default : (int)Directions.Right;
            }
            if (e.Key == Key.Up)   _direction = (int) Directions.Up;
            if (e.Key == Key.Down) _direction = (int) Directions.Down;

            if (_pressedKeys.Contains(e.Key)) return;
            _pressedKeys.Add(e.Key);
            e.Handled = true;
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.Key);
            e.Handled = true;
            if (_pressedKeys.Count == 0) _direction = (int) Directions.Default;
            else
            {
                if (Keyboard.IsKeyDown(Key.Left)) _direction = (int)Directions.Left;
                else if (Keyboard.IsKeyDown(Key.Right)) _direction = (int)Directions.Right;    
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadLevel();
            RenderLevel();
            CompositionTarget.Rendering += GameLoop;
        }

        private int LogicX
        {
            get { return ((GameViewModel)DataContext).LogicX; }
        }

        private int LogicY
        {
            get { return ((GameViewModel)DataContext).LogicY; }
        }

        private double PhysicalX
        {
            get { return ((GameViewModel)DataContext).PhysicalX; }
            set { ((GameViewModel)DataContext).PhysicalX = value; }
        }

        private double PhysicalY
        {
            get { return ((GameViewModel)DataContext).PhysicalY; }
            set { ((GameViewModel)DataContext).PhysicalY = value; }
        }
    }
}