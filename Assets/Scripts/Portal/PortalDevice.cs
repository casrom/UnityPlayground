using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PortalDevice : MonoBehaviour
{

    public PortalSubDevice[] subDevices;
    public Transform portalCube;
    public Transform portalDeviceStand;

    public float subDeviceSize = .2f;
    public float portalSize = 5f;

    public float speed = 1f;
    Vector3 targetScale;
    Vector3 targetLocation;
    Quaternion targetRotation;

    void Awake()
    {

    }


    public void InitializeSubDevices()
    {
        subDevices = new PortalSubDevice[8];
        for (int i = 0; i < 8; i++)
        {
            Transform child = transform.GetChild(i);
            //float x = (i & (1 << 0)) == 1 ? -subDeviceSize / 2 : subDeviceSize / 2;
            //float y = (i & (1 << 0)) == 1 ? -subDeviceSize / 2 : subDeviceSize / 2;
            //float z = (i & (1 << 0)) == 1 ? -subDeviceSize / 2 : subDeviceSize / 2;
            PortalSubDevice subDevice = child.gameObject.GetComponent<PortalSubDevice>();
            if (subDevice == null)
                subDevice = child.gameObject.AddComponent<PortalSubDevice>() as PortalSubDevice;
            subDevices[i] = subDevice;
            subDevices[i].GetComponent<PortalSubDevice>().SetTargetLocation(PortalDevice.cornerTable[i] * subDeviceSize/2);
        }
    }

    public void MoveSubDevices()
    {
        foreach (PortalSubDevice s in subDevices) s.MoveToTargeLocation();
    }

    public void Expand() {
        for (int i = 0; i < 8; i++) {
            subDevices[i].SetTargetLocation(PortalDevice.cornerTable[i] * portalSize / 2);
            subDevices[i].MoveToTargeLocation();
        }

        targetScale = Vector3.one * portalSize;
        StartCoroutine(PortalCubeCoroutine());

    }

    public void Contract() {
        for (int i = 0; i < 8; i++) {
            subDevices[i].SetTargetLocation(PortalDevice.cornerTable[i] * subDeviceSize / 2);
            subDevices[i].MoveToTargeLocation();
        }

        targetScale = Vector3.one * subDeviceSize/2;
        StartCoroutine(PortalCubeCoroutine());
    }

    public IEnumerator PortalCubeCoroutine() {
        while (portalCube.localScale != targetScale) {
            portalCube.localScale = Vector3.Lerp(portalCube.localScale, targetScale, speed * Time.deltaTime);
            yield return null;
        }
    }


    IEnumerator MoveCoroutine() {
        while (transform.localPosition != targetLocation) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocation, speed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator RotateCoroutine() {
        while (!transform.rotation.Equals(targetRotation)) {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void ResetLocRot() {
        targetLocation = portalDeviceStand.transform.position + Vector3.up * .4f;
        targetRotation = Quaternion.Euler(35.26f, 0, 45);
        StartCoroutine(MoveCoroutine());
        StartCoroutine(RotateCoroutine());
    }

    public void SetTargetRotation(Vector3 euler) {
        targetRotation = Quaternion.Euler(euler);
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Vector3[] cornerTable =
    {
                             //  xyz
        new Vector3(-1,-1,-1),  //0 000
        new Vector3(-1,-1,1),  //1 001
        new Vector3(-1,1,-1),  //2 010
        new Vector3(-1,1,1),  //3 011
        new Vector3(1,-1,-1),  //4 100
        new Vector3(1,-1,1),  //5 101
        new Vector3(1,1,-1),  //6 110
        new Vector3(1,1,1),  //7 111
    };
}
