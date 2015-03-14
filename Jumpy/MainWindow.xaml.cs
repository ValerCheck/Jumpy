using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Jumpy
{
    public partial class MainWindow : Window
    {

        #region Keys

        private bool upKeyPressed;
        private bool downKeyPressed;
        private bool leftKeyPressed;
        private bool rightKeyPressed;
        
        #endregion

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
            get { return root ?? (root = Content as Canvas); }
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
                //break;
            }
            return rect1.IsEmpty;
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadLevel();
            RenderLevel();
            //CompositionTarget.Rendering += MainLoop;
        }

        public List<Level> Levels;
        public Level CurrentLevel;

        public void LoadLevel()
        {
            CurrentLevel = JsonConvert.DeserializeObject<Level>(File.ReadAllText(@"levelmaps.json"));
        }

        public void RenderLevel()
        {
            var map = CurrentLevel.Map;
            var elementHeight = SystemParameters.PrimaryScreenHeight / CurrentLevel.Height;
            var elementWidth = elementHeight;
            //var elementWidth = 32;
            //var elementHeight = 32;
            for (var x = 0; x < CurrentLevel.Width; x++)
            {
                for (var y = 0; y < CurrentLevel.Height; y++)
                {
                    if (map[y, x] == Helper.EntityTypes[EntityType.Brick])
                    {
                        var brick = new Border()
                        {
                            Width = elementWidth,
                            Height = elementHeight,
                            Background = Brushes.Brown,
                            BorderBrush = Brushes.Brown,
                            Padding = new Thickness(0),
                            Margin = new Thickness(0)
                            //Margin = new Thickness(elementWidth*x, elementHeight*y, 0, 0)
                        };
                        RootCanvas.Children.Add(brick);
                        Canvas.SetLeft(brick,elementWidth*x);
                        Canvas.SetTop(brick,elementHeight*y);
                    }
                }
            }
            var playerView = new Ellipse()
            {
                Width = elementWidth,
                Height = elementHeight,
                Fill = Brushes.Green,
                Margin = new Thickness(elementWidth*CurrentLevel.PlayerPosition[0], 
                    elementHeight*CurrentLevel.PlayerPosition[1], 0, 0)
            };
            RootCanvas.Children.Add(playerView);
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

        private void MainLoop(object sender, EventArgs arts)
        {
            if (checkCollision())
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
            }
            if (leftKeyPressed)
            {
                if (X > 0 && MoveSpeed > -4)
                {
                    MoveSpeed -= 0.5;
                }
                
            }
            else if (rightKeyPressed)
            {
                if(X < Width - 70 && MoveSpeed < 4)
                    MoveSpeed += 0.5;
            }
            if (upKeyPressed)
            {
                if (!IsJumping)
                {
                    IsJumping = true;
                    VerticalSpeed = 10;
                }
            }
            MoveObject();
            
        }

        private double X
        {
            get { return (DataContext as PlayerBallViewModel).X; }
            set { (DataContext as PlayerBallViewModel).X = value; }
        }

        private double Y
        {
            get { return (DataContext as PlayerBallViewModel).Y; }
            set { (DataContext as PlayerBallViewModel).Y = value; }
        }

        private void MoveObject()
        {
            X += MoveSpeed;
            Y += VerticalSpeed;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) upKeyPressed = true;
            if (e.Key == Key.Down) downKeyPressed = true;
            if (e.Key == Key.Left) leftKeyPressed = true;
            if (e.Key == Key.Right) rightKeyPressed = true;
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) upKeyPressed = false;
            if (e.Key == Key.Down) downKeyPressed = false;
            if (e.Key == Key.Left) leftKeyPressed = false;
            if (e.Key == Key.Right) rightKeyPressed = false;
        }
    }
}
