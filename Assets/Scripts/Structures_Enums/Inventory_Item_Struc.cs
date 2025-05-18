using UnityEngine;


    public struct StorageSlot
    {
        public string ItemID;
        public int Quantity;

        public StorageSlot(string id, int qty)
        {
            ItemID = id;
            Quantity = qty;
        }
    }


