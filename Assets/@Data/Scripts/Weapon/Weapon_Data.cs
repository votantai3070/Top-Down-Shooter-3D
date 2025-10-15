using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;


    [Header("Bullet")]
    public int bulletsInMagazine; // Current Bullet
    public int magazineCapacity; // Sức chứa băng đạn
    public int totalReserveAmmo; // Số đạn còn lại

    [Header("Regular Shot")]
    public ShootType shootType;
    public int bulletsPerShot = 1;
    public float fireRate;

    [Header("Burst Shot")]
    public bool burstAvailable;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = .1f;

    [Header("Weapon Spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = .15f;

    [Header("Weapon Generics")]
    public WeaponType weaponType;
    [Range(1, 3)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipmentSpeed = 1;
    [Range(4, 8)]
    public float gunDistance = 4;
    [Range(4, 8)]
    public float cameraDistance = 6;
}
