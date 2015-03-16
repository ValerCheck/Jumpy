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
using System.Windows.Threading;

namespace Jumpy
{
    public partial class MainWindow
    {

        #region Keys

        private bool upKeyPressed;
        private bool downKeyPressed;
        private bool leftKeyPressed;
        private bool rightKeyPressed;
        
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

        private int Direction = 0;

        private double VerticalSpeed = 2;
        private double MoveSpeed = 0;
        private const double MaxDownSpeed = 6;
        private const double MaxMoveSpeed = 4;
        private bool IsJumping;
        private bool IsMoving;
        private Canvas root;
        private Image player;

        public Canvas RootCanvas
        {
            get { return root ?? (root = (Content as Grid).Children.OfType<Canvas>().First()); }
        }

        public Image Player
        {
            get
            {
                return player ?? (player = (from c in RootCanvas.Children.Cast<Object>()
                    where c.GetType() == typeof (Image)
                    select c).Cast<Image>().Select(r => r)
                    .FirstOrDefault(c => (string) c.Tag == "player"));
            }
        }

        private bool checkCollision()
        {
            var rect1 = CreateElementRect(Player);
            var nearest = findNearest();
            if (nearest.Count() == 0)
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

        public void RenderLevel()
        {
            var viewModel = DataContext as GameViewModel;
            var map = CurrentLevel.Map;
            var elementHeight = RootCanvas.ActualHeight / CurrentLevel.Height;
            var elementWidth = elementHeight;
            viewModel.ViewWidth = elementWidth;
            viewModel.ViewHeight = elementHeight;
            var leftMargin = (RootCanvas.ActualWidth - elementWidth*CurrentLevel.Width)/2;
            for (var x = 0; x < CurrentLevel.Width; x++)
            {
                for (var y = 0; y < CurrentLevel.Height; y++)
                {
                    if (map[y, x] == (int)EntityType.Brick)
                    {
                        var brick = new Border()
                        {
                            Width = elementWidth,
                            Height = elementHeight,
                            Background = Brushes.Brown,
                            BorderBrush = Brushes.Brown,
                        };
                        RootCanvas.Children.Add(brick);
                        Canvas.SetLeft(brick,(elementWidth)*x + leftMargin);
                        Canvas.SetTop(brick,elementHeight*y);
                        viewModel.Actors.Add(new Brick(new []{x,y}));
                    }
                }
            }
            var playerView = new Ellipse()
            {
                Width = elementWidth,
                Height = elementHeight,
                Fill = Brushes.Green,
            };
            RootCanvas.Children.Add(playerView);
            //Canvas.SetLeft(playerView,elementWidth * CurrentLevel.PlayerPosition[0]);
            Canvas.SetTop(playerView, elementHeight * CurrentLevel.PlayerPosition[1]);
            viewModel.Player = new Player(new []{CurrentLevel.PlayerPosition[0],CurrentLevel.PlayerPosition[1]});
            X = elementWidth*CurrentLevel.PlayerPosition[0] + 300;
            Binding phX = new Binding("PhysicalX");
            phX.Source = viewModel;
            playerView.SetBinding(Canvas.LeftProperty, phX);
        }

        private IEnumerable<FrameworkElement> findNearest()
        {
            var allObstacles = (from child in RootCanvas.Children.Cast<Object>()
                where child is Rectangle
                select child).Cast<FrameworkElement>();
            var nearest = new List<FrameworkElement>();
            var playerRect = CreateElementRect(Player);
            //playerRect.Scale(2,2);
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
            return (X <= 0) || (X >= Width) || (checkCollision() && !IsJumping);
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

        private void MovePlayer()
        {
            /*if (checkCollision())
            {
                if (VerticalSpeed > -MaxDownSpeed)
                    VerticalSpeed -= 0.5;
            }
            else
            {
                VerticalSpeed = 0;
                IsJumping = false;
            }
            if (!IsCollided())
            {
                if (MoveSpeed > 0)
                {
                    MoveSpeed -= 0.05;
                    if (MoveSpeed < 0.05)
                        MoveSpeed = 0;
                }
                if (MoveSpeed < 0)
                    MoveSpeed += 0.05;
            }
            else
            {
                MoveSpeed = -MoveSpeed;
            }*/

            switch (Direction)
            {
                case (int)Directions.Left:
                    if (X > 0 && MoveSpeed >= 0)
                    {
                        MoveSpeed = -4;
                    }
                    break;
                case (int)Directions.Right:
                    if (X < Width && MoveSpeed <= 0)
                    {
                        MoveSpeed = 4;
                    }
                    break;
                case (int)Directions.Up:
                    if (!IsJumping)
                    {
                        IsJumping = true;
                        VerticalSpeed = 10;
                    }
                    break;
                case (int)Directions.Default:
                    MoveSpeed = 0;
                    break;
            }

            X += MoveSpeed;
            Y += VerticalSpeed;
        }

        private double X
        {
            get { return (DataContext as GameViewModel).PhysicalX; }
            set { (DataContext as GameViewModel).PhysicalX = value; }
        }

        private double Y
        {
            get { return (DataContext as GameViewModel).PhysicalY; }
            set { (DataContext as GameViewModel).PhysicalY = value; }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Left))
            {
                if (Keyboard.IsKeyDown(Key.Right))
                {
                    Direction = (int) Directions.Default;
                }
                else
                {
                    Direction = (int)Directions.Left;
                }
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                if (Keyboard.IsKeyDown(Key.Left))
                {
                    Direction = (int)Directions.Default;
                }
                else
                {
                    Direction = (int)Directions.Right;
                }
            }
            if (e.Key == Key.Up)
                Direction = (int)Directions.Up;
            if (e.Key == Key.Down)
                Direction = (int)Directions.Down;

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
                Direction = (int)Directions.Default;
            if (Keyboard.IsKeyDown(Key.Left))
                Direction = (int) Directions.Left;
            if (Keyboard.IsKeyDown(Key.Right))
                Direction = (int) Directions.Right;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadLevel();
            RenderLevel();

            CompositionTarget.Rendering += GameLoop;
        }

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Up && e.Key == Key.Down)
            //    Direction = (int) Directions.Default;
            //if (e.Key == Key.Up)
            //    Direction = (int)Directions.Up;
            //if (e.Key == Key.Right)
            //    Direction = (int)Directions.Right;
            //if (e.Key == Key.Down)
            //    Direction = (int)Directions.Down;
            //if (e.Key == Key.Left)
            //    Direction = (int)Directions.Left;
            
        }
    }
}