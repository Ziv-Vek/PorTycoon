using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item", order = 1)]
public class Item : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public string type;
    public string rarity;
    public Color color;
}