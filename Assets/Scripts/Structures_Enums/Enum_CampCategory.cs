using UnityEngine;

public enum CampCategorys
{
    Blacksmith_Bronze,
    Blacksmith_Iron,
    Blacksmith_Steel,
    Construction_Materials,
    Construction_Buildings,
    NA,
}


[CreateAssetMenu(fileName = "CampCategoryData", menuName = "ScriptableObjects/CampCategoryData", order = 1)]
public class CampCategoryData : ScriptableObject
{
    public CampType campType;
    public CampCategorys campCategory;
    public Sprite categorysImage;
}