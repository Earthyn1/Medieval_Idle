using UnityEngine;
using UnityEngine.UI;

public class XP_Feed_Slot : MonoBehaviour
{
    public Text xp_feed_SlotText;
    public Image xp_feed_SlotImage;
    public Animator xp_feed_SlotAnimation;
    public GameObject item_xp_feed_Slot;

    private void Start()
    {
      
    }

  

    public void OnAnimationFinished()
    {
        
        Item_XP_Feed_Slot parentscript = item_xp_feed_Slot.GetComponent<Item_XP_Feed_Slot>();
      

    }
}
