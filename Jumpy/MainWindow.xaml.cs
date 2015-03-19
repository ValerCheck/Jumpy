using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Jumpy.Entities;
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
        private const double GravityAcceleration = 0.5;
        
        #endregion

        private enum Directions
        {
            Up = 8,
            Down = 2,
            Left = 4,
            Right = 6,
            Default = 0
        }

        private enum EntityType
        {
            Space = 0,
            Brick = 1,
            RaisingPlatform
        }

        private int _direction;

        private double _verticalSpeed;
        private double _horizontalSpeed;
        
        
        private bool _isJumping;
        private bool IsMoving;
        
        private Canvas _root;
        private Image _player;

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
            var nearest = findNearest();
            if (!nearest.Any())
                return true;
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

        private List<Key> _pressedKeys = new List<Key>();
        private List<Level> Levels;
        private Level CurrentLevel;

        public void LoadLevel()
        {
            CurrentLevel = JsonConvert.DeserializeObject<Level>(File.ReadAllText(@"levelmaps.json"));
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
            var viewModel = DataContext as GameViewModel;
            var elementSize = RootCanvas.ActualHeight / CurrentLevel.Height;
            viewModel.ViewWidth = elementSize;
            viewModel.ViewHeight = elementSize;
            var leftMargin = (RootCanvas.ActualWidth - elementSize*CurrentLevel.Width)/2;
            viewModel.PhysicalZeroPointX = leftMargin;
            viewModel.PhysicalZeroPointY = 0;
            for (var x = 0; x < CurrentLevel.Width; x++)
            {
                for (var y = 0; y < CurrentLevel.Height; y++)
                {
                    if (CurrentLevel.Map[y, x] == (int)EntityType.Brick)
                    {
                        var brick = new Border
                        {
                            Width = elementSize,
                            Height = elementSize,
                            Background = Brushes.Brown,
                            BorderBrush = Brushes.Brown,
                        };
                        AddElementToCanvas(brick, (elementSize) * x + leftMargin, elementSize * y);
                        viewModel.Actors.Add(new Brick(new []{x,y}));
                    }
                }
            }
            var playerView = new Ellipse()
            {
                Width = elementSize,
                Height = elementSize,
                Fill = Brushes.Green,
            };
            RootCanvas.Children.Add(playerView);
            viewModel.Player = new Player(new []{CurrentLevel.PlayerPosition[0],CurrentLevel.PlayerPosition[1]});
            X = elementSize*CurrentLevel.PlayerPosition[0] + leftMargin;
            Y = elementSize*CurrentLevel.PlayerPosition[1];
            Binding phX = new Binding("PhysicalX") { Source = viewModel };
            Binding phY = new Binding("PhysicalY") { Source = viewModel };
            playerView.SetBinding(Canvas.LeftProperty, phX);
            playerView.SetBinding(Canvas.TopProperty, phY);
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

        private bool IsCollided()
        {
            return (X <= 0) || (X >= Width) || (CheckCollision() && !_isJumping);
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

        private bool CanJump()
        {
            var viewModel = (GameViewModel) DataContext;
            if (_isJumping && _verticalSpeed.Equals(0))
            {
                return true;
            }
            return false;
        }

        private void Jump()
        {
            _verticalSpeed = -VelocityJump;
            _isJumping = true;
        }

        private void Fall()
        {
            _verticalSpeed += GravityAcceleration;
        }

        private void MovePlayer()
        {
            if (_direction == (int) Directions.Left)
            {
                if (X > 0)
                    _horizontalSpeed = -VelocityHorizontal;
            }
            if (_direction == (int) Directions.Right)
            {
                if (X < RootCanvas.ActualWidth)
                    _horizontalSpeed = VelocityHorizontal;
            }
            if (_direction == (int) Directions.Up)
            {
                if (CanJump())
                    Jump();
            }
            if (_direction == (int) Directions.Default)
            {
                if (!_isJumping)
                {
                    _horizontalSpeed = 0;
                    _verticalSpeed = 0;
                }
                
            }

            if (_isJumping)
                Fall();

            X += _horizontalSpeed;
            Y += _verticalSpeed;
        }



        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Left))
            {
                if (Keyboard.IsKeyDown(Key.Right))
                {
                    _direction = (int) Directions.Default;
                }
                else
                {
                    _direction = (int)Directions.Left;
                }
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                if (Keyboard.IsKeyDown(Key.Left))
                {
                    _direction = (int)Directions.Default;
                }
                else
                {
                    _direction = (int)Directions.Right;
                }
            }
            if (e.Key == Key.Up)
            {
                if (!_isJumping)
                {
                    _direction = (int) Directions.Up;
                    _isJumping = true;    
                }
                                
            }
            if (e.Key == Key.Down)
                _direction = (int)Directions.Down;

            if (_pressedKeys.Contains(e.Key))
                return;
            _pressedKeys.Add(e.Key);
            e.Handled = true;
   
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.Key);
            e.Handled = true;
            if (_pressedKeys.Count == 0)
                _direction = (int) Directions.Default;
            else
            {
                if (Keyboard.IsKeyDown(Key.Left))
                    _direction = (int)Directions.Left;
                else if (Keyboard.IsKeyDown(Key.Right))
                    _direction = (int)Directions.Right;    
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadLevel();
            RenderLevel();
            CompositionTarget.Rendering += GameLoop;
        }

        private double X
        {
            get { return ((GameViewModel)DataContext).PhysicalX; }
            set { ((GameViewModel)DataContext).PhysicalX = value; }
        }

        private double Y
        {
            get { return ((GameViewModel)DataContext).PhysicalY; }
            set { ((GameViewModel)DataContext).PhysicalY = value; }
        }
    }
}