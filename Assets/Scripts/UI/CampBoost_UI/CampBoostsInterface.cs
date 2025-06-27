using System.Collections.Generic;

public interface ICampBoostData
{
    int CurrentTier { get; set; }  // now it's writable
    List<CampBoost_Class> GetAllBoosts();

    int Resource1_Current { get; }
    int Resource2_Current { get; }

    void AddResources(string itemId, int amount, CampTiersArray tierData);
    bool IsResourceComplete(CampTiersArray tierData);

    void ResetResources();

}


