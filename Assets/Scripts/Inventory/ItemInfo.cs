using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "Inventory/ItemInfo", order = 2)]
public class ItemInfo : ScriptableObject
{
    public string itemName = "Item Name";
    public GameObject prefab;
    public Vector3 displayScale = Vector3.one;
}
