using UnityEngine;

public class CampTiersArray
{
    public int tierLevel;
    public CampBoost_Class boost_1;
    public CampBoost_Class boost_2;
    public CampBoost_Class boost_3;
    public CampBoost_Class boost_4;
    public int goldCost;
    public SimpleItemData resource1;
    public SimpleItemData resource2;
   


    public CampTiersArray(
        CampBoost_Class b1,
        CampBoost_Class b2,
        CampBoost_Class b3,
        CampBoost_Class b4,
        int tierLevel,
        int goldCost,
        SimpleItemData resource1,
        SimpleItemData resource2)
    {
        boost_1 = b1;
        boost_2 = b2;
        boost_3 = b3;
        boost_4 = b4;
        this.tierLevel = tierLevel;
        this.goldCost = goldCost;
        this.resource1 = resource1;
        this.resource2 = resource2;
    }
}
