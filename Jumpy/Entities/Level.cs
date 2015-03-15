namespace Jumpy.Entities
{
    public class Level
    {

        public Level(){}

        public Level(int width,int height)
        {
            Width = width;
            Height = height;
            Map = new int[Width,Height];
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public int[,] Map;
        public int[] PlayerPosition { get; set; }
    }
}
