using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout : MonoBehaviour
{

    public WeaponBase playerWeapon;
    public WeaponData SecondaryWeaponData;
    public WeaponData PrimaryWeaponData;

    bool primaryActive, secondaryActive;

    // Start is called before the first frame update
    void Start()
    {
        playerWeapon = GetComponent<WeaponBase>();

        primaryActive = true;
        PrimaryWeaponData.GetData(playerWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwapWeapon();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (primaryActive)
            SecondaryWeaponData = null;

            if (secondaryActive)
            PrimaryWeaponData = null;
        }
    }


    void SwapWeapon()
    {
        // Checks if secondary is available and you're using your primary
        if (primaryActive && SecondaryWeaponData != null)
        {
            SecondaryWeaponData.SetData(playerWeapon);
            primaryActive = false;
            secondaryActive = true;
        }

        // Checks if primary is available and you're using your secondary
        else if (secondaryActive && PrimaryWeaponData != null)
        {
            PrimaryWeaponData.SetData(playerWeapon);
            primaryActive = true;
            secondaryActive = false;
        }
    }
}
