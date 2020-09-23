using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float maxSpeed = .1f;

    public float reach = 5f;

    public float poleReach = 5f;

    public BoxCollider triggerCollider;

    
    public Transform pole;

    protected Vector3 targetPosition, defaultPosition, polePosition, defaultPolePosition, rootPos;

    protected Quaternion targetRot, defaultRot, playerTargetRot, roomRot;

    float targetSpeed, speed;

    bool trigger;

    MouseLook look;

    Transform playerCam, player;
    void Start()
    {
        playerCam = Camera.main.transform;
        player = playerCam.parent;
        defaultPosition = transform.position + rootPos;
        defaultRot = transform.rotation;
        defaultPolePosition = pole.position + rootPos;
        look = playerCam.GetComponent<MouseLook>();

        targetSpeed = maxSpeed;
        rootPos = transform.parent.position;

    }

    float velocity = 0f;
    // Update is called once per frame
    void Update()
    {
        //Ray r = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        //RaycastHit hit = new RaycastHit();
        //Physics.Raycast(r, out hit);
        //if  (triggeCollider && hit.collider == triggeCollider) {
        //    targetPosition = (Camera.main.transform.position - rootPos).normalized * reach;
        //} else {
        //    targetPosition = defaultPosition;
        //}
        trigger = triggerCollider.bounds.Contains(playerCam.position);

        if (trigger) {

            targetPosition = (Camera.main.transform.position - rootPos).normalized * reach + Vector3.up * 2 + rootPos;
            targetRot = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            polePosition = -(Camera.main.transform.position - rootPos).normalized * poleReach + Vector3.up * 8 + rootPos;

            
            //look.m_Rotation.x = playerTargetRot.eulerAngles.x;

            var angle = Vector3.SignedAngle(playerCam.forward, transform.forward, playerCam.right);
            if (angle * angle > 1)
                look.Look(new Vector2(0, -angle / 100f));
            //Debug.Log(angle);

            //playerCam.localRotation = Quaternion.Lerp(playerCam.localRotation, playerTargetRot, Time.deltaTime * 0.1f);
            playerTargetRot = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            playerTargetRot = Quaternion.Euler(0, playerTargetRot.eulerAngles.y, 0);

            player.rotation = Quaternion.Lerp(player.rotation, playerTargetRot, Time.deltaTime * 0.1f);

        } else {
            pole.position = defaultPolePosition;
            targetPosition = defaultPosition;
            targetRot = defaultRot;
        }

        pole.position = Vector3.Lerp(pole.position, polePosition, Time.deltaTime * speed*5);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * speed * 2);
        speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocity, 0.0003f);

    }
}
