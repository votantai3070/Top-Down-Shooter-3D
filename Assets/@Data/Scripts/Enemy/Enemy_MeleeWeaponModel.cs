using UnityEngine;

public class Enemy_MeleeWeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponModelType;
    public AnimatorOverrideController animatorOverrideController;
    public Enemy_MeleeWeaponData weaponData;

    [SerializeField] private GameObject[] trailEffects;


    private void Awake()
    {
        EnableTrailEffects(false);
    }

    public void EnableTrailEffects(bool active)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(active);
        }
    }
}
