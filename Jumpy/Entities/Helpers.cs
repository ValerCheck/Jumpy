using System.Collections.Generic;

namespace Jumpy.Entities
{
    public enum EntityType
    {
        Space,
        Brick,
        RaisingPlatform
    }

    public static class Helper
    {
        public static Dictionary<EntityType,int> EntityTypes = new Dictionary<EntityType, int>()
        {
            {EntityType.Brick,1},
            {EntityType.Space,0}
        }; 
    }
}
