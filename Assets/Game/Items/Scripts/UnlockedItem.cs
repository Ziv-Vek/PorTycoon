using System;


[Serializable]
public class UnlockedItem
{
    public Item item;
    public DateTime Date;

    public UnlockedItem(Item item, DateTime unlockDate)
    {
        this.item = item;
        this.Date = unlockDate;
    }
}