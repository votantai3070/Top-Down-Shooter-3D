using UnityEngine;

public class Enemy_AnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void StartManualMovement() => enemy.ActivateManualMovement(true);
    public void StopManualMovement() => enemy.ActivateManualMovement(false);
    public void EquipWeapon() => enemy.visuals.EnableWeapon(true);

    public void StartManualRotation() => enemy.ActivateManualRotation(true);
    public void StopManualRotation() => enemy.ActivateManualRotation(false);

    public void AbilityEvents() => enemy.AbilityTrigger();

    public void EnableIK() => enemy.visuals.EnableIK(true, true, 1.5f);

    public void EnableWeaponModel()
    {
        enemy.visuals.EnableSecondaryWeaponModel(false);
        enemy.visuals.EnableWeapon(true);
    }
}
