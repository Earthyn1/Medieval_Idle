using System.Linq;
using UnityEngine;

public class ActionCampHandler : MonoBehaviour
{
    void Update()
    {
        foreach (var entry in DataGameManager.instance.activeCamps.ToList())
        {
            float progress = entry.GetProgress();            

            if (entry.Slot != null)
            {
                entry.Slot.UpdateProgressBar(progress); //This updates the slot progress bar.
            }

            if (entry.IsCompleted())
            {
                CompleteCampAction(entry);
                entry.RestartTimer();  // Automatically restart the timer
            }
        }
    }

    void CompleteCampAction(CampActionEntry entry)
    {        
        XPManager.AddXP(entry.CampData.campType, entry.CampData.xpGiven);

        float progress = XPManager.GetLevelProgress(entry.CampData.campType);
        Debug.Log($"{entry.CampData.campType} is {progress * 100f}%");

        RollForProducedItem(entry);
        

    }

    void RollForProducedItem(CampActionEntry campData)
    {
        // Generate a random number between 1 and 100
        int roll = UnityEngine.Random.Range(1, 101);
        Debug.Log($"Rolled: {roll}");

        float accumulatedChance = 0;

        // Sort produced items by drop chance (if not already sorted)
        campData.CampData.ProducedItems.Sort((a, b) => a.dropChance.CompareTo(b.dropChance));

        foreach (var producedItem in campData.CampData.ProducedItems)
        {
            // Accumulate the drop chance
            accumulatedChance += producedItem.dropChance;

            // Check if the roll falls within the current accumulated range
            if (roll <= accumulatedChance)
            {
                Debug.Log($"Item acquired: {producedItem.item}, Qty: {producedItem.qty}");

                // Add item to inventory
                TownStorageManager.AddItem(producedItem.item, producedItem.qty);
                return;
            }
        }

        // Fallback if no item matched (edge case)
        Debug.Log("No item acquired. Drop chances may not sum up to 100.");
    }


}