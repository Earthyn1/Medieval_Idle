using UnityEngine;

public interface ICampActionHandler
{
    void UpdateProgress(CampActionEntry entry);
    bool IsCompleted(CampActionEntry entry);

    void RestartTimer(CampActionData entry);


}

public static class CampActionHandlerFactory
{
    public static ICampActionHandler GetHandler(CampType campType)
    {
        return campType switch
        {
            CampType.FishingCamp => new FishingCampHandler(),
            _ => new DefaultCampHandler(),
        };
    }
}