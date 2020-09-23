using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory", order = 1)]

public class Inventory : ScriptableObject
{
    private Dictionary<int, ItemInfo> items; //items in the inventory
    public int capacity = 4;

    public int counter;
    
    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Inventory ID of added item. -1 if the inventory is full. </returns>
    public int AddItem(ItemInfo item) {
        if (items == null) items = new Dictionary<int, ItemInfo>();
        if (items.Count == capacity) return -1;
        items.Add(counter, item);
        counter++;
        return counter-1;
    }

    public void RemoveItem(int key) {
        if (items.ContainsKey(key)) {
            items.Remove(key);
        } else {
            Debug.LogWarning("Trying to remove an item from inventory with invalid key!");
        }
    }

    public void Clear() {
        if(items != null)
            items.Clear();
        counter = 0;
    }

    public int Count() {
        return items.Count;
    }

    public Dictionary<int, ItemInfo> GetItems() {
        if (items == null) {
            items = new Dictionary<int, ItemInfo>();
        }
        return items;
    }

}
