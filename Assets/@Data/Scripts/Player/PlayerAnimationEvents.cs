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

        weaponControllers.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        weaponVisualController.MaximizeRigWeight();
        weaponVisualController.MaximizeLeftHandWeight();
    }

    public void WeaponEquipingIsOver()
    {
        weaponControllers.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => weaponVisualController.SwitchOnCurrentWeaponModel();
}
