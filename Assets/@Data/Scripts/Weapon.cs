using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto,
}


[System.Serializable] // Make class visible in the inspector
public class Weapon
{
    public WeaponType weaponType;
    [Space]
    [Header("Shooting spesifics")]
    public ShootType shootType;
    public int bulletsPerShot;
    public float defaulFireRate;
    public float fireRate = 1; //bullets per second
    private float lastShootTime;

    [Header("Burst fire")]
    public bool burstAvailable;
    public bool burstActive;

    public int burstModeBulletsPerShot;
    public float burstModeFireRate;
    public float burstFireDelay = .1f;

    [Header("Magazine details")]
    public int bulletsInMagazine; // Current Bullet
    public int magazineCapacity; // Sức chứa băng đạn
    public int totalReserveAmmo; // Số đạn còn lại

    [Range(1, 2)]
    public float reloadSpeed = 1;// How fast character reload weapon
    [Range(1, 2)]
    public float equipmentSpeed = 1; // How fast character equip weapon
    [Range(2, 12)]
    public float gunDistance = 6f;
    [Range(1, 8)]
    public float cameraDistance = 6;

    [Header("Spread")]
    public float baseSpread;
    public float maximumSpread = 3;
    private float currentSpread = 2;

    public float spreadIncreaseRate = .15f;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;

    #region Spread methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            currentSpread = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    public void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion

    #region Burst methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (!burstAvailable) return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstModeBulletsPerShot;
            fireRate = burstModeFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaulFireRate;
        }
    }

    #endregion

    public bool CanShoot()
    {
        if (HaveEnoughBullets() && ReadyToFire())
        {
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
