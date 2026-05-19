using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //Globals

    [SerializeField] private float doorSpeed = 2.0f;

    [SerializeField] private float doorHeight = 3.0f;

    private Vector3 upperClosedPos;
    private Vector3 lowerClosedPos;

    private Vector3 upperOpenPos;
    private Vector3 lowerOpenPos;

    bool isOpen = false;
    bool doorMoving = false;
    //References

    [SerializeField] private GameObject upperDoorPart;
    [SerializeField] private GameObject lowerDoorPart;

    private void Start()
    {
        //Calculate Open and Closed heights for each door part
        upperClosedPos = upperDoorPart.transform.position;
        lowerClosedPos = lowerDoorPart.transform.position;

        upperOpenPos = new Vector3(upperClosedPos.x, upperClosedPos.y + doorHeight, upperClosedPos.z);
        lowerOpenPos = new Vector3(lowerClosedPos.x, lowerClosedPos.y - doorHeight, lowerClosedPos.z);

    }



    public void ToggleDoor()
    {
        if (!doorMoving) //Make sure door is not already is the process of being opened /closed
        {
            if (Input.GetKeyDown(KeyCode.E) && !isOpen)
            {
                //Open Door
                StartCoroutine(OpenDoor());

            }
            else if (Input.GetKeyDown(KeyCode.E) && isOpen)
            {
                //Close Door
                StartCoroutine(CloseDoor());

            }
        }
    }

    IEnumerator CloseDoor()
    {
        doorMoving = true;
        float time = 0;

        //Linearly interpolate between open and closed postitions for each door part

        while (Math.Abs(upperDoorPart.transform.position.y - upperClosedPos.y) > 0) 
        {
            time += Time.deltaTime * doorSpeed;

            upperDoorPart.transform.position = Vector3.Lerp(upperOpenPos, upperClosedPos, time);
            lowerDoorPart.transform.position = Vector3.Lerp(lowerOpenPos, lowerClosedPos, time);

            yield return new WaitForEndOfFrame();
        }

        doorMoving = false;
        isOpen = false;
    }

    IEnumerator  OpenDoor()
    {
        doorMoving = true;
        float time = 0;

        //Linearly interpolate between closed and open postitions for each door part

        while (Math.Abs(upperDoorPart.transform.position.y - upperOpenPos.y) > 0)
        {
            time += Time.deltaTime * doorSpeed;
            
            upperDoorPart.transform.position = Vector3.Lerp(upperClosedPos, upperOpenPos,time);
            lowerDoorPart.transform.position = Vector3.Lerp(lowerClosedPos, lowerOpenPos,time);

            yield return new WaitForEndOfFrame();
        }

        doorMoving = false;
        isOpen = true;
       
    }
}
