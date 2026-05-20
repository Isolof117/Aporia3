using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Loadout : MonoBehaviour
{

    public WeaponBase playerWeapon;
    public WeaponData SecondaryWeaponData;
    public WeaponData PrimaryWeaponData;
    private Vector3 pistolFirePoint;

    bool primaryActive, secondaryActive;

    // Start is called before the first frame update
    void Start()
    {
        playerWeapon = GetComponentInChildren<WeaponBase>();
        PrimaryWeaponData = gameObject.AddComponent<WeaponData>();
        SecondaryWeaponData = gameObject.AddComponent<WeaponData>();

        SecondaryWeaponData.PistolData();

        if (playerWeapon == null)
        {
            Debug.LogError("WeaponBase NOT FOUND");
            return;
        }

        if (PrimaryWeaponData == null)
        {
            Debug.Log("Primary weapon data NOT FOUND");
            return;
        }

        if (SecondaryWeaponData == null)
        {
            Debug.Log("Secondary weapon data NOT FOUND");
            return;
        }

        primaryActive = true;

        Debug.Log("Loadout initialized correctly");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipPrimary();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipSecondary();
        }

        if (primaryActive)
        {
            PrimaryWeaponData.GetData(playerWeapon);
        }
        if (secondaryActive)
        {
            SecondaryWeaponData.GetData(playerWeapon);
        }

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SwapWeapon();
        //}

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (primaryActive)
            {
                SecondaryWeaponData = null;
                Debug.Log("Secondary weapon dropped.");
            }

            if (secondaryActive)
            {
                PrimaryWeaponData = null;
                Debug.Log("Primary weapon dropped.");
            }
        }
    }


    void DetermineWeaponModel()
    {
        if (playerWeapon.fireRate > 0.2 && playerWeapon.fireRate < 0.5)
        {

        }
    }

    void EquipPrimary()
    {
        if (!primaryActive)
        {
            if (PrimaryWeaponData == null)
            {
                Debug.LogError("No primary weapon data!");
                return;
            }

            if (playerWeapon == null)
            {
                Debug.LogError("No player weapon!");
                return;
            }

            Debug.Log("Equipped primary");

            PrimaryWeaponData.SetData(playerWeapon);

            playerWeapon.gunModels[0].SetActive(true);
            playerWeapon.gunModels[1].SetActive(false);

            primaryActive = true;
            secondaryActive = false;
        }
    }

    void EquipSecondary()
    {
        if (primaryActive)
        {
            if (SecondaryWeaponData == null)
            {
                Debug.LogError("No secondary weapon data!");
                return;
            }

            if (playerWeapon == null)
            {
                Debug.LogError("No player weapon!");
                return;
            }

            Debug.Log("Equipped secondary");

            SecondaryWeaponData.SetData(playerWeapon);

            playerWeapon.gunModels[0].SetActive(false);
            playerWeapon.gunModels[1].SetActive(true);

            Debug.Log("Moved fire point");
            pistolFirePoint.x = -0.013f;
            pistolFirePoint.y = 2.789f;
            pistolFirePoint.z = -0.07f;

            playerWeapon.firePoint.transform.SetLocalPositionAndRotation(pistolFirePoint, Quaternion.identity);

            primaryActive = false;
            secondaryActive = true;
        }

    }

    // Pickups Version
    void SwapWeapon()
    {
        // Checks if secondary is available and you're using your primary
        if (primaryActive && SecondaryWeaponData != null)
        {
            Debug.Log("Swapped to secondary");
            SecondaryWeaponData.SetData(playerWeapon);

            primaryActive = false;
            secondaryActive = true;
        }
        // Checks if primary is available and you're using your secondary
        else if (secondaryActive && PrimaryWeaponData != null)
        {
            Debug.Log("Swapped to primary");

            PrimaryWeaponData.SetData(playerWeapon);
            primaryActive = true;
            secondaryActive = false;
        }
    }
}
