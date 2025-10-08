using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControllers : MonoBehaviour
{
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    // This is the default speed form which our mass formula is derived.

    [SerializeField] private Weapon currentWeapon;


    [Header("Bullet details")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform gunPoint;

    [SerializeField] Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlotAllow = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();


        currentWeapon.bulletsInMagazine = currentWeapon.magazineCapacity;
    }

    #region Slot Management - Pickup/Equip/Drop Weapon
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) return;

        weaponSlots.Remove(currentWeapon);

        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
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
    #endregion

    private void Shoot()
    {
        if (!currentWeapon.CanShoot()) return;

        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;

        newBullet.GetComponent<Rigidbody>().linearVelocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 3f);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
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

    public Transform GunPoint() => gunPoint;

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.AimTransform();

        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (!player.aim.CanAimPrecisely() && aim == null)
            direction.y = 0;

        //weaponHolder.LookAt(player.aim.AimTransform());
        //gunPoint.LookAt(player.aim.AimTransform()); //TODO: find a better place for this

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;


    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        controls.Character.Fire.performed += ctx => Shoot();

        controls.Character.EquipSlot1.performed += ctx => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += ctx => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += ctx => DropWeapon();

        controls.Character.ReloadWeapon.performed += ctx =>
        {
            if (currentWeapon.CanReload())
            {
                player.weaponVisuals.PlayReloadAnimation();
            }
        };
    }
    #endregion
}
