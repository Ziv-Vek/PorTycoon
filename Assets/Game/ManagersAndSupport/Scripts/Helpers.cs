using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Helpers
{
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
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
}