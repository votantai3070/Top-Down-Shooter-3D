using UnityEngine;

public class PlayerWeaponControllers : MonoBehaviour
{
    private Player player;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform gunPoint;

    [SerializeField] Transform weaponHolder;

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += ctx => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);

        newBullet.GetComponent<Rigidbody>().linearVelocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 3f);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

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

    public Transform GunPoint() => gunPoint;
}
