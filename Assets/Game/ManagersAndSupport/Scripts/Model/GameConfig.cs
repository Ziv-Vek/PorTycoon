using System;
using System.Collections.Generic;

[Serializable]
public class GameConfig
{
    public List<Item> items;
    public List<Level> levels;
}

[Serializable]
public class Item
{
    public string id;
    public string name;
    public string description;
    public string imagePath;

    [NonSerialized] public DateTime DateUnlocked;
}

[Serializable]
public class Level
{
    public int levelId;
    public Dictionary<string, Box> boxes;
}

[Serializable]
public class Box
{
    public int probability;
    public List<BoxItem> items;
}

[Serializable]
public class BoxItem
{
    public string id;
    public float probability;
}