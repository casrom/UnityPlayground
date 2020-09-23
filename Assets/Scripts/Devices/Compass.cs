using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Compass : MonoBehaviour {
    public Transform pointer;
    public Transform marker;
    public Transform outerRing;
    public Transform core;
    public Material pointerMaterial;
    public float speed = 2f;

    public GameObject doorPointerPrefab;
    private GameObjectPool doorPointerPool;
    private List<GameObject> doorPointers;
    

    public RoomsManager roomsManager;
    public TimeManager timeManager;

    [ColorUsage(true, true)]
    public Color enabledColor;
    [ColorUsage(true, true)]
    public Color disabledColor;
    [ColorUsage(true, true)]
    public Color dangerColor;
    [ColorUsage(true, true)]
    public Color clockColor;

    public Vector3 enableLocation = new Vector3(0, -0.117f, 0.35f);
    public Vector3 disbleLocation = new Vector3(0, -0.36f, 0.35f);

    bool active = false;
    float m_toggleCompass;
    float markerSpeed;
    float markerTargetSpeed;

    bool clockMode = false;


    Quaternion defaultPointerRot, defaultMarkerRot;
    Quaternion targetPointerRot, targetMarkerRot;
    Color targetColor;
    // Start is called before the first frame update
    void Start() {
        defaultPointerRot = pointer.rotation;
        defaultMarkerRot = marker.rotation;

        targetPointerRot = defaultPointerRot;
        targetMarkerRot = defaultMarkerRot;

        targetColor = disabledColor;
        //disable
        transform.localPosition = disbleLocation;
        pointerMaterial.SetColor("_EmissiveColor", disabledColor);

        //DoorPointers
        doorPointerPrefab.SetActive(false);
        doorPointerPool = new GameObjectPool();
        for (int i = 0; i < 10; i++) doorPointerPool.AddToPool(Instantiate(doorPointerPrefab, transform));
        doorPointers = new List<GameObject>();
    }

    int maxDepth = 5;
    float velocity = 0;

    // Update is called once per frame
    void Update() {
        //Debug.Log(clockMode);
        Dictionary<DoorAnchor,Door> activeRoomDoors = roomsManager.activeRoom.doors;
        while (activeRoomDoors.Count != doorPointers.Count) {
            if (doorPointers.Count < activeRoomDoors.Count) {
                doorPointers.Add(doorPointerPool.DePool());
            } else {
                doorPointerPool.Pool(doorPointers[doorPointers.Count - 1]);
                doorPointers.RemoveAt(doorPointers.Count - 1);
            }
        }
        int i = 0;
        foreach (KeyValuePair<DoorAnchor, Door> door in activeRoomDoors) {
            Quaternion rotation = Quaternion.LookRotation(door.Value.transform.position + Vector3.up * 2 - Camera.main.transform.position);
            GameObject doorPointer = doorPointers[i];
            doorPointer.transform.rotation = Quaternion.Lerp(doorPointer.transform.rotation, rotation, Time.deltaTime * speed);
            i++;
        }


        if (active) {
            targetColor = clockColor;
            targetPointerRot = Quaternion.LookRotation(Vector3.forward);
            targetMarkerRot = Quaternion.LookRotation(Vector3.up);
            markerTargetSpeed = (maxDepth - (roomsManager.returnProgress - 1)) * 50;

            if (clockMode) {
                targetPointerRot = Quaternion.LookRotation(Camera.main.transform.up, -Camera.main.transform.position + transform.position) * Quaternion.Euler(0, -timeManager.window * 360 / 12, 0);
                targetMarkerRot = Quaternion.LookRotation(Camera.main.transform.up, -Camera.main.transform.position + transform.position);
                marker.localRotation = Quaternion.Lerp(marker.localRotation, Quaternion.identity, Time.deltaTime * speed);

            } else if (roomsManager.returnMode) {
                targetColor = dangerColor;

                speed = 10f;
                if (roomsManager.returnDoorIndex == -1) {
                    targetPointerRot = Quaternion.LookRotation(Vector3.up, Camera.main.transform.forward);
                } else {
                    if (roomsManager.returnDoorIndex < roomsManager.activeRoom.info.doorAnchors.Length) {
                        Transform returnDoor = roomsManager.activeRoom.doors[roomsManager.activeRoom.info.doorAnchors[roomsManager.returnDoorIndex]].transform;
                        //Debug.Log(returnDoor.position);
                        targetPointerRot = Quaternion.LookRotation(returnDoor.position + Vector3.up * 2 + returnDoor.forward * 1 - transform.position);
                    } else {
                        Debug.LogWarning("Return door anchor index out of range. Index: " + roomsManager.returnDoorIndex);
                    }
                }

                marker.localRotation = Quaternion.Euler(0, 0, marker.localRotation.eulerAngles.z + Time.deltaTime * markerSpeed);

            } else {
                marker.localRotation = Quaternion.Euler(0, marker.localRotation.eulerAngles.y + Time.deltaTime * markerSpeed, 0);
                markerTargetSpeed = (maxDepth - (roomsManager.depth - 1)) * 50;
                targetColor = enabledColor;
            }



        } else { //not active, reset

            targetPointerRot = Quaternion.LookRotation(Camera.main.transform.up, -Camera.main.transform.position + transform.position);
            targetMarkerRot = Quaternion.LookRotation(Camera.main.transform.up, -Camera.main.transform.position + transform.position);
            marker.localRotation = Quaternion.Lerp(marker.localRotation, Quaternion.Euler(0, 0, 0), speed);

        }
        //always

        core.rotation = Quaternion.Lerp(core.rotation, targetMarkerRot, Time.deltaTime * speed);
        pointer.rotation = Quaternion.Lerp(pointer.rotation, targetPointerRot, Time.deltaTime * speed);
        outerRing.rotation = Quaternion.Lerp(outerRing.rotation, pointer.rotation, Time.deltaTime * speed);

        //change color
        Vector4 color = pointerMaterial.GetColor("_EmissiveColor");
        pointerMaterial.SetColor("_EmissiveColor", Vector4.Lerp(color, targetColor, Time.deltaTime * speed*2));

        //interpolate markerspeed
        markerSpeed = Mathf.SmoothDamp(markerSpeed, markerTargetSpeed, ref velocity, 0.3f);

        //Debug.Log(markerTargetSpeed);
    }

    IEnumerator ColorCoroutine(Color targetColor) {
        if (pointerMaterial.GetColor("_EmissiveColor") != targetColor) {
            for (int i = 0; i < 200; i++) {
                Vector4 color = pointerMaterial.GetColor("_EmissiveColor");
                pointerMaterial.SetColor("_EmissiveColor", Vector4.Lerp(color, targetColor, Time.deltaTime * 2));
                yield return null;
            }
        }
    }

    IEnumerator EnableCoroutine() {
        //transform.gameObject.SetActive(true);
        //Move UP
        while (transform.localPosition != enableLocation) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, enableLocation, Time.deltaTime * 5);
            if ((transform.localPosition - enableLocation).magnitude < 0.001f) {
                transform.localPosition = enableLocation;
            }
            yield return null;
        }
        targetColor = enabledColor;
        yield return new WaitForSeconds(.5f);

        active = true;

    }

    IEnumerator DisableCoroutine() {
        //targetPointerRot = defaultPointerRot;
        //targetMarkerRot = defaultMarkerRot;
        markerTargetSpeed = 0;
        speed = 10f;
        targetColor = disabledColor;
        //for (int i = 0; i < 40; i++) {
        //    Vector4 color = pointerMaterial.GetColor("_EmissiveColor");
        //    pointerMaterial.SetColor("_EmissiveColor", Vector4.Lerp(color, disabledColor, Time.deltaTime * 5));
        //    yield return null;
        //}
        //pointerMaterial.SetColor("_EmissiveColor", disabledColor);
        yield return new WaitForSeconds(.5f);
        speed = 2f;
        //Move Down
        while (transform.localPosition != disbleLocation) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, disbleLocation, Time.deltaTime * 2);
            if ((transform.localPosition - disbleLocation).magnitude < 0.001f) {
                transform.localPosition = disbleLocation;
            }
            yield return null;
        }

        //transform.gameObject.SetActive(false);

    }
    public void OnToggleCompass(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            //Debug.Log("TOGGLE");
            StopAllCoroutines();
            if (active) {
                active = false;
                StartCoroutine(DisableCoroutine());
            } else {
                StartCoroutine(EnableCoroutine());
            }

        }
    }

    public void OnLeftClick(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            clockMode = true;
            speed = 4f;
        } else if (context.phase == InputActionPhase.Canceled) {
            clockMode = false;
            speed = 2f;
        }
    }
}
