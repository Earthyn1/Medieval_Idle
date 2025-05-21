using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

                if (HasEnoughResources(entry))
                {
                    entry.RestartTimer();  // Automatically restart the timer
                }
                else
                {
                    Debug.Log("Not enough resource to restart action");

                    if (entry.Slot == null)
                    {
                        DataGameManager.instance.activeCamps.Remove(entry);
                        DataGameManager.instance.CurrentVillagerCount = DataGameManager.instance.CurrentVillagerCount + entry.CampData.populationCost;
                        DataGameManager.instance.topPanelManager.UpdateTownPopulation();
                       
                    }
                    else
                    {
                        entry.Slot.DeactivateActionSlot();
                    }
                }
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
                TownStorageManager.AddItem(producedItem.item, producedItem.qty, campData.CampData.campType);
                return;
            }
        }

        // Fallback if no item matched (edge case)
        Debug.Log("No item acquired. Drop chances may not sum up to 100.");
    }

    public void CheckforRequiredResourceRemoval(CampActionEntry entry)
    {
        if (entry.CampData.RequiredItems.Count > 0)
        {
            foreach (SimpleItemData item in entry.CampData.RequiredItems)
            {
                TownStorageManager.RemoveItem(item.item, item.qty);
            }
        }
    }

    public bool HasEnoughResources(CampActionEntry campData)
    {
        bool hasEnough = campData.CampData.RequiredItems.All(item =>
            DataGameManager.instance.TownStorage_List.Any(slot =>
                slot.ItemID == item.item && slot.Quantity >= item.qty));

        if (hasEnough)
        {
            CheckforRequiredResourceRemoval(campData);
        }

        return hasEnough;
    }



}