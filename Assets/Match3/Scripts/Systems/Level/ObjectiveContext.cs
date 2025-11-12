using System.Collections.Generic;

namespace Level
{
    public class ObjectiveContext
    {
        public int CurrentScore;
        public Dictionary<string, int> GemsDestroyed = new(); // key: GemSO.name
        public int ObstaclesDestroyed;
    }
}