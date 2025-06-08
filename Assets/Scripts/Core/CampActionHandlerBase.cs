using UnityEngine;

public interface ICampActionHandler
{
    void UpdateProgress(CampActionEntry entry);
    bool IsCompleted(CampActionEntry entry);
    void RestartTimer(CampActionEntry entry);
    void CompleteAction(CampActionEntry entry);
    bool HasEnoughCampSpecificResources(CampActionEntry entry);
    void RemoveCampSpecificResources(CampActionEntry entry);
    void ReturnCampSpecificResources(CampActionEntry entry);

}

public static class CampActionHandlerFactory
{
    private static MiningCampHandler _miningHandlerInstance;
    private static FishingCampHandler _fishingHandlerInstance;
    private static ConstructionCampHandler _constructionHandlerInstance;

    private static DefaultCampHandler _defaultHandlerInstance;

    public static ICampActionHandler GetHandler(CampType campType)
    {
        switch (campType)
        {
            case CampType.MiningCamp:
                if (_miningHandlerInstance == null)
                    _miningHandlerInstance = new MiningCampHandler();
                return _miningHandlerInstance;

            case CampType.FishingCamp:
                if (_fishingHandlerInstance == null)
                    _fishingHandlerInstance = new FishingCampHandler();
                return _fishingHandlerInstance;

            case CampType.ConstructionCamp:
                if (_constructionHandlerInstance == null)
                    _constructionHandlerInstance = new ConstructionCampHandler();
                return _constructionHandlerInstance;

            // other camp types...

            default:
                if (_defaultHandlerInstance == null)
                    _defaultHandlerInstance = new DefaultCampHandler();
                return _defaultHandlerInstance;
        }
    }
}