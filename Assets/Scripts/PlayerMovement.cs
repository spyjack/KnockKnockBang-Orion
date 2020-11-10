using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController = null;

    [SerializeField]
    CharacterController characterController = null;

    [SerializeField]
    float speed = 6f;
    [SerializeField]
    float sprintMultiplier = 2f;
    [SerializeField]
    float gravity = 20.0f;

    Vector3 movementDirection = Vector3.zero;
    Vector3 rotateDirection = Vector3.zero;

    [SerializeField]
    bool canMove = true;

    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.Dead)
        playerMovement();
    }

    void playerMovement()
    {
        if (characterController.isGrounded)
        {
            movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            movementDirection.Normalize();
            movementDirection *= speed;

            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                movementDirection *= sprintMultiplier;
            }
        }

        movementDirection.y -= gravity * Time.deltaTime;

        characterController.Move(transform.TransformDirection(movementDirection) * Time.deltaTime);
    }
}
