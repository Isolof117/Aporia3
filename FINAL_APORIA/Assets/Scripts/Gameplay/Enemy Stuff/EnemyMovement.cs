using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    [Header("Patrol variables")]

    [SerializeField] private Transform[] patrolNodes;

    int nodePointer = 0;


    [Header("Player References")]

    [SerializeField] private Transform playerRef;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;



    [Header("Sight Parameters")]

    [SerializeField] private float sightDistance;
    [SerializeField] private float fovRange = 80.0f;

    [SerializeField] private float enemyMemoryTime = 2.5f;

    private bool hasSeenPlayer = false;


    [Header("Attack Variables")]

    [SerializeField] private float attackStoppingDistance = 8.0f;

    [Header("Search Variables")]

    [SerializeField] private float enemySearchTime = 10.0f;

    [SerializeField] private float searchTurnSpeed = 2.0f;
    [SerializeField] private float searchAngle = 45f;

    private Quaternion initialSearchRot;

    [Header("Death Variables")]

    [SerializeField] private GameObject deathPickUp;

    [Header("SFXs")]

    [SerializeField] private AudioSource audioSource;
    [SerializeField] AudioClip hitClip;

    [Header("Other")]

    [SerializeField] private bool isIdle = false;
    [SerializeField] public WeaponBase Weapon;
    //[SerializeField] private WeaponData data;


    [Header("Enemy Alerts")]
    [SerializeField] private TextMeshProUGUI enemyAlertBox;
    [SerializeField] private Color searchColor = new Color32(245, 212, 66, 255);
    [SerializeField] private Color attackColor = new Color32(252, 48, 109, 255);
    [SerializeField] private Color patrolColor = new Color32(0, 255, 0, 255);

    private NavMeshAgent agent;

    public Health healthScript;


    private Vector3 lastSeenPlayerLocation;
    float timeSinceLastSeenPlayer;

    public enum EnemyType
    {
        Heavy,
        Medium,
        Burst,
        Light
    }

    public enum EnemyState
    {
        Patrol,
        Search,
        Attack
    }

    [SerializeField] private EnemyState enemyState;
    [SerializeField] private EnemyType enemyType;
    private bool enemyIsFiring;

    private void Start()
    {
        enemyState = EnemyState.Patrol;
        DetermineEnemyType();

        Weapon.bulletsLeft = Weapon.magazineSize;
        Weapon.ResetProperties();

        agent = GetComponent<NavMeshAgent>();

        Weapon.isAiControlled = true;
        Weapon.target = playerRef;

        agent.SetDestination(patrolNodes[nodePointer].position);

        timeSinceLastSeenPlayer = enemySearchTime + 1;

    }

    private void OnEnable()
    {
        //Subscribe to events

        healthScript.OnDeath += HandleEnemyDeath;
        healthScript.OnDamage += HandleEnemyDamage;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isIdle)
        {
            //CHECK FOR STATE CHANGE

            CheckForStateChange();


            //Patrol Mode

            if (enemyState == EnemyState.Patrol)
            {

                Patrol();

            }

            //Search Mode

            if(enemyState == EnemyState.Search)
            {
                Search();
            }

            timeSinceLastSeenPlayer += Time.deltaTime;

            //Attack Mode

            if (enemyState == EnemyState.Attack)
            {
                Attack();
              
            }
           

        }


    }


    void CheckForStateChange()
    {
        EnemyState previousState = enemyState;

        float distanceToPlayer = Vector3.Distance(transform.position, playerRef.position);

        bool playerVisible = false;
        bool inFOV = false;

        RaycastHit hit;

        //Check if player is in eye range

        if (distanceToPlayer < sightDistance)
        {
            //Check if player is in FOV (peripherals)
            inFOV = CheckInFOV();

            Vector3 towardsPlayer = (playerRef.position - transform.position).normalized;

            LayerMask visionMask = playerLayer | wallLayer;

            //Check if player is not behind any walls

            if (Physics.Raycast(transform.position, towardsPlayer, out hit, distanceToPlayer, visionMask))
            {
                Debug.DrawRay(transform.position, towardsPlayer * distanceToPlayer, Color.green);

                //Debug.Log("Hit:" + hit.collider.name);

                if ((playerLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    playerVisible = true;
                }
            }


        }

        if(playerVisible && inFOV)
        {
            timeSinceLastSeenPlayer = 0;

        }

        if ((inFOV && playerVisible) || timeSinceLastSeenPlayer < enemyMemoryTime)
        {
            enemyState = EnemyState.Attack;

    
        }
        else if(hasSeenPlayer && timeSinceLastSeenPlayer < enemySearchTime)
        {
            enemyState = EnemyState.Search;

          
        }
        else
        {
            enemyState = EnemyState.Patrol;

           

           
        }

        //Check if state has changed

        if (previousState != enemyState)
        {
            OnStateChange();
        }


    }

    void OnStateChange()
    {
        //Modify navmesh agent values when switching states

        switch (enemyState)
        {
            case EnemyState.Patrol:

                agent.SetDestination(patrolNodes[nodePointer].position);
                agent.stoppingDistance = 0f;

                hasSeenPlayer = false;

                enemyAlertBox.color = patrolColor;
                enemyAlertBox.text = "...";

                break;

            case EnemyState.Search:

                initialSearchRot = transform.rotation;

                lastSeenPlayerLocation = playerRef.position;
                agent.SetDestination(lastSeenPlayerLocation);
                agent.stoppingDistance = 1.5f;

                enemyAlertBox.color = searchColor;
                enemyAlertBox.text = "?";

                break;

            case EnemyState.Attack:

                agent.SetDestination(playerRef.position);
                agent.stoppingDistance = attackStoppingDistance;

                hasSeenPlayer = true;
                timeSinceLastSeenPlayer = 0;

                enemyAlertBox.color = attackColor;
                enemyAlertBox.text = "!";

                break;

            default:

                Debug.LogError("Enemy State '" + enemyState + "' does not exist! ");
                break;
        }

    }

    public void SetState(EnemyState newState)
    {
        enemyState = newState;

        OnStateChange();
    }

    void DetermineEnemyType()
    {
        
        switch (enemyType)
        {
            case EnemyType.Heavy:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Auto;
                    Weapon.magazineSize = 30;
                    Weapon.fireRate = 0.3f;
                    break;
                }
            case EnemyType.Medium:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Auto;
                    Weapon.magazineSize = 15;
                    Weapon.fireRate = 0.73f;
                    break;
                }
            case EnemyType.Burst:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Burst;
                    Weapon.magazineSize = 20;
                    Weapon.fireRate = 0.5f;
                    Weapon.bulletsPerBurst = 3;
                    break;
                }
            case EnemyType.Light:
                {
                    Weapon.currentMode = WeaponBase.ShootingMode.Single;
                    Weapon.magazineSize = 10;
                    Weapon.fireRate = 0.8f;
                    break;
                }
            default:
                {
                    Debug.LogError("Enemy Type '" + enemyType + "' does not exist! ");
                    break;
                }
        }
    }

    bool CheckInFOV() //Check if the player is in the angle inbetween the enemies view
    {
        Vector3 towardsPlayerVector = (playerRef.position - transform.position).normalized;

        float angle = Vector3.Angle(transform.forward, towardsPlayerVector);

        return angle <= (fovRange * 0.5f);

    }
    

    void Search()
    {
        //Enemy has reached last known player location


        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            //Rotate to imitate searching

            float angle = Mathf.Sin(Time.time * searchTurnSpeed) * searchAngle;

            transform.rotation = initialSearchRot * Quaternion.Euler(0, angle, 0);
        }
    }

    void Patrol()
    {   

        //Iterate to next node if enemy has made it to target position
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            if (nodePointer == patrolNodes.Length - 1)
            {
                nodePointer = 0;
            }
            else
            {
                nodePointer++;
            }

            agent.SetDestination(patrolNodes[nodePointer].position);
        }
        
    }

    //Basic aggro function
    void Attack()
    {
        //Step closer to target

        agent.SetDestination(playerRef.position);

        if (Weapon.bulletsLeft <= 0 && !Weapon.isReloading)
        {
            Weapon.Reload();
            return;
        }

        if (Weapon != null)
            Weapon.Fire();
    }

    void HandleEnemyDamage()
    {
        audioSource.PlayOneShot(hitClip);
    }

    void HandleEnemyDeath()
    {
        Debug.Log("Enemy Death event called");
        GameObject deathPickUpInstance = Instantiate(deathPickUp, transform.position + new Vector3(0.0f, 0.5f, 0.0f), transform.rotation);
        WeaponData data = deathPickUpInstance.GetComponent<WeaponData>();

        if (data != null)
            data.GetData(Weapon);

        Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        //Enemy range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        //Enemy fov gizmos
        Gizmos.color = Color.red;

        float halfFOV = fovRange * 0.5f;

        Vector3 leftFOV = Quaternion.AngleAxis(-halfFOV, Vector3.up) * transform.forward;
        Vector3 rightFOV = Quaternion.AngleAxis(halfFOV, Vector3.up) * transform.forward;

        Vector3 leftPoint = transform.position + leftFOV * sightDistance;
        Vector3 rightPoint = transform.position + rightFOV * sightDistance;

        Gizmos.DrawLine(transform.position, leftPoint);
        Gizmos.DrawLine(transform.position, rightPoint);

    }

    private void OnDrawGizmos()
    {
        //Enemy path gizmos

        for (int i = 0; i < patrolNodes.Length; i++)
        {
            //Circles for nodes
            Gizmos.color = Color.magenta;

            Gizmos.DrawSphere(patrolNodes[i].position, 0.3f);

            //Draw lines to connect nodes
            Gizmos.color = Color.cyan;

            if (i != patrolNodes.Length - 1)
            {

                Gizmos.DrawLine(patrolNodes[i].position, patrolNodes[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(patrolNodes[i].position, patrolNodes[0].position);
            }
        }
    }

    private void OnDisable()
    {
        //Unsubscribe to events

        healthScript.OnDeath -= HandleEnemyDeath;
        healthScript.OnDamage -= HandleEnemyDamage;
    }
}
