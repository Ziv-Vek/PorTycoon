using UnityEngine;

[System.Serializable]
public class Item
{
    public string id;
    public string name;
    public string description;
    public string image;

    // public Sprite Icon;
    // public string Type;
    // public Color Color;
    public int rarity;
    [System.NonSerialized] public float Probability; // We don't load this from the JSON, it's calculated in the code.
}