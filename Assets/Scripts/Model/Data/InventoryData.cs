using UnityEngine;
using System;
using System.Collections.Generic;
using MainNameSpace.Model.Definitions;

namespace MainNameSpace.Model.Data
{
    [Serializable]
    public class InventoryData 
    {
        [SerializeField] private List<InventoryItemData> _inventory = new List<InventoryItemData>();

        public delegate void OnInventoryChanged(string id, int value);
        //----with
        public OnInventoryChanged OnChanged;
        //----or
        //public Action<string, int> OnChanged;

        public void Add(string id, int value)
        {
            if (value <= 0) return;

            var itemDef = DefsFacade.I.Items.Get(id);
            if (itemDef.IsVoid) return;

            var item = GetItem(id);
            if(item == null)
            {
                item = new InventoryItemData(id);
                _inventory.Add(item);
            }
            item.Value += value;
            OnChanged?.Invoke(id, Count(id));
        }

        public void Remove(string id, int value)
        {
            var itemDef = DefsFacade.I.Items.Get(id);
            if (itemDef.IsVoid) return;

            var item = GetItem(id);
            if (item == null) return;

            item.Value -= value;
            if (item.Value <= 0) _inventory.Remove(item);
            OnChanged?.Invoke(id, Count(id));
        }

        private InventoryItemData GetItem(string id)
        {
            foreach (var itemdata in _inventory)
            {
                if (itemdata.Id == id)
                    return itemdata;
            }
            return null;
        }

        public int Count(string id)
        {
            var count = 0;
            foreach (var item in _inventory)
            {
                if (item.Id == id) count += item.Value;
            }
            return count;
        }
    }

    [Serializable]
    partial class InventoryItemData
    {
        public string Id;
        public int Value;

        public InventoryItemData(string id)
        {
            Id = id;
        }
    }
}
