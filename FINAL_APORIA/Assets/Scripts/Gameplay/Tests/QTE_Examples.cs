using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTE_Examples : MonoBehaviour
{
    [SerializeField] private Canvas QTECanvas;
    
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            LoadQTE();
        }
    }

    private void LoadQTE()
    {
        QTECanvas.gameObject.SetActive(true);
    }
}
