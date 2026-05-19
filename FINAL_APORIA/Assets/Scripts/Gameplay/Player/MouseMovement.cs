using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MouseMovement : MonoBehaviour
{
    //Globals

    [SerializeField] private float mouseSensitivity = 100f;


    [SerializeField] private float minMouseYClamp = -80f; //Stop player from breaking their neck
    [SerializeField] private float maxMouseYClamp = 80f; //Stop player from breaking their neck

    private float xRot = 0.0f; 

    //References

    [SerializeField] private Transform playerBody;

    // Start is called before the first frame update
    void Start()
    {
        //Locks cursor to the middle of the screen and makes it invisible

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the Mouse Inputs

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Look Up and Down (ONLY MOVES CAMERA)

        xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, minMouseYClamp, maxMouseYClamp); //clamp x rotation

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f); //apply rotation


        //Look Left and Right (MOVES PLAYER AND CAMERA)

        playerBody.Rotate(Vector3.up * mouseX); 


    }
}
