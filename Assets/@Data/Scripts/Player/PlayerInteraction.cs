using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new();

    private Interactable closestInteracable;

    private void Start()
    {
        Player player = GetComponent<Player>();
        player.controls.Character.Interaction.performed += ctx => InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        closestInteracable?.Interaction();
        interactables.Remove(closestInteracable);

        UpdateClosestInteracable();
    }

    public void UpdateClosestInteracable()
    {
        closestInteracable?.HighlightActive(false);

        closestInteracable = null;
        float closestDistance = float.MaxValue;

        foreach (var interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (distance < closestDistance)
            {
                {
                    closestDistance = distance;
                    closestInteracable = interactable;
                }
            }
        }

        closestInteracable?.HighlightActive(true);
    }

    public List<Interactable> GetInteractables() => interactables;
}
