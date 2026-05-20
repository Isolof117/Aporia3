using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public float bulletVelocity, bulletSpread, fireRate;
    public int magazineSize, bulletsLeft;
    public int bulletsPerBurst;

    WeaponBase.ShootingMode weaponMode;

    public void GetData(WeaponBase currentWeapon)
    {
        bulletVelocity = currentWeapon.bulletVelocity;
        bulletSpread = currentWeapon.bulletSpread;
        fireRate = currentWeapon.fireRate;
        magazineSize = currentWeapon.magazineSize;
        bulletsLeft = currentWeapon.bulletsLeft;
        weaponMode = currentWeapon.currentMode;
    }

    public void SetData(WeaponBase newWeapon)
    {
        fireRate = Mathf.Max(0.05f, fireRate);
        magazineSize = Mathf.Max(1, magazineSize);
        bulletsPerBurst = Mathf.Max(1, bulletsPerBurst);

        newWeapon.bulletVelocity = bulletVelocity;
        newWeapon.bulletSpread = 0;
        newWeapon.fireRate = fireRate;
        newWeapon.magazineSize = magazineSize;
        newWeapon.bulletsLeft = Mathf.Clamp(bulletsLeft, 0, magazineSize);
        newWeapon.currentMode = weaponMode;

        if (newWeapon.bulletsLeft <= 2)
        {
            newWeapon.bulletsLeft++;
        }

        newWeapon.ResetProperties();
    }


    public void PistolData()
    {
        fireRate = 0.6f;
        magazineSize = 20;
        bulletVelocity = 30;
        bulletsLeft = magazineSize;
        weaponMode = WeaponBase.ShootingMode.Single;
    }
}
