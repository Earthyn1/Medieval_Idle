[System.Serializable]
public class SimpleItemData
{
    public string item;
    public int qty;
    public float dropChance;

    public SimpleItemData(string item, int qty, float dropChance)
    {
        this.item = item;
        this.qty = qty;
        this.dropChance = dropChance;
    }
}