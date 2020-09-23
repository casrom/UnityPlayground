using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    Renderer screen;
    Camera playerCam;
    public Camera portalCam;
    RenderTexture viewTexture;


    void Awake() {
        playerCam = Camera.main;
        //portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    void CreateViewTexture() {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if(viewTexture!=null) {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

            portalCam.targetTexture = viewTexture;
            //linkedPortal.screen.material.SetColor("_UnlitColor", Color.red);
            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    //called before player cam rendered
    public void Render() {
        screen.enabled = false;
        CreateViewTexture();

        //Position portal cam
        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;

        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        portalCam.Render();

        screen.enabled = true;

    }


    void Start()
    {
        screen = GetComponent<Renderer>();
        //linkedPortal.screen.material.SetColor("_UnlitColor", Color.red);

        //Debug.Log(screen.material);
    }

    void Update()
    {
        Render();
    }
}
