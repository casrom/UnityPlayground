using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public InventoryDisplay playerInventory;
    public ItemInfo info;
    public Rigidbody itemRB;

    // Start is called before the first frame update
    void Start()
    {
        //playerInventory.Clear();
        displayText = info.itemName;
        playerInventory = GameObject.FindGameObjectWithTag("InventoryDisplay").GetComponent<InventoryDisplay>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Action() {
        if(playerInventory.AddItem(this)) {
            //transform.gameObject.SetActive(false);
        } else {
            StartCoroutine(InventoryFullMessage());
        }
        //deactivate, or move it to player inventory??
    }

    IEnumerator InventoryFullMessage() {
        displayText = "Inventory Full";
        yield return new WaitForSeconds(2);
        displayText = info.itemName;
    }
}
