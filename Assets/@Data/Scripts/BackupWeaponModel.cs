using UnityEngine;

public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType weaponType;

    private Player player;

    private void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }
}
