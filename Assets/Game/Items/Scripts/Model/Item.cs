using UnityEngine;

[System.Serializable]
public class Item
{
    public string id;
    // public string Name { get; set; }
    // public Sprite Icon { get; set; }
    // public string Type { get; set; }
    // public Color Color { get; set; }
    public int rarity;
    [System.NonSerialized] public float Probability; // We don't load this from the JSON, it's calculated in the code.
}