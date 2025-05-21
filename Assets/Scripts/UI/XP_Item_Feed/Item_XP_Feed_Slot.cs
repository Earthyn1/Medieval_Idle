using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;


public class Item_XP_Feed_Slot : MonoBehaviour
{
    public GameObject item_Panel_Parent;
    public GameObject xp_feed_Slot;
    public CampType CampType;
    public GameObject panel;
   




    public void SetupXP(string xpAmount,Sprite campXPImage)
    {
        XP_Feed_Slot xpFeedSlot_Script = xp_feed_Slot.GetComponent<XP_Feed_Slot>();
        xpFeedSlot_Script.xp_feed_SlotText.text = xpAmount;
        xpFeedSlot_Script.xp_feed_SlotImage.sprite = campXPImage;
        xpFeedSlot_Script.item_xp_feed_Slot = gameObject;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);

    }

    



}
