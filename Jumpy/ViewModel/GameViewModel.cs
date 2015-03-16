﻿using System.Windows.Input;
using Jumpy.Entities;
using ViewModel;
using System.Collections.Generic;
using System;

namespace Jumpy.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private List<IActor> _actors = new List<IActor>();
        private Player _player;
        private double _x = 0;
        private double _y = 400;
        private bool isJumping;
        private double _gravity = 2;
        private double _velocity = 20;

        public double ViewWidth { get; set; }
        public double ViewHeight { get; set; }

        public List<IActor> Actors
        {
            get { return _actors; }
            set
            {
                _actors = value;
                RaisePropertyChanged("Actors");
            }
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

        public double PhysicalX
        {
            get { return _x; }
            set
            {
                _x = value;
                Player.X = Convert.ToInt16(Math.Round(_x/ViewWidth));
                RaisePropertyChanged("PhysicalX");
            }
        }

        public double PhysicalY
        {
            get { return _y; }
            set
            {
                _y = value;
                Player.Y = Convert.ToInt16(Math.Round(_y/ViewHeight));
                RaisePropertyChanged("PhysicalY");
            }
        }

        private void MoveLeftAction()
        {
            if (Player.X > 0)
                PhysicalX -= _velocity;
        }

        private void MoveRightAction()
        {
            if (PhysicalX < 1000)
                PhysicalX += _velocity;
        }

        private void JumpAction()
        {
            isJumping = true;
            while (isJumping)
            {
                if (_y + _velocity > 0)
                {
                    PhysicalY += _velocity;
                    _velocity -= _gravity;
                }
                else
                {
                    PhysicalY = 0;
                    isJumping = false;
                }
            }
            
            _velocity = 20;
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