using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Monitor_config_panel : Interactable {

    public PlayerInput input;

    public PortalDevice device;

    public Transform displayCube;
    public TextMeshProUGUI text;

    public TextMeshProUGUI UIText;
    public Interact interact;

    // Start is called before the first frame update
    void Start()
    {
        rotation = new Vector3();
    }


    int activeItemIdx = 0;
    int totalItems = 4;
    Vector3 rotation;


    float throttleV = .2f; //in seconds
    float throttleH = .1f; //in seconds


    void Update()
    {

        //format string
        string display = "";
        string prefix = " ";
        string postfix = " ";
        if(activeItemIdx == 0) {
            prefix = "<";
            postfix = ">";
        }
        display += prefix + "euler X  " + rotation.x + postfix + "\n"; prefix = " "; postfix = " ";
        if (activeItemIdx == 1) {
            prefix = "<";
            postfix = ">";
        }
        display += prefix + "euler y  " + rotation.y + postfix + "\n"; prefix = " "; postfix = " ";
        if (activeItemIdx == 2) {
            prefix = "<";
            postfix = ">";
        }
        display += prefix + "euler Z  " + rotation.z + postfix + "\n"; prefix = " "; postfix = " ";
        if (activeItemIdx == 3) {
            prefix = "<";
            postfix = ">";
        }
        display += prefix + "  commence  " + postfix; prefix = " "; postfix = " ";
        text.SetText(display);

        displayCube.rotation = Quaternion.Euler(rotation);
    }

    public override void Action() {
        input.SwitchCurrentActionMap("UI");
        device.SetTargetRotation(rotation);
        StartCoroutine(device.RotateCoroutine());
        switched = false;

    }

    private Vector2 m_Nav;

    

    public void OnNavigate(InputAction.CallbackContext context) {
        m_Nav = context.ReadValue<Vector2>();

        switch (context.phase) {
            //case InputActionPhase.Performed:
            //    if (context.interaction is SlowTapInteraction) {
            //        scroll = true;
            //    } else {
            //        scroll = false;
            //    }
            //    break;
            case InputActionPhase.Performed:
                
                break;

            case InputActionPhase.Started:
                if (m_Nav.x == 0) {
                    verticalNav = true;
                    StartCoroutine(VerticalSelectCoroutine());
                } else if (m_Nav.y == 0) {
                    horizontalNav = true;
                    StartCoroutine(HorizontalValueCoroutine());
                }

                break;

            case InputActionPhase.Canceled:
                StopCoroutine(VerticalSelectCoroutine());
                StopCoroutine(HorizontalValueCoroutine());
                verticalNav = false;
                horizontalNav = false;
                throttleH = 0.2f;
                break;
        }

    }

    bool horizontalNav = false;
    bool verticalNav = false;

    IEnumerator VerticalSelectCoroutine() {
        while(verticalNav) {
            activeItemIdx -= (int)m_Nav.y; //vertical movement
            activeItemIdx = (activeItemIdx % totalItems + totalItems) % totalItems;
            yield return new WaitForSeconds(throttleV);
        }
    }

    float throttleSpeedUp = 1f;
    float minThrottleH = 0.001f;

    IEnumerator HorizontalValueCoroutine() {
        while (horizontalNav) {
            throttleH -= Time.deltaTime * throttleSpeedUp;
            if (throttleH < minThrottleH) throttleH = minThrottleH;
            if (activeItemIdx != 3) {
                rotation[activeItemIdx] += m_Nav.x;
                rotation[activeItemIdx] = (rotation[activeItemIdx] % 360 + 360) % 360;

                device.SetTargetRotation(rotation);
                StartCoroutine(device.RotateCoroutine());

            }
            yield return new WaitForSeconds(throttleH);
        }
    }

    bool switched = false;
    public void OnExit(InputAction.CallbackContext context) {
        SwitchToPlayerActionMap();
    }


    public void OnSubmit(InputAction.CallbackContext context) {
        SwitchToPlayerActionMap();
    }

    void SwitchToPlayerActionMap() {
        if (!switched) {
            switched = true;
            input.SwitchCurrentActionMap(input.defaultActionMap);
        }
        interact.interacting = false;

    }

}
