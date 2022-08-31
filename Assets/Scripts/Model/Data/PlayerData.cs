using UnityEngine;
using System;
namespace MainNameSpace.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private InventoryData _inventory;
        //public int Coins;
        public int Health;
        //public bool IsArmed;
        public InventoryData Inventory => _inventory;

        public PlayerData Clone()
        {
            var json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
}

