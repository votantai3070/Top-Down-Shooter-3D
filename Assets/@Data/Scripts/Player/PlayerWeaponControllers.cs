using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControllers : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    // This is the default speed form which our mass formula is derived.

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
        if (isShooting)
            Shoot();
    }

    #region Slot Management - Pickup/Equip/Drop/Ready Weapon

    private void EquipStartingWeapon()
    {
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
        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();
    }

    public void PickUpWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlotAllow)
        {
            Debug.Log("No Slots Available");
            return;
        }

        foreach (Weapon w in weaponSlots)
        {
            if (w.weaponType != newWeapon.weaponType)
            {
                weaponSlots.Add(newWeapon);
            }
        }

        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;
    #endregion

    private void Shoot()
    {
        if (!WeaponReady()) return;

        if (!currentWeapon.CanShoot()) return;

        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        GameObject newBullet = ObjectPool.instance.Get();

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);


        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;

        newBullet.GetComponent<Rigidbody>().linearVelocity = bulletDirection * bulletSpeed;

        player.weaponVisuals.PlayFireAnimation();
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Weapon BackupWeapon()
    {
        foreach (Weapon w in weaponSlots)
        {
            if (w != currentWeapon)
            {
                return w;
            }

        }
        return null;
    }

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


    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        controls.Character.Fire.performed += ctx => isShooting = true;
        controls.Character.Fire.canceled += ctx => isShooting = false;

        controls.Character.EquipSlot1.performed += ctx => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += ctx => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += ctx => DropWeapon();

        controls.Character.ReloadWeapon.performed += ctx =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
    }

    #endregion
}
