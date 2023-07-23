using System.Linq;
using UnityEngine;

public class ConfigManager
{
    public GameConfig Config { get; private set; }

    public ConfigManager()
    {
        LoadConfig();
    }


    private GameConfig LoadConfig()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("config");
        Config = JsonUtility.FromJson<GameConfig>(jsonText.text);

        return Config;
    }

    public LevelData GetLevelConfig(string levelID)
    {
        if (Config == null)
        {
            Debug.LogError("Config not loaded");
            return null;
        }

        var levelConfig = Config.levels.FirstOrDefault(level => level.id == levelID);
        if (levelConfig != null)
        {
            // Calculate total weight
            float totalWeight = levelConfig.items.Sum(item => item.rarity);

            foreach (var item in levelConfig.items)
            {
                // Compute the item's probability as the inverse proportion of its rarity
                item.Probability = totalWeight / item.rarity;
            }

            // Normalize probabilities so that their sum is 1
            float totalProbability = levelConfig.items.Sum(item => item.Probability);
            foreach (var item in levelConfig.items)
            {
                item.Probability /= totalProbability;
            }

            return levelConfig;
        }

        Debug.LogError("Level " + levelID + " not found in config");
        return null;
    }
}