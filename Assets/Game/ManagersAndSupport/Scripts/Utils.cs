using System.Collections.Generic;

namespace Game.ManagersAndSupport.Scripts
{
    public static class Utils
    {
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int r = UnityEngine.Random.Range(0, i);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}