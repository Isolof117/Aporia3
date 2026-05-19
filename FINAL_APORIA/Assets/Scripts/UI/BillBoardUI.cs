using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardUI : MonoBehaviour
{
    //Attach this to a world-space Canvas to make it always face the player
    private Camera cam;
    
    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
