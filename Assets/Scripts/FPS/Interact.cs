using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class Interact : MonoBehaviour
{
    public PlayerInput playerInput;
    int interactMask = 1 << 10;
    public float range = 10f;
    public GameObject messageObject;
    TextMeshProUGUI message;
    public GameObject crosshairObject;
    Image crosshair;

    bool messageDisplayed = false;
    bool crosshairActive = false;
    public Color defaultCrosshair;
    public Color activeCrosshair;


    float m_interact;
    public bool interacting = false;

    public void OnInteract(InputAction.CallbackContext context) {
        m_interact = context.ReadValue<float>();
        if (context.phase == InputActionPhase.Started) {
            if (crosshairActive) { //Interactable
                entity.Action();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        message = messageObject.GetComponent<TextMeshProUGUI>();
        message.SetText("");

        crosshair = crosshairObject.GetComponent<Image>();
    }

    Interactable entity;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;


        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, range, interactMask)) {
            //Hit an interactable object
            //Debug.Log("HIT");
            entity = hit.transform.GetComponent<Interactable>();
            if (entity != null && entity.displayText != null) {
                message.SetText(entity.displayText);
                messageDisplayed = true;
            }

            if (entity != null && entity.isInteractable) {
                crosshair.color = activeCrosshair;
                crosshairActive = true;

                //if (m_interact == 1) {
                //    entity.Action();
                //    //interacting = true;
                //}
            }


        } else { //hit nothing, reset
            if (messageDisplayed) {
                messageDisplayed = false;
                message.SetText("");
            }

            if (crosshairActive) {
                crosshair.color = defaultCrosshair;
                crosshairActive = false;
            }
        }

        if (interacting) {
            message.SetText("");
            crosshair.enabled = false;
        } else {
            crosshair.enabled = true;
        }

        if(playerInput.currentActionMap.name == "UI") {
            crosshair.enabled = false;
        }
        
    }

}
