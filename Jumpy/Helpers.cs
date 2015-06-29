using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jumpy
{
    public class ModelPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public enum Directions
    {
        Left = 1,
        Up = 2,
        Right = 3,
        Down = 4,
        Default = 0
    }

    public enum EntityType
    {
        Space = 0,
        Brick = 1,
    }
}
