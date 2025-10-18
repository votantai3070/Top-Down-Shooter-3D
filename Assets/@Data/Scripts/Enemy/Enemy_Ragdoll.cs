using UnityEngine;

public class Enemy_Ragdoll : MonoBehaviour
{
    [SerializeField] Transform ragdollParent;

    [SerializeField] Collider[] ragdollCollider;
    [SerializeField]
    Rigidbody[] ragdollRigibodies;

    private void Awake()
    {
        ragdollCollider = GetComponentsInChildren<Collider>();
        ragdollRigibodies = GetComponentsInChildren<Rigidbody>();

        RagdollActive(false);
    }

    public void RagdollActive(bool active)
    {
        foreach (var rb in ragdollRigibodies)
        {
            rb.isKinematic = !active;
        }
    }

    public void ColliderActive(bool active)
    {
        foreach (var cd in ragdollCollider)
        {
            cd.enabled = active;
        }
    }
}
