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

    public ShootType shootType;
    public int bulletsPerShot { get; private set; }

    #region Regular mode variables
    private float defaulFireRate;
    public float fireRate = 1; //bullets per second
    private float lastShootTime;
    #endregion

    #region Burst mode variables
    private bool burstAvailable;
    private bool burstActive;
    private int burstBulletsPerShot;
    private float burstFireRate;
    public float burstFireDelay { get; private set; }
    #endregion

    [Header("Magazine details")]
    public int bulletsInMagazine; // Current Bullet
    public int magazineCapacity; // Sức chứa băng đạn
    public int totalReserveAmmo; // Số đạn còn lại

    #region Weapon generic info variables
    public float reloadSpeed { get; private set; }// How fast character reload weapon
    public float equipmentSpeed { get; private set; } // How fast character equip weapon
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    #endregion

    #region Weapon spread variables
    [Header("Spread")]
    private float baseSpread;
    private float maximumSpread = 3;
    private float currentSpread = 2;

    private float spreadIncreaseRate = .15f;
    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;
    #endregion

    public Weapon_Data weaponData { get; private set; } //serves as default weapon data

    public Weapon(Weapon_Data weaponData)
    {
        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        fireRate = weaponData.fireRate;
        this.weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;

        burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        burstBulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireDelay = weaponData.burstFireDelay;

        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;

        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        defaulFireRate = fireRate;
        this.weaponData = weaponData;
    }

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
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
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
