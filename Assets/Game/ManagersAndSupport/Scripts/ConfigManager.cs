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

            LoadConfig();
            if (!IsConfigValid())
            {
                Debug.LogError("Config is not valid");
                // exit the game
                throw new Exception("Config is not valid");
            }
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
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
                float totalProbability = box.items.Sum(item => item.probability);
                if (totalProbability != 100)
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
        }

        return true;
    }
}