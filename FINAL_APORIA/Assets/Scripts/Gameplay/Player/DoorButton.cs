using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    //Globals

    [SerializeField] private float interactionRange;

    //References 

    [SerializeField] private GameObject door;

    [SerializeField] private Camera playerCam;

    [SerializeField] private Canvas interactCanvas;


    // Update is called once per frame
    void Update()
    {
        //Get the direction the player is looking in
        Ray ray = playerCam.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));

        RaycastHit hit;

        //Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green); //Shows ray

        //Check if player is looking at an object nearby

        if(Physics.Raycast(ray, out hit, interactionRange))
        {
            //Check player is looking at an interactable object
            if(hit.collider.CompareTag("Interactable"))
            {
                //Show Interaction UI
                interactCanvas.gameObject.SetActive(true);

                //Open / Close door
                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.GetComponent<Door>().ToggleDoor();
                }
            }
            else
            {
                //Hide Interact UI
                interactCanvas.gameObject.SetActive(false);
            }

        }
        else
        {
            //Hide Interact UI
            interactCanvas.gameObject.SetActive(false);
        }

    }
}
