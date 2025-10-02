using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransform;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(pistol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(rifle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(shotgun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(revolver);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(autoRifle);
        }
    }

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransform.Length; i++)
        {
            gunTransform[i].gameObject.SetActive(false);
        }
    }
}
