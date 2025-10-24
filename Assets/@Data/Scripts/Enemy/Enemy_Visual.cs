using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyWeaponModelType { OneHand, Throw, Unarmed }

public class Enemy_Visuals : MonoBehaviour
{
    [Header("Enemy Weapon Model")]
    [SerializeField] private Enemy_WeaponModel[] models;
    [SerializeField] private EnemyWeaponModelType weaponType;
    [SerializeField] private GameObject currentWeaponModel;

    [Header("Enemy Visual Color")]
    [SerializeField] private Texture[] colorTexture;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Corruption visuals")]
    [SerializeField] private GameObject[] corruptionCystals;
    [SerializeField] private int corruptionAmount;

    private void Awake()
    {
        models = GetComponentsInChildren<Enemy_WeaponModel>(true);

        CollectCorruptionCystals();
        SetupRandomCorruption();
    }

    public void EnableWeaponTrail(bool active)
    {
        Enemy_WeaponModel currentWeaponScript = currentWeaponModel.GetComponent<Enemy_WeaponModel>();

        currentWeaponScript.EnableTrailEffects(active);
    }

    public void SetupWeaponType(EnemyWeaponModelType type) => weaponType = type;

    public void SetupRandomLook() => SetupRandomColor();
    public void SetupWeaponLook() => SetupRandomWeapon();


    private void SetupRandomWeapon()
    {
        foreach (var weaponModel in models)
        {
            weaponModel.gameObject.SetActive(false);
        }

        List<Enemy_WeaponModel> filteredModels = new();
        foreach (var weaponModel in models)
        {
            if (weaponModel.weaponModelType == weaponType)
            {
                filteredModels.Add(weaponModel);
            }
        }

        //Debug.Log("filteredModels Count: " + filteredModels.Count);

        int randomIndex = Random.Range(0, filteredModels.Count);

        Debug.Log("FilteredModels Name: " + filteredModels[randomIndex].name);

        currentWeaponModel = filteredModels[randomIndex].gameObject;
        //currentWeaponModel.SetActive(true);

        OverrideAnimatorControllerIfCan();
    }

    private void OverrideAnimatorControllerIfCan()
    {
        AnimatorOverrideController animatorOverride =
            currentWeaponModel.GetComponent<Enemy_WeaponModel>().animatorOverrideController;

        if (animatorOverride != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = animatorOverride;
        }
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTexture.Length);

        Material newMaterial = new(skinnedMeshRenderer.material);

        newMaterial.mainTexture = colorTexture[randomIndex];

        skinnedMeshRenderer.material = newMaterial;
    }

    public void SetupRandomCorruption()
    {
        foreach (var crystal in corruptionCystals)
        {
            crystal.SetActive(false);
        }
        List<GameObject> crystalsList = new(corruptionCystals);
        for (int i = 0; i < corruptionAmount; i++)
        {
            if (crystalsList.Count == 0)
                break;
            int randomIndex = Random.Range(0, crystalsList.Count);
            crystalsList[randomIndex].SetActive(true);
            crystalsList.RemoveAt(randomIndex);
        }
    }

    private void CollectCorruptionCystals()
    {
        Enemy_CorruptionCrystal[] cystalComponents = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);
        corruptionCystals = new GameObject[cystalComponents.Length];

        for (int i = 0; i < cystalComponents.Length; i++)
        {
            corruptionCystals[i] = cystalComponents[i].gameObject;
        }
    }

    public GameObject CurrentWeaponModel() => currentWeaponModel;
}
