using System;
using System.Linq;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    private static ConfigManager _instance;

    public static ConfigManager Instance { get; private set; }

    public GameConfig Config { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the manager persists between scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }


        LoadConfig();
        if (!IsConfigValid())
        {
            Debug.LogError("Config is not valid");
            throw new Exception("Config is not valid");
        }
    }


    private void LoadConfig()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("config");
        Config = Newtonsoft.Json.JsonConvert.DeserializeObject<GameConfig>(jsonText.text);
    }

    private bool IsConfigValid()
    {
        // check all items probabilities equals 100
        foreach (var level in Config.levels)
        {
            foreach (var box in level.boxes.Values)
            {
                // all boxes probabilities equals 100
                float totalBoxProbability = level.boxes.Values.Sum(b => b.probability);
                if (totalBoxProbability != 100)
                {
                    Debug.LogError($"Level {level.levelId} box probability sum is not 100");
                    return false;
                }

                float totalItemProbability = box.items.Sum(item => item.probability);
                if (totalItemProbability != 100)
                {
                    Debug.LogError($"Box {box} probability is not 100");
                    return false;
                }

                // check all items exists in items list
                foreach (var boxItem in box.items)
                {
                    if (!Config.items.Exists(i => i.id == boxItem.id))
                    {
                        Debug.LogError($"Item {boxItem.id} not found in items list");
                        return false;
                    }
                }
            }

            // Check all upgrades have total prices minus one equals to upgrades levels count
            foreach (var upgrade in level.upgrades.Values)
            {
                if (upgrade.levels.Count - 1 != upgrade.prices.Count)
                {
                    Debug.LogError(
                        $"Level {level.levelId} upgrade {upgrade} prices count is not equal to levels count");
                    return false;
                }
            }
        }

        return true;
    }

    public string GetBoxTypeRandomly(int? level)
    {
        // If the level is null, use the current level
        level ??= GameManager.Instance.CurrentLevel;

        var boxes = Config.levels[(int)level - 1].boxes.Shuffle();

        // Calculate the total probability for normalization
        float totalProbability = 100;

        // Generate a random value between 0 and the total probability
        float randomValue = UnityEngine.Random.Range(0, totalProbability);

        // Loop through the boxes to find the corresponding random box type
        foreach (var boxPair in boxes)
        {
            string boxType = boxPair.Key;
            Box box = boxPair.Value;

            if (randomValue < box.probability)
            {
                return boxType;
            }

            randomValue -= box.probability;
        }


        // This point should never be reached if the probabilities are set up correctly.
        Debug.LogError($"Error selecting box type in level {level}. Check your box probabilities configuration.");
        return null;
    }
}