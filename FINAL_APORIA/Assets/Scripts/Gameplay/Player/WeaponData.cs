using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public float bulletVelocity, bulletSpread, fireRate;
    public int magazineSize, bulletsLeft;
    public int bulletsPerBurst;

    WeaponBase.ShootingMode weaponMode;

    public void GetData(WeaponBase currentEnemyWeapon)
    {
        bulletVelocity = currentEnemyWeapon.bulletVelocity;
        bulletSpread = currentEnemyWeapon.bulletSpread;
        fireRate = currentEnemyWeapon.fireRate;
        magazineSize = currentEnemyWeapon.magazineSize;
        bulletsLeft = currentEnemyWeapon.bulletsLeft;
        weaponMode = currentEnemyWeapon.currentMode;
    }

    public void SetData(WeaponBase newWeapon)
    {
        fireRate = Mathf.Max(0.05f, fireRate);
        magazineSize = Mathf.Max(1, magazineSize);
        bulletsPerBurst = Mathf.Max(1, bulletsPerBurst);

        newWeapon.bulletVelocity = bulletVelocity;
        newWeapon.bulletSpread = bulletSpread;
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
}
