using System.Threading;
using UnityEngine;

public class LocalMarket_Items
{

    public string itemID;
    public int itemSellPrice;
    public int itemStackSizeSold;
    public bool itemLocked;

    public LocalMarket_Items(string itemID, int itemSellPrice, int itemStackSizeSold, bool itemLocked)
    {
        this.itemID = itemID;
        this.itemSellPrice = itemSellPrice;
        this.itemStackSizeSold = itemStackSizeSold;
        this.itemLocked = itemLocked;   
    }

    public LocalMarket_Items() { }

}
