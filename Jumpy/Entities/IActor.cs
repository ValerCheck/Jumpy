namespace Jumpy.Entities
{
    public interface IActor
    {
        int[] LogicPosition { get; set; }
        int X { get; set; }
        int Y { get; set; }
        bool IsSolid { get; }
    }
}
