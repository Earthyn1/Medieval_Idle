using UnityEngine;
using UnityEngine.UI;

public class Tier_Shield : MonoBehaviour
{
    public Image campImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataGameManager.instance.tierShield = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
