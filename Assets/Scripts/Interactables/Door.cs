using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable {
    // Start is called before the first frame update\

    public Transform door;
    public Transform handle;
    public bool autoClose = true;
    public bool open = false;
    bool inAction = false;
    float stepSize = 3f;

    private Movement playerMovement;

    public AudioSource openAudio;
    public AudioSource closeAudio;

    //[HideInInspector]
    public HashSet<Room> rooms = new HashSet<Room>();
    RoomsManager roomsManager; 

    void Start()
    {
        roomsManager = GameObject.FindGameObjectWithTag("RoomsManager").GetComponent<RoomsManager>();
        playerMovement = Camera.main.transform.parent.GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Action() {
        if (!inAction) {
            StopAllCoroutines();
            if (open) {
                StartCoroutine(CloseDoor(3));
            } else {
                if (playerMovement.m_Sprint) {
                    StartCoroutine(OpenDoor(10));
                } else {
                    StartCoroutine(OpenDoor(3));
                }
            }
        }

        foreach(Room room in rooms) {
            room.lastObserved = Time.time + 4;
        }
    }


    IEnumerator CloseDoor(int stepSize) {
        inAction = true;
        for (float i = 0; i < 90; i += stepSize) {
            door.Rotate(0, 0, stepSize);

            if (i == 75) closeAudio.Play();
            yield return null;
        }

        foreach (Room room in rooms) {
            if (room != roomsManager.activeRoom) {
                room.lastObserved = 0f;
            }
        }

        open = false;
        inAction = false;
    }


    IEnumerator OpenDoor(int stepSize) {
        openAudio.time = 0.4f;
        openAudio.Play();
        inAction = true;

        for (float i = 0; i < 90; i += stepSize) {
            door.Rotate(0, 0, -stepSize);
            yield return null;
        }
        open = true;
        inAction = false;
        if (autoClose) {
            yield return new WaitForSeconds(2);
            StartCoroutine(CloseDoor(3));
        }
    }

}
