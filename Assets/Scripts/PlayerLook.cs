using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController = null;

    [SerializeField]
    Transform shoulderLeft = null;
    [SerializeField]
    Transform shoulderRight = null;

    [SerializeField]
    float mouseSensitivity = 100f;

    [SerializeField]
    Vector2 mousePosition = Vector2.zero;

    float mouseVerticalRotation = 0f;
    [SerializeField]
    float maxRotation = 90f;
    [SerializeField]
    float minRotation = 75f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.Dead)
            return;

        MouseLook();
        AlignArms();
    }

    void MouseLook()
    {
        mousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * Time.deltaTime;

        playerController.transform.Rotate(Vector3.up * mousePosition.x);

        mouseVerticalRotation = Mathf.Max(maxRotation, Mathf.Min(minRotation, mouseVerticalRotation - mousePosition.y));
        transform.localRotation = Quaternion.Euler(mouseVerticalRotation, 0f, 0f);
    }

    void AlignArms()
    {
        shoulderRight.localRotation = transform.localRotation;
        shoulderLeft.localRotation = transform.localRotation;
    }
}
