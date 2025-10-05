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

    private Vector3 BulletDirection()
    {
        Vector3 direction = (player.aim.aim.position - gunPoint.position).normalized;

        if (!player.aim.CanAimPrecisely())
            direction.y = 0;

        weaponHolder.LookAt(player.aim.aim);
        gunPoint.LookAt(player.aim.aim);

        return direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25f);

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25f);

        Gizmos.color = Color.red;
    }
}
