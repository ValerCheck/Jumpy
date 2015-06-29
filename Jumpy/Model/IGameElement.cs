namespace Jumpy.Model
{
    public interface IGameElement
    {
        double X { get; set; }
        double Y { get; set; }
        double PrevX { get; set; }
        double PrevY { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }
}
