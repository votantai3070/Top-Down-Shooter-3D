using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerControls controls;
    public PlayerAim aim { get; private set; } // Read-only property
    public PlayerMovement movement { get; private set; }
    public PlayerWeaponControllers weaponControllers { get; private set; }

    public PlayerWeaponVisuals weaponVisuals { get; private set; }

    private void Awake()
    {
        controls = new PlayerControls();
        aim = GetComponent<PlayerAim>();
        movement = GetComponent<PlayerMovement>();
        weaponControllers = GetComponent<PlayerWeaponControllers>();
        weaponVisuals = GetComponent<PlayerWeaponVisuals>();
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
