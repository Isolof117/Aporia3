using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemySentry : MonoBehaviour
{
    public static System.Action<EnemySentry> OnSentryDeactivated;


    [Header("Materials")]

    [SerializeField] private Material onMat;

    [SerializeField] private Material offMat;

    [Header("Attacking State")]

    [SerializeField] private Transform firePoint;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float bulletSpread;

    [SerializeField] private float bulletVelocity;

    [SerializeField] private float fireRate = 0.5f;

    [SerializeField] private float attackStartUpTime = 2.0f;

    [SerializeField] private int bulletsPerBurst = 5;


    public bool isActive = false;

    private bool isAttacking = false;

    [Header("Scanning State")]

    [SerializeField] private float scanAngle = 40.0f;

    [SerializeField] private float scanSpeed = 2.0f;

    [SerializeField] private float sightRange = 10.0f;

    [SerializeField] private float giveUpRange = 18.0f;

    [SerializeField] private float fovRange = 80.0f;

    [Header("Deactivated state")]

    [SerializeField] private float deactivatedX = 60.0f;


    [Header("SFXs")]

    [SerializeField] private AudioClip aggroSound;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip deactivateSound;

    [Header("Parameters")]
    [SerializeField] private GameObject cameraHead;

    [SerializeField] private Canvas sentryCanvas;

    private Health healthScript;

    private Renderer headRend;

    private AudioSource audioSource;


    public enum SentryState
    {
        DEACTIVATED,
        SCANNING,
        ATTACKING
    };

    public SentryState state;

    private Quaternion startingRot;

    private float scanTime = 0f;


    [SerializeField] private Transform playerRef;
  

    // Start is called before the first frame update
    void Awake()
    {
        healthScript = GetComponent<Health>();
        headRend = cameraHead.GetComponent<Renderer>();

        audioSource = GetComponent<AudioSource>();

        startingRot = cameraHead.transform.localRotation;

        
    }

    private void OnEnable()
    {
        healthScript.OnDeath += Deactivate;
    }

    private void OnDisable()
    {
        healthScript.OnDeath -= Deactivate;
    }

    private void Start()
    {
        state = SentryState.DEACTIVATED;

        cameraHead.transform.localRotation = startingRot * Quaternion.Euler(deactivatedX, 0f, 0f);

        headRend.material = offMat;

    }

    // Update is called once per frame
    void Update()
    {

        if(state == SentryState.SCANNING)
        {
            //Scan movement
            scanTime += Time.deltaTime;

            float angle = Mathf.PingPong(scanTime * scanSpeed, scanAngle * 2) - scanAngle;

            cameraHead.transform.localRotation = startingRot *  Quaternion.Euler(0f, angle, 0f);

            //Check for aggro

            float distanceToPlayer = Vector3.Distance(transform.position, playerRef.position);

            if(distanceToPlayer < sightRange)
            {

                if(CheckInFOV())
                {
                    state = SentryState.ATTACKING;

               
                }
               
            }    

        }

        if(state == SentryState.ATTACKING)
        {
         

            cameraHead.transform.LookAt(playerRef);

           if(!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
          


            //Check for de-aggro
            float distanceToPlayer = Vector3.Distance(transform.position, playerRef.position);

            if (distanceToPlayer > giveUpRange)
            {
                state = SentryState.SCANNING;
            }

            Vector3 directionToPlayer = (playerRef.position - transform.position).normalized;


            float dot = Vector3.Dot(transform.forward, directionToPlayer);

            if(dot <= 0)
            {
                state = SentryState.SCANNING;
            }
            

            
        }
        
        //DEBUG ONLY

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Activate();
        //}
    }


    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        audioSource.pitch = 1.0f;
        audioSource.PlayOneShot(aggroSound);

        yield return new WaitForSeconds(attackStartUpTime);

        for(int i = 0; i < bulletsPerBurst; i++)
        {
            if(state != SentryState.ATTACKING) { break; }

            FireBullet();

            yield return new WaitForSeconds(fireRate);
        }

        isAttacking = false;

    }

    void FireBullet()
    {
        Debug.Log("Fired bullet");

        Vector3 shotDirection = CalculateDirectionAndSpread().normalized;

        // Fix Bullet Orientation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        bullet.transform.forward = shotDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shotDirection * bulletVelocity, ForceMode.Impulse);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(shotSound);

    }
    Vector3 CalculateDirectionAndSpread()
    {
        Vector3 targetPoint = playerRef.transform.position + Vector3.up * 0.3f;

        Vector3 direction = targetPoint - firePoint.position;

        float x = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float y = UnityEngine.Random.Range(-bulletSpread, bulletSpread);

        return direction + new Vector3(x, y, 0);

    }

    public void Deactivate()
    {
        state = SentryState.DEACTIVATED;
        sentryCanvas.enabled = false;

        audioSource.pitch = 0.5f;
        audioSource.PlayOneShot(deactivateSound);

        OnSentryDeactivated?.Invoke(this); //notify wave controller that sentry has been deactivated

        StartCoroutine(DeactivationAnim());
    }

    IEnumerator DeactivationAnim()
    {
        Quaternion startRot = cameraHead.transform.localRotation;

        Quaternion endRot = startRot * Quaternion.Euler(deactivatedX, 0f, 0f);

        bool matChange = false;

        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime;

            //Turn off light half way through anim
            if(t > 0.5f && !matChange) { headRend.material = offMat; matChange = true; }

            cameraHead.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        isActive = false;
    }

    public void Activate()
    {
        sentryCanvas.enabled = true;

        isActive = true;

        healthScript.ResetHealth();

        StartCoroutine(ActivationAnim());

    }

    IEnumerator ActivationAnim()
    {
        Quaternion startRot = cameraHead.transform.localRotation;

        Quaternion endRot = startingRot;

        bool matChange = false;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            //Turn on light half way through anim
            if (t > 0.5f && !matChange) { headRend.material = onMat; matChange = true; }

            cameraHead.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        scanTime = scanAngle / scanSpeed;
        state = SentryState.SCANNING;

    }

    bool CheckInFOV() //Check if the player is in the angle inbetween the enemies view
    {
        Vector3 towardsPlayerVector = (playerRef.position - cameraHead.transform.position).normalized;

        float angle = Vector3.Angle(cameraHead.transform.forward, towardsPlayerVector);

        return angle <= (fovRange * 0.5f);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, giveUpRange);

        //Enemy fov gizmos
        Gizmos.color = Color.red;

        float halfFOV = fovRange * 0.5f;

        Vector3 leftFOV = Quaternion.AngleAxis(-halfFOV, Vector3.up) * cameraHead.transform.forward;
        Vector3 rightFOV = Quaternion.AngleAxis(halfFOV, Vector3.up) * cameraHead.transform.forward;


        Vector3 leftPoint = cameraHead.transform.position + leftFOV * sightRange;
        Vector3 rightPoint = cameraHead.transform.position + rightFOV * sightRange;

        Gizmos.DrawLine(cameraHead.transform.position, leftPoint);
        Gizmos.DrawLine(cameraHead.transform.position, rightPoint);
    }
}
