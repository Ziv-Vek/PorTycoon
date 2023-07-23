using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

[System.Serializable]
public class GameConfig
{
    public List<LevelData> levels;
}

[System.Serializable]
public class LevelData
{
    public string id;
    public List<Item> items;

    public Item GetRandomItemForLevel()
    {
        // Generate a random value from 0 to 1
        float randomValue = UnityEngine.Random.value;

        // Find which item this random value corresponds to
       items = items.OrderBy(item => item.Probability).ToList();
        foreach (var item in items)
        {
            if (randomValue <= item.Probability)
            {
                return item;
            }

            randomValue -= item.Probability;
        }

        // If for some reason no item was found (which should never happen), return null
        return null;
    }
}