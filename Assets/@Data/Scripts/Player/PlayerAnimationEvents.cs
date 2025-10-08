using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals weaponVisualController;
    private PlayerWeaponControllers weaponControllers;

    private void Start()
    {
        weaponVisualController = GetComponentInParent<PlayerWeaponVisuals>();
        weaponControllers = GetComponentInParent<PlayerWeaponControllers>();
    }

    public void ReloadIsOver()
    {
        weaponVisualController.MaximizeRigWeight();
        //Refill bullets
        weaponControllers.CurrentWeapon().RefillBullets();
    }

    public void ReturnRig()
    {
        weaponVisualController.MaximizeRigWeight();
        weaponVisualController.MaximizeLeftHandWeight();
    }

    public void WeaponGrabIsOver()
    {
        weaponVisualController.SetBusyGrabbingWeaponTo(false);
    }
}
