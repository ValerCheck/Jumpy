using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using Jumpy.Model;

namespace Jumpy.DataStructures
{
    public class QuadTree
    {
        private int MAX_OBJECTS = 10;
        private int MAX_LEVELS = 5;

        private int level;
        private List<IGameElement> objects;
        private Rectangle bounds;
        private QuadTree[] nodes;

        public QuadTree(int pLevel, Rectangle pBounds)
        {
            level = pLevel;
            objects = new List<IGameElement>();
            bounds = pBounds;
            nodes = new QuadTree[4];
        }

        public void Clear()
        {
            objects.Clear();
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null) continue;
                nodes[i].Clear();
                nodes[i] = null;
            }
        }

        private void Split()
        {
            var subWidth = (bounds.Width/2);
            var subHeight = (bounds.Height/2);
            var x = Canvas.GetLeft(bounds);
            var y = Canvas.GetTop(bounds);

            var boundsI = new Rectangle() {Width = subWidth, Height = subHeight};
            Canvas.SetLeft(boundsI,x + subWidth);
            Canvas.SetTop(boundsI,y);
            var boundsII = new Rectangle() {Width = subWidth, Height = subHeight};
            Canvas.SetLeft(boundsII,x);
            Canvas.SetTop(boundsII,y);
            var boundsIII = new Rectangle() {Width = subWidth, Height = subHeight};
            Canvas.SetLeft(boundsIII,x);
            Canvas.SetTop(boundsIII,y + subHeight);
            var boundsIV = new Rectangle() {Width = subWidth, Height = subHeight};
            Canvas.SetLeft(boundsIV,x + subWidth);
            Canvas.SetTop(boundsIV,y + subHeight);

            nodes[0] = new QuadTree(level+1,boundsI);
            nodes[1] = new QuadTree(level+1,boundsII);
            nodes[2] = new QuadTree(level+1,boundsIII);
            nodes[3] = new QuadTree(level+1,boundsIV);
        }

        private int getIndex(Rectangle pRect)
        {
            var index = -1;
            double verticalMidPoint = Canvas.GetLeft(bounds) + (bounds.Width/2);
            double horizontalMidPoint = Canvas.GetTop(bounds) + (bounds.Height/2);
            var topQuadrant = (Canvas.GetTop(pRect) < horizontalMidPoint &&
                               Canvas.GetTop(pRect) + pRect.Height < horizontalMidPoint);
            return index;
        }

    }
}
