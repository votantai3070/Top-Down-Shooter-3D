using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private Transform[] gunTransform;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;

    [SerializeField] private Transform currentGun;

    [Header("Rid")]
    [SerializeField] float rigIncreaseSpeed;
    private bool rigShouldBeInscreased;
    private Rig rig;

    [Header("Left Hand IK")]
    [SerializeField] private Transform leftHand;


    private void Start()
    {
        SwitchOn(pistol);

        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");

            rig.weight = 0f;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            rigShouldBeInscreased = true;
        }

        if (rigShouldBeInscreased)
        {
            rig.weight += rigIncreaseSpeed * Time.deltaTime;

            if (rig.weight >= 1f)
            {
                rig.weight = 1f;
                rigShouldBeInscreased = false;
            }
        }
    }

    public void ReturnRigWeightToOne() => rigShouldBeInscreased = true;

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;

        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransform.Length; i++)
        {
            gunTransform[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;

        leftHand.position = targetTransform.position;
        leftHand.rotation = targetTransform.rotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
            SwitchAnimationLayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(rifle);
            SwitchAnimationLayer(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(shotgun);
            SwitchAnimationLayer(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(revolver);
            SwitchAnimationLayer(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(autoRifle);
            SwitchAnimationLayer(1);
        }
    }
}
