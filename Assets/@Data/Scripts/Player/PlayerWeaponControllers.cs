using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControllers : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    // This is the default speed form which our mass formula is derived.

    [SerializeField] private Weapon_Data defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;


    [Header("Bullet details")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;

    [SerializeField] Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlotAllow = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        Invoke(nameof(EquipStartingWeapon), .1f);
    }

    private void Update()
    {
        //Debug.Log("weaponReady: " + weaponReady);

        if (isShooting)
            Shoot();

    }

    #region Slot Management - Pickup/Equip/Drop/Ready Weapon

    private void EquipStartingWeapon()
    {
        if (weaponSlots.Count == 0)
            weaponSlots.Add(new Weapon(defaultWeaponData));

        weaponSlots[0] = new Weapon(defaultWeaponData);

        EquipWeapon(0);
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) return;

        weaponSlots.Remove(currentWeapon);

        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count) return;

        SetWeaponReady(false);


        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.instance.ChangeCameraDistance(CurrentWeapon().cameraDistance);
    }

    public void PickUpWeapon(Weapon_Data newWeaponData)
    {
        if (weaponSlots.Count >= maxSlotAllow)
        {
            Debug.Log("No Slots Available");
            return;
        }

        foreach (var w in weaponSlots)
        {
            if (w.weaponType == newWeaponData.weaponType)
            {
                Debug.Log("Has Same Weapon");
                return;
            }
        }

        Weapon newWeapon = new(newWeaponData);

        weaponSlots.Add(newWeapon);

        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;
    #endregion

    IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();
            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        if (!WeaponReady()) return;

        if (!currentWeapon.CanShoot()) return;

        player.weaponVisuals.PlayFireAnimation();

        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated())
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;


        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();

        bulletScript.BulletSetup(player.weaponControllers.CurrentWeapon().gunDistance);

        Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;

        newBullet.GetComponent<Rigidbody>().linearVelocity = bulletDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.AimTransform();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (!player.aim.CanAimPrecisely() && aim == null)
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (Weapon w in weaponSlots)
        {
            if (w.weaponType == weaponType)
                return w;
        }

        return null;
    }


    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        controls.Character.Fire.performed += ctx => isShooting = true;
        controls.Character.Fire.canceled += ctx => isShooting = false;

        controls.Character.EquipSlot1.performed += ctx => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += ctx => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += ctx => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += ctx => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += ctx => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += ctx => DropWeapon();

        controls.Character.ReloadWeapon.performed += ctx =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToggleWeaponMode.performed += ctx => currentWeapon.ToggleBurst();
    }

    #endregion
}
