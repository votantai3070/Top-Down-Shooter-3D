using UnityEngine;

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

    [Header("Left Hand IK")]
    [SerializeField] private Transform leftHand;

    private void Start()
    {
        SwitchOn(pistol);

        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
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
}
