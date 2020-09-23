using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour {

    public Inventory inventory;
    public PlayerInput playerInput;
    public Transform inventoryBox;
    RoomsManager roomsManager;

    bool m_inventory;
    private Vector2 m_Move;
    private Vector2 selectedGridPosition;
    private Vector3 activeItemPositionOffset = new Vector3(0, 0, -2f);
    private Vector3 inventoryBoxTargetLocation = Vector3.zero;

    private int heldItemIndex = -1;
    private Vector3 heldItemPosition = new Vector3(0.48f, -0.37f, 0.79f);

    
    Dictionary<int, Vector3> itemLocalPositions;
    Dictionary<Vector2, int> itemGridPositions;
    Dictionary<int, GameObject> itemObjects;
    Vector3 itemLocalPositionAnchor;
    float speed = 4f;

    //Inventory Positions
    Vector3 activePosition = new Vector3(0, -0.04f, 0.22f); //position of inventory while active
    Vector3 inactivePosition = new Vector3(0, -0.23f, 0.22f); //position of inventory while active
    Vector3 targetPosition;

    // Start is called before the first frame update
    void Start() {
        roomsManager = GameObject.FindGameObjectWithTag("RoomsManager").GetComponent<RoomsManager>();

        itemLocalPositions = new Dictionary<int, Vector3>();
        itemGridPositions = new Dictionary<Vector2, int>();
        itemObjects = new Dictionary<int, GameObject>();
        selectedGridPosition = Vector2.zero;
        targetPosition = inactivePosition;
        itemLocalPositionAnchor = new Vector3(-1, 3, -2); //position of the first item

        //Debug.Log(Object.ReferenceEquals(items[0] , items[1]));
    }


    // Update is called once per frame
    void Update() {
        if (m_inventory || heldItemIndex != -1) {
            //Update Item positions mapping
            int i = 0;
            foreach (KeyValuePair<int, ItemInfo> item in inventory.GetItems()) {
                ItemInfo itemInfo = item.Value;
                Vector2 itemGridPosition = new Vector2(i % 2, (i / 2) % 2);
                Vector3 itemLocalPosition = itemLocalPositionAnchor + new Vector3(itemGridPosition.x, -itemGridPosition.y, 0) * 2;
                GameObject itemObject;
                if (!itemObjects.ContainsKey(item.Key)) { //item not instantiated
                    itemObject = Instantiate(itemInfo.prefab, transform);
                    itemObject.transform.localPosition = itemLocalPosition;
                    itemObject.transform.localScale = itemInfo.displayScale;
                    itemObjects.Add(item.Key, itemObject);
                    itemGridPositions.Add(itemGridPosition, item.Key);
                    itemLocalPositions.Add(item.Key, itemLocalPosition);
                    Debug.LogWarning("Item Object was not initialized!");
                }
                //itemObject = itemObjects[itemInfo];
                if (!itemGridPositions.ContainsKey(itemGridPosition)) {
                    itemGridPositions.Add(itemGridPosition, item.Key);
                    itemLocalPositions[item.Key] = itemLocalPosition;
                } else {
                    itemGridPositions[itemGridPosition] = item.Key;
                }
                if (!itemLocalPositions.ContainsKey(item.Key))
                    itemLocalPositions[item.Key] = itemLocalPosition;

                i++;

            }

            //move items to targe location
            foreach (KeyValuePair<int, ItemInfo> item in inventory.GetItems()) {
                Vector3 targPos = itemLocalPositions[item.Key];
                Vector3 itemTargetScale = item.Value.displayScale;

                if (itemGridPositions.ContainsKey(selectedGridPosition)) {
                    //Debug.Log(selectedGridPosition);
                    int actiItemIdx = itemGridPositions[selectedGridPosition];
                    if(item.Key == actiItemIdx)
                        targPos += activeItemPositionOffset;
                }
                if (heldItemIndex == item.Key) {
                    targPos = heldItemPosition;
                    itemTargetScale = Vector3.one;
                }
                Vector3 currPos = itemObjects[item.Key].transform.localPosition;
                Transform objectTransform = itemObjects[item.Key].transform;
                objectTransform.localPosition = Vector3.Lerp(currPos, targPos, Time.deltaTime * speed);
                objectTransform.localScale = Vector3.Lerp(objectTransform.localScale, itemTargetScale, Time.deltaTime * speed);
            }
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
        //inventoryBox.localPosition = Vector3.Lerp(inventoryBox.localPosition, inventoryBoxTargetLocation, Time.deltaTime * speed);

    }

    //public IEnumerator InventoryBoxBacking() {
    //    inventoryBoxTargetLocation = Vector3.forward * 5;
    //    yield return new WaitForSeconds(1);
    //    inventoryBoxTargetLocation = Vector3.zero;
    //}
    public IEnumerator ItemBack(int index) {
        speed *= 2;
        Vector3 originalPos = itemLocalPositions[index];
        itemLocalPositions[index] = new Vector3(0, 2, -7);
        yield return new WaitForSeconds(.5f);
        speed /= 2;
        itemLocalPositions[index] = originalPos;

    }

    public IEnumerator ToggleInventory() {
        if (m_inventory) {  //Deactivate
            targetPosition = inactivePosition;
            yield return null; //need this to properly complete current action
            playerInput.SwitchCurrentActionMap("Player");

        } else {            //Activate
            if (heldItemIndex != -1) {
                //holding something, put it back
                //StartCoroutine(InventoryBoxBacking());
                StartCoroutine(ItemBack(heldItemIndex));
                itemObjects[heldItemIndex].transform.parent = transform;
                heldItemIndex = -1;
            }
            targetPosition = activePosition;
            yield return null;
            playerInput.SwitchCurrentActionMap("UI");
        }
        m_inventory = !m_inventory;
    }


    public void OnToggle(InputAction.CallbackContext context) {
        //Debug.Log(context.phase);
        if (context.phase == InputActionPhase.Started) {
            StartCoroutine(ToggleInventory());
        }
    }

    public void OnNavigate(InputAction.CallbackContext context) {
        m_Move = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Started) {
            Vector2 newSelected = selectedGridPosition;
            newSelected.x = (m_Move.x + selectedGridPosition.x + 2) % 2;
            newSelected.y = (m_Move.y + selectedGridPosition.y + 2) % 2;
            if (itemGridPositions.ContainsKey(newSelected)) {
                selectedGridPosition = newSelected;
            }

//            Debug.Log(selectedGridPosition);
        }

    }

    public void OnSelect(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            if(itemGridPositions.ContainsKey(selectedGridPosition)) {
                heldItemIndex = itemGridPositions[selectedGridPosition];
                itemObjects[heldItemIndex].transform.parent = transform.parent;
                StartCoroutine(ToggleInventory());
            }
        }
    }

    public void OnDrop(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            if (heldItemIndex != -1) {
                itemObjects[heldItemIndex].transform.parent = roomsManager.activeRoom.transform;
                Rigidbody rb = itemObjects[heldItemIndex].GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.velocity = Camera.main.transform.forward * 10;

                inventory.RemoveItem(heldItemIndex);
                itemObjects.Remove(heldItemIndex);
                itemLocalPositions.Remove(heldItemIndex);
                itemGridPositions.Clear();
                selectedGridPosition = Vector2.zero;

                heldItemIndex = -1;
            }

        }
    }

    public bool AddItem(Item item) {
        if(heldItemIndex != -1) {
            itemObjects[heldItemIndex].transform.parent = transform;
            heldItemIndex = -1;
        }
        int itemID = inventory.AddItem(item.info);
        if (itemID != -1) {
            item.gameObject.transform.parent = transform.parent;
            item.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            itemObjects.Add(itemID, item.gameObject);
            heldItemIndex = itemID;
            return true;
        } else {
            return false;
        }
        
    }
}
