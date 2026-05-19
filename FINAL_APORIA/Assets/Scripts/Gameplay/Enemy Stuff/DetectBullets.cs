using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBullets : MonoBehaviour
{


    //Script to detect nearny bullets and cause enemy to aggro

    private EnemyMovement enemyScript;
    private void Start()
    {
        enemyScript = GetComponentInParent<EnemyMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            enemyScript.SetState(EnemyMovement.EnemyState.Attack);
        }
    }

}
