using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}


[System.Serializable] // Make class visible in the inspector
public class Weapon
{
    public WeaponType weaponType;


    public int bulletsInMagazine; // Current Bullet
    public int magazineCapacity; // Sức chứa băng đạn
    public int totalReserveAmmo; // Số đạn còn lại

    [Range(1, 2)]
    public float reloadSpeed = 1;// How fast character reload weapon
    [Range(1, 2)]
    public float equipmentSpeed = 1; // How fast character equip weapon

    [Space]
    public float fireRate = 1; //bullets per second
    private float lastShootTime;

    public bool CanShoot()
    {
        if (HaveEnoughBullets() & ReadyToFire())
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
    }

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    #region Reload methods
    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
            return false;

        if (totalReserveAmmo > 0)
            return true;

        return false;
    }

    public void RefillBullets()
    {
        int bulletsSpent = magazineCapacity - bulletsInMagazine;

        int bulletsToReload = Mathf.Min(magazineCapacity, totalReserveAmmo);

        totalReserveAmmo -= bulletsSpent;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo <= 0)
            totalReserveAmmo = 0;
    }

    private bool HaveEnoughBullets() => bulletsInMagazine > 0;

    #endregion
}
