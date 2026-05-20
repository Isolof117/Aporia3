using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponBase : MonoBehaviour
{

    [Header("Assets")]
    public Camera Camera;
    public GameObject[] gunModels;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject MovingBoxCanvas;
    public QTE_MovingBox MovingBox;
    private Coroutine movingBoxQTERoutine;
    public GameObject SkillCheckCanvas;
    public SkillCheck KeyInputs;

    [Header("Bullet Attributes")]
    public float bulletVelocity, bulletSpread, fireRate, nextFireTime;
    private float lifeTime = 3;

    [Header("AI Settings")]
    public bool isAiControlled = false;
    public Transform target;

    [Header("Conditions")]
    public bool isShooting;
    private bool isBurstFiring;
    public int bulletsPerBurst = 3;
    private bool QTEShown = false;

    [Header("Reloading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;
    public AudioSource ReloadAudio;

    // UI
    //public TextMeshProUGUI QTEPopUp;

    // States
    public enum ShootingMode
    {
        Auto,
        Single,
        Burst
    }
    public ShootingMode currentMode;

    // Constructors
    private void Awake()
    {
        bulletsLeft = magazineSize;
        nextFireTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAiControlled) return;

        if (currentMode == ShootingMode.Auto)
        {
            isShooting = Input.GetMouseButton(0);
        }
        else if (currentMode == ShootingMode.Burst || currentMode == ShootingMode.Single)
        {
            isShooting = Input.GetMouseButtonDown(0);
        }

        if (isShooting)
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
        {
            bulletsLeft = 0;
            isShooting = false;
            QTESelect();
        }

        if (bulletsLeft <= 0 && !isReloading && isShooting && !QTEShown)
        {
            bulletsLeft = 0;
            isShooting = false;
            QTESelect();
        }

        if (QTEShown && bulletsLeft > 0)
        {
            CancelQTE();
        }

        if (AmmoManager.Instance.ammoCount != null && !isAiControlled)
        {
            AmmoManager.Instance.ammoCount.text = $"{bulletsLeft}/{magazineSize}";
        }
    }

    public void ResetProperties()
    {
        fireRate = Mathf.Max(0.05f, fireRate);
        magazineSize = Mathf.Max(1, magazineSize);
        bulletsPerBurst = Mathf.Max(1, bulletsPerBurst);

        CancelQTE();

        //bulletsLeft = magazineSize;
        isReloading = false;
        isBurstFiring = false;
        nextFireTime = Time.time;
        isShooting = false;
    }

    void QTESelect()
    {
        QTEShown = true;
        Debug.Log("QTE Shown: " + QTEShown);

        int Choice = Random.Range(0, 2);
        switch (Choice)
        {
            case 0:
                {
                    // SkillCheck QTE
                    if (MovingBoxCanvas != null)
                        MovingBoxCanvas.SetActive(false);

                    if (MovingBox != null)
                    {
                        MovingBox.gameObject.SetActive(false);
                        MovingBox.QTEActive = false;
                    }

                    if (SkillCheckCanvas != null)
                        SkillCheckCanvas.SetActive(true);

                    if (KeyInputs != null)
                    {
                        KeyInputs.gameObject.SetActive(true);
                        KeyInputs.isVisible = false;
                        KeyInputs.StartQTE(this);
                    }

                    break;
                }

            case 1:
                {
                    // MovingBox QTE
                    if (SkillCheckCanvas != null)
                        SkillCheckCanvas.SetActive(false);

                    if (KeyInputs != null)
                    {
                        KeyInputs.gameObject.SetActive(false);
                        KeyInputs.isVisible = false;
                    }

                    if (MovingBoxCanvas != null)
                        MovingBoxCanvas.SetActive(true);

                    if (MovingBox != null)
                    {
                        MovingBox.gameObject.SetActive(true);
                        MovingBox.QTEActive = true;

                        movingBoxQTERoutine = StartCoroutine(MovingBox.DoQTE(this));
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public void CancelQTE()
    {
        isReloading = false;
        isShooting = false;
        QTEShown = false;

        if (MovingBoxCanvas != null)
        {
            MovingBoxCanvas.SetActive(false);
        }

        if (movingBoxQTERoutine != null)
        {
            StopCoroutine(movingBoxQTERoutine);
            movingBoxQTERoutine = null;
        }

        if (SkillCheckCanvas != null)
        {
            SkillCheckCanvas.SetActive(false);
        }

        if (KeyInputs != null)
        {
            KeyInputs.isVisible = false;
            KeyInputs.gameObject.SetActive(false);
        }

        if (MovingBox != null)
        {
            MovingBox.QTEActive = false;
            MovingBox.gameObject.SetActive(false);
        }

        Debug.Log("QTE Cancelled. QTE Shown: " + QTEShown);
    }

    public bool CanFire()
    {
        return !isReloading && bulletsLeft > 0 && Time.time >= nextFireTime && !isBurstFiring;
    }

    public void Fire()
    {
        if (!CanFire())
            return;

        switch (currentMode)
        {
            case ShootingMode.Auto:
                FireBullet();
                nextFireTime = Time.time + fireRate;
                break;

            case ShootingMode.Single:
                FireBullet();
                nextFireTime = Time.time + fireRate;
                break;

            case ShootingMode.Burst:
                StartCoroutine(FireBurst());
                break;
        }
    }

    private void FireBullet()
    {
        Debug.Log("FIRING");

        bulletsLeft--;

        Vector3 shotDirection = CalculateDirectionAndSpread().normalized;

        // Fix Bullet Orientation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        bullet.transform.forward = shotDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shotDirection * bulletVelocity, ForceMode.Impulse);

        Debug.DrawRay(firePoint.position, shotDirection * 10f, Color.red, 1f);

        StartCoroutine(DestroyBullet(bullet, lifeTime));
    }

    private IEnumerator FireBurst()
    {
        isBurstFiring = true;

        int bulletsToFire = Mathf.Min(bulletsPerBurst, bulletsLeft);

        for (int i = 0; i < bulletsToFire; i++)
        {
            FireBullet();

            if (i < bulletsToFire - 1)
            {
                yield return new WaitForSeconds(fireRate);
            }
        }

        nextFireTime = Time.time + fireRate;
        isBurstFiring = false;
    }

    public void Reload()
    {
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }


    private void ReloadCompleted()
    {
        isReloading = false;
        bulletsLeft = magazineSize;
        nextFireTime = Time.time;
        isBurstFiring = false;
    }

    Vector3 CalculateDirectionAndSpread()
    {
        Vector3 targetPoint;

        if (isAiControlled && target != null)
        {
            targetPoint = target.position + Vector3.up * 0.3f; // aim at chest/head
        }
        else
        {
            Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - firePoint.position;

        float x = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float y = UnityEngine.Random.Range(-bulletSpread, bulletSpread);

        return direction + new Vector3(x, y, 0);

    }

    private IEnumerator DestroyBullet(GameObject bullet, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(bullet);

    }
}