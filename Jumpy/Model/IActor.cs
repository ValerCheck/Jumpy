namespace Jumpy.Entities
{
    public interface IActor
    {
        ModelPoint LogicPosition { get; set; }
        int X { get; set; }
        int Y { get; set; }
        bool IsSolid { get; }
    }
}
