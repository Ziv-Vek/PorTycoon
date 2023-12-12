using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-3)]
public class ItemsManager : MonoBehaviour
{
    private static ItemsManager _instance;

    public GameObject NewItemCanvas;
    public GameObject FinishCollectionCanvas;
    public static ItemsManager Instance { get; private set; }

    private GameConfig _gameConfig;

    private List<List<Item>> _cachedAllItemsListByLevel;
    private Dictionary<int, List<Item>> _itemsCache = new();
    public Dictionary<string, Item> UnlockedItems { get; } = new();

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

        _gameConfig = ConfigManager.Instance.Config;
    }


    public Item GetRandomItemFromBox(string boxType, int? levelNum)
    {
        // If the level is null, use the current level
        levelNum ??= GameManager.Instance.CurrentLevel;

        // Find the desired level by its ID
        Level targetLevel = _gameConfig.levels[(int)(levelNum - 1)];

        if (targetLevel == null)
        {
            Debug.LogError($"No level found with ID: {levelNum}");
            return null;
        }

        // Find the desired box within the level by its type
        if (!targetLevel.boxes.ContainsKey(boxType))
        {
            Debug.LogError($"No box of type: {boxType} found in level with ID: {levelNum}");
            return null;
        }

        Box targetBox = targetLevel.boxes[boxType];

        // Convert Dictionary to a List and shuffle it
        IList<BoxItem> shuffledItems = targetBox.items.Shuffle();

        float totalRarity = 100;

        // Generate a random value between 0 and the totalRarity
        float randomValue = UnityEngine.Random.Range(0, totalRarity);

        // Loop through the shuffled items to find the corresponding random item
        foreach (var boxItem in shuffledItems)
        {
            if (randomValue < boxItem.probability)
            {
                return _gameConfig.items.Find(i => i.id == boxItem.id);
            }

            randomValue -= boxItem.probability;
        }

        return null;
    }

    public void UnlockItem(Item item)
    {
        if (UnlockedItems.ContainsKey(item.id))
        {
            Debug.Log("Duplication: " + item.name);
            Bank.Instance.SpawnStar();
            return;
        }

        item.DateUnlocked = DateTime.Now;
        UnlockedItems.Add(item.id, item);
        UIManager.Instance.UpdateUI();

        if (IsLevelCompleted(GameManager.Instance.CurrentLevel) && GameObject.Find("Fishing") == null)
        {
            FinishCollectionCanvas.SetActive(true);
            FinishCollectionCanvas.GetComponent<CollectionFinishScreen>().StartAnimation(GetAllLevelItems(GameManager.Instance.currentLevel));
            UIManager.ShowWinPanel();
            Bank.Instance.AddMoneyToPile(GameObject.Find("ScretchMoneyPile").GetComponent<MoneyPile>(), "Win");
        }
        // Showing the item if its new 
        else if (GameObject.Find("Fishing") == null) //Checking if the player is not fishing in this time
        {
            NewItemCanvas.SetActive(true);
            NewItemCanvas.GetComponent<NewItemScreen>().AddItemToList(item);
        }


    }

    public List<Item> GetAllLevelItems(int levelNum)
    {
        // Check if the result is already cached
        if (_itemsCache.TryGetValue(levelNum, out List<Item> cachedItems))
        {
            return cachedItems;
        }

        var distinctItemIds = _gameConfig.levels[levelNum - 1]
            .boxes
            .Values
            .SelectMany(box => box.items)
            .Select(item => item.id)
            .Distinct()
            .ToList();

        // Filter the global items list based on the distinct IDs
        var items = _gameConfig.items
            .Where(item => distinctItemIds.Contains(item.id))
            .ToList();

        return items;
    }


    public List<List<Item>> GetAllItemsListByLevel()
    {
        if (_cachedAllItemsListByLevel != null)
            return _cachedAllItemsListByLevel;

        List<List<Item>> allItemsList = new List<List<Item>>();
        for (int i = 0; i < _gameConfig.levels.Count; i++)
        {
            allItemsList.Add(GetAllLevelItems(i + 1));
        }

        _cachedAllItemsListByLevel = allItemsList; // Cache the result
        return allItemsList;
    }

    public bool IsLevelCompleted(int levelNum)
    {
        List<Item> allLevelItems = GetAllLevelItems(levelNum);
        Debug.Log("all level items:" + allLevelItems.Count + " unlocked items:" + UnlockedItems.Count);
        return allLevelItems.All(item => UnlockedItems.ContainsKey(item.id));
    }

    public void SaveData(UserData userData)
    {
        userData.unlockedItems = UnlockedItems;
    }

    public void ResetData()
    {
        UnlockedItems?.Clear();
    }

    public void LoadData(UserData userData)
    {
        if (userData.unlockedItems == null)
            return;

        foreach (var item in userData.unlockedItems)
        {
            UnlockedItems.Add(item.Key, item.Value);
        }
    }
}