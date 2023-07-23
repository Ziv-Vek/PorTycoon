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
            // Calculate probabilities
            float totalWeight = levelConfig.items.Sum(item => item.rarity);
            foreach (var item in levelConfig.items)
            {
                item.Probability = item.rarity / totalWeight;
            }

            return levelConfig;
        }

        Debug.LogError("Level " + levelID + " not found in config");
        return null;
    }
}