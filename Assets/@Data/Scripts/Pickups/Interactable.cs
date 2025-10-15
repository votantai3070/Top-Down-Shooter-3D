using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected PlayerWeaponControllers weaponControllers;

    private MeshRenderer mesh;
    [SerializeField] private Material highlightMaterial;
    protected Material defaultMaterial;

    private void Start()
    {
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = mesh.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = mesh.sharedMaterial;
    }

    public virtual void Interaction()
    {
        Debug.Log("Interaction:  " + gameObject.name);
    }

    public void HighlightActive(bool active)
    {
        if (active)
            mesh.material = highlightMaterial;
        else
            mesh.material = defaultMaterial;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponControllers == null)
            weaponControllers = other.GetComponent<PlayerWeaponControllers>();

        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null) return;

        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteracable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null) return;

        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteracable();
    }
}
