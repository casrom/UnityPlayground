using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float sprintSpeed = 24f;
    public float jumpHeight = 2f;
    public float gravity = -50f;
    public bool flyMode = false;

    public Transform groundCheck;
    public float groundCheckRadius = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;


    private Vector2 m_Move;
    public bool m_Sprint;
    private bool m_Descend;
    private bool m_Jump;

    //AUDIO
    AudioSource footsteps_indoor;


    public void OnMove(InputAction.CallbackContext context) {
        m_Move = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context) {
        m_Sprint = context.ReadValue<float>() == 1;
//        Debug.Log(context);
    }

    public void OnJump(InputAction.CallbackContext context) {
        m_Jump = context.ReadValue<float>() == 1;
        //        Debug.Log(context);
    }

    public void OnDescend(InputAction.CallbackContext context) {
        m_Descend = context.ReadValue<float>() == 1;
        //        Debug.Log(context);
    }

    // Start is called before the first frame update
    void Start()
    {
        footsteps_indoor = transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        float x = m_Move.x;
        float z = m_Move.y;

        Vector3 move = transform.right * x + transform.forward * z;

        //if (move!= Vector3.zero && !footsteps_indoor.isPlaying) {
        //    footsteps_indoor.Play();
        //    Debug.Log("Played");
        //} else if (move == Vector3.zero && footsteps_indoor.isPlaying) {
        //    footsteps_indoor.Stop();
        //    Debug.Log("Stopped");
        //}
        





        if (m_Jump) {
            if (flyMode) {
                move.y = 1;
                velocity.y = 0;
            }else if (isGrounded)velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (m_Descend && flyMode) {
            move.y = -1;
        }

        if (m_Sprint) {
            controller.Move(move * sprintSpeed * Time.deltaTime);
        } else {
            controller.Move(move * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }


    // this script pushes all rigidbodies that the character touches
    float pushPower = 2.0f;
    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic) {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3) {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }

}
