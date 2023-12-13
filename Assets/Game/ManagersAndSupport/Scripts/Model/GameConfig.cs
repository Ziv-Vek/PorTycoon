using System;
using System.Collections.Generic;

[Serializable]
public class GameConfig
{
    public Dictionary<string,string> colors;
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

    public DateTime DateUnlocked;
}

[Serializable]
public class Level
{
    public Dictionary<string, Upgrades> upgrades;
    public int levelId;
    public Dictionary<string, Box> boxes;
}
[Serializable]
public class Upgrades
{
    public List<float> levels;
    public List<float> prices;
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
    public string color;
}