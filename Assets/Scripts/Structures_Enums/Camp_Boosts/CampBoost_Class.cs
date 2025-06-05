using UnityEngine;

public class CampBoost_Class
{
    public string boostName;
    public string boostDescription;
    public Sprite boostSprite;
    public float boostAmount;
    public BoostUnit boostUnit;

    public string GetFormattedAmount()
    {
        return boostUnit switch
        {
            BoostUnit.Percent => $"+{boostAmount * 100f:0.#}%",
            BoostUnit.Seconds => $"-{boostAmount:0.##}s",
            BoostUnit.Flat => $"+{boostAmount}",
            _ => boostAmount.ToString()
        };
    }
}

public enum BoostUnit
{
    Percent,      // %
    Seconds,      // s
    Flat          // flat number (e.g., +3 catches)
}
