using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum Enemy_MeleeWeaponType { OneHand, Throw, Unarmed }
public enum Enemy_RangeWeaponType { Pistol, Revolver, Shotgun, AutoRifle, Rifle }

public class Enemy_Visuals : MonoBehaviour
{
    public GameObject currentWeaponModel { get; private set; }

    [Header("Enemy Visual Color")]
    [SerializeField] private Texture[] colorTexture;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Corruption visuals")]
    private GameObject[] corruptionCystals;
    [SerializeField] private int corruptionAmount;

    [Header("Rig references")]
    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private Rig rig;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private MultiAimConstraint weaponAimConstraint;

    private float leftHandTargetWeight;
    private float weaponAimTargetWeight;
    private float rigChangeRate;

    private void Update()
    {
        leftHandIKConstraint.weight = AdjustIKWeight(leftHandIKConstraint.weight, leftHandTargetWeight);
        weaponAimConstraint.weight = AdjustIKWeight(weaponAimConstraint.weight, weaponAimTargetWeight);
    }

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    public void EnableWeaponTrail(bool active)
    {
        Enemy_MeleeWeaponModel currentWeaponScript = currentWeaponModel.GetComponent<Enemy_MeleeWeaponModel>();

        currentWeaponScript.EnableTrailEffects(active);
    }

    public void EnableSecondaryWeaponModel(bool active)
    {
        FindSecondaryWeaponModel()?.SetActive(active);
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
        corruptionCystals = CollectCorruptionCystals();


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

    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<Enemy_Range>() != null;

        Debug.Log("thisEnemyIsMelee: " + thisEnemyIsMelee);
        Debug.Log("thisEnemyIsRange: " + thisEnemyIsRange);

        if (thisEnemyIsRange)
            currentWeaponModel = FindRangeWeaponModel();

        if (thisEnemyIsMelee)
            currentWeaponModel = FindMeleeWeaponModel();

        currentWeaponModel.SetActive(true);

        OverrideAnimatorControllerIfCan();
    }

    private void OverrideAnimatorControllerIfCan()
    {
        AnimatorOverrideController animatorOverride =
            currentWeaponModel.GetComponent<Enemy_MeleeWeaponModel>()?.animatorOverrideController;

        if (animatorOverride != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = animatorOverride;
        }
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        Animator anim = GetComponentInChildren<Animator>();

        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public void EnableWeapon(bool active)
    {
        if (!CurrentWeaponModel()) return;
        //hiddenWeapon.gameObject.SetActive(false);
        CurrentWeaponModel()?.SetActive(active);
    }

    private GameObject FindRangeWeaponModel()
    {
        Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_RangeWeaponModel>(true);

        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                SwitchAnimationLayer(((int)weaponModel.weaponHoldType));
                SetupLeftHandIK(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                return weaponModel.gameObject;
            }
        }

        Debug.LogWarning("No range weapon found!");
        return null;
    }

    private GameObject FindMeleeWeaponModel()
    {
        Enemy_MeleeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_MeleeWeaponModel>(true);
        Enemy_MeleeWeaponType weaponType = GetComponent<Enemy_Melee>().weaponType;

        List<Enemy_MeleeWeaponModel> filteredModels = new();
        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponModelType == weaponType)
            {
                filteredModels.Add(weaponModel);
            }
        }

        int randomIndex = Random.Range(0, filteredModels.Count);

        return filteredModels[randomIndex].gameObject;
    }

    private GameObject[] CollectCorruptionCystals()
    {
        Enemy_CorruptionCrystal[] cystalComponents = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);
        GameObject[] corruptionCystals = new GameObject[cystalComponents.Length];

        for (int i = 0; i < cystalComponents.Length; i++)
        {
            corruptionCystals[i] = cystalComponents[i].gameObject;
        }

        return corruptionCystals;
    }

    private GameObject FindSecondaryWeaponModel()
    {
        Enemy_SecondaryWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_SecondaryWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var model in weaponModels)
        {
            if (model.weaponType == weaponType)
            {
                return model.gameObject;
            }
        }

        return null;
    }

    public GameObject CurrentWeaponModel() => currentWeaponModel;

    public void EnableIK(bool enableLeftHand, bool enableAim, float changeRate = 10)
    {
        rigChangeRate = changeRate;
        leftHandTargetWeight = enableLeftHand ? 1 : 0;
        weaponAimTargetWeight = enableAim ? 1 : 0;
    }

    private void SetupLeftHandIK(Transform leftHandTarget, Transform leftElbowTarget)
    {

        leftHandIK.localPosition = leftHandTarget.localPosition;
        leftHandIK.localRotation = leftHandTarget.localRotation;

        leftElbowIK.localPosition = leftElbowTarget.localPosition;
        leftElbowIK.localRotation = leftElbowTarget.localRotation;

    }

    private float AdjustIKWeight(float currentWeight, float targetWeight)
    {
        if (Mathf.Abs(currentWeight - targetWeight) > 0.05f)
            return Mathf.Lerp(currentWeight, targetWeight, rigChangeRate * Time.deltaTime);
        else
            return targetWeight;
    }
}
