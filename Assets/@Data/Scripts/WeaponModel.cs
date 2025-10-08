using UnityEngine;

public enum GrabType { SideGrab, BackSide };

public enum HoldType { CommonHold = 1, LowHold, HighHold }

public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public GrabType grabType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform holdPoint;
}
