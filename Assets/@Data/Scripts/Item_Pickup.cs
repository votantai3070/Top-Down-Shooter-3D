using UnityEngine;

public class Item_Pickup : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerWeaponControllers>() != null)
        {
            other.GetComponent<PlayerWeaponControllers>()?.PickUpWeapon(weapon);
        }
    }
}
