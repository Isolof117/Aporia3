using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Globals
    [Header("Movement Variables")]

    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float jumpHeight = 3.0f;

    [Header("Gravity")]

    [SerializeField] private float gravity = -9.81f * 2;

    [Header("Ground Collision")]

    [SerializeField] private Transform groundCheck; //The player transform which provides collision with the floor
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Death Screen")]
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToShow;



    private bool isAlive = true;

    private bool isGrounded;
    private Vector3 velocity;

    //References

    private CharacterController controller;

    public Health healthScript;
    public Canvas deathCanvas;
    public WeaponBase weaponScript;
   
 
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        healthScript.OnDeath += HandlePlayerDeath;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            //Check if Player is touching the ground

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

            ResetGravity(); //(STOPS THE PLAYER FROM CONTINUOUSLY BEING DRAGGED DOWN WHEN ON GROUND)

            PlayerMove();  //Move player (x and z)

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                //Jump

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            }

            //Apply Gravity to player

            if (velocity.y < 0)
            {
                //Fall faster
                velocity.y += gravity * 1.5f * Time.deltaTime;
            }
            else
            {
                //Rise Slower
                velocity.y += gravity * Time.deltaTime;
            }


            //Execute Jump

            controller.Move(velocity * Time.deltaTime);
        }
     
    }


    void PlayerMove()
    {

        //Get Movement Inputs from Keyboard

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        //Create Movement Vector

        Vector3 movement = transform.right * x + transform.forward * z;

        //Move the Player

        controller.Move(movement * speed * Time.deltaTime);
    }

    void ResetGravity()
    {
        //Reset default velocity (STOPS THE PLAYER FROM CONTINUOUSLY BEING DRAGGED DOWN)
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandlePlayerDeath()
    {
        //Do player Death stuff here - This stuff is temporary

        deathCanvas.gameObject.SetActive(true);

        isAlive = false;

        weaponScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;

        // Swap UI
        if (objectToHide != null)
            objectToHide.SetActive(false);

        if (objectToShow != null)
            objectToShow.SetActive(true);

        // Cursor unlock
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause game
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        healthScript.OnDeath -= HandlePlayerDeath;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        //Visualise Ground Collider

        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

}
