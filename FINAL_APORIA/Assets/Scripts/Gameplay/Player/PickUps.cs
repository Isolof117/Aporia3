using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickUps : MonoBehaviour
{
    // Weapon data from enemy
    public WeaponData data;
    private Material PickupColour;

    [SerializeField] private float rotationSpeed;

    private void Awake()
    {
        PickupColour = this.gameObject.GetComponent<Material>();
        data = GetComponent<WeaponData>();

        SelectPickupType();

        if (data == null)
        {
            Debug.LogError("No WeaponData found on pickup prefab!");
        }
    }

    private void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void SelectPickupType()
    {
        int choice = Random.Range(0, 1);
        switch (choice)
        {
            case 0:
                this.gameObject.tag = "HealthPickup";
                break;

            case 1:
                this.gameObject.tag = "WeaponPickup";
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if only the player collides with the pickup
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player collided with pickup!");

        // Get the current weapon base from the pickup
        if (this.gameObject.CompareTag("HealthPickup"))
        {
            // Add health pickup
            Health objectHealth = other.GetComponentInChildren<Health>();

            WeaponBase currentWeapon = other.GetComponentInChildren<WeaponBase>();

            if (currentWeapon == null)
            {
                Debug.LogError("Could not find player's WeaponBase!");
                return;
            }
            if (objectHealth == null)
            {
                Debug.LogError("Could not find player's health!");
                return;
            }

            Debug.Log("Player health obtained");

            objectHealth.TakeDamage(-30);
        }

        // Get the weapon data from the enemy and apply it to the player's current weapon
        if (this.gameObject.CompareTag("WeaponPickup"))
        {
            WeaponBase currentWeapon = other.GetComponentInChildren<WeaponBase>();

            if (currentWeapon == null)
            {
                Debug.LogError("Could not find player's WeaponBase!");
                return;
            }

            Loadout playerLoadout = other.GetComponent<Loadout>();

            if (playerLoadout == null)
            {
                Debug.LogError("Could not find player's Loadout!");
                return;
            }

            Debug.Log("Player weapon obtained!");

            // If secondary empty -> fill it
            if (playerLoadout.SecondaryWeaponData == null)
            {
                playerLoadout.SecondaryWeaponData = data;
                Debug.Log("Stored weapon in secondary slot");
            }
            else
            {
                // overwrite primary
                playerLoadout.PrimaryWeaponData = data;

                // immediately equip
                data.SetData(currentWeapon);

                Debug.Log("Overwrote primary weapon");
            }
        }

        //Destroy
        Destroy(gameObject);
    }
}