using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Helpers
{
    private static System.Random systemRandom = new System.Random();
    
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int r = Random.Range(0, i);
            (list[i], list[r]) = (list[r], list[i]);
        }

        return list;
    }

    public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        List<KeyValuePair<TKey, TValue>> tempList = new List<KeyValuePair<TKey, TValue>>(dictionary);

        int n = tempList.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int r = Random.Range(0, i);
            (tempList[i], tempList[r]) = (tempList[r], tempList[i]);
        }

        return tempList.ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    
    public static float GenerateRandomNumber(float min, float max)
    {
        // Generate a random number between 0.0 and 1.0
        double randomNumber = systemRandom.NextDouble();

        // Scale and shift the number to fit the range 0.2 to 0.8
        return (float)(min + randomNumber * (max - min));
    }
    
}