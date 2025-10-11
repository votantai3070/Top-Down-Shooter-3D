using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator anim;

    private Player player;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackupWeaponModel[] backupWeaponModels;

    [Header("Rid")]
    [SerializeField] float rigWeightIncreaseRate;
    private bool shouldIncrease_RigWeight;
    private Rig rig;

    [Header("Left Hand IK")]
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    bool shouldIncrease_LeftHandWeight;
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Target;


    private void Start()
    {
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public void PlayFireAnimation() => anim.SetTrigger("Fire");

    public void PlayReloadAnimation()
    {


        float reloadSpeed = player.weaponControllers.CurrentWeapon().reloadSpeed;

        anim.SetTrigger("Reload");
        anim.SetFloat("ReloadSpeed", reloadSpeed);
        ReduceRigWeight();
    }

    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().equipType;

        float equipmentSpeed = player.weaponControllers.CurrentWeapon().equipmentSpeed;

        ReduceLeftHandIKWeight();
        ReduceRigWeight();
        anim.SetFloat("EquipType", ((float)equipType));
        anim.SetFloat("EquipSpeed", equipmentSpeed);
        anim.SetTrigger("EquipWeapon");
    }


    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = ((int)CurrentWeaponModel().holdType);

        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();

        if (!player.weaponControllers.HasOnlyOneWeapon())
            SwitchOnBackupWeaponModel();

        SwitchAnimationLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel w in backupWeaponModels)
        {
            w.Activate(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        SwitchOffBackupWeaponModels();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach (BackupWeaponModel backupModel in backupWeaponModels)
        {
            if (backupModel.weaponType == player.weaponControllers.CurrentWeapon().weaponType)
                continue;


            if (player.weaponControllers.HasWeaponTypeInInventory(backupModel.weaponType))
            {
                if (backupModel.HangTypeIs(HangType.LowBackHang))
                    lowHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.BackHang))
                    backHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.SideHang))
                    sideHangWeapon = backupModel;
            }
        }

        lowHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.weaponControllers.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }

        return weaponModel;
    }

    #region Animation Rigging Methods
    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIK_Target.position = targetTransform.position;
        leftHandIK_Target.rotation = targetTransform.rotation;
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1f)
            {
                leftHandIK.weight = 1f;
                shouldIncrease_LeftHandWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1f)
            {
                rig.weight = 1f;
                shouldIncrease_RigWeight = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = .15f;
    }

    private void ReduceLeftHandIKWeight()
    {
        leftHandIK.weight = 0f;
    }

    public void MaximizeRigWeight() => shouldIncrease_RigWeight = true;

    public void MaximizeLeftHandWeight() => shouldIncrease_LeftHandWeight = true;
    #endregion

}
