using UnityEngine;




public static class LocalMarketManager
{

    public static void SetupLocalMarket()
    {
        foreach (Transform child in DataGameManager.instance.populate_Camp_Slots.parentContainer) // Clear existing slots
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }



}

