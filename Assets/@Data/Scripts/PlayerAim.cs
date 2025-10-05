using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Control")]
    public Transform aim;

    [SerializeField] bool isAimingPrecisely;

    [Header("Camera Control Information")]
    [Range(.5f, 1f)]
    [SerializeField] float minCameraDistance = 1.5f;
    [Range(1f, 3f)]
    [SerializeField] float maxCameraDistance = 4f;
    [Range(3f, 5f)]
    [SerializeField] float cameraSensetivity = 5f;

    [Space]

    [SerializeField] Transform cameraTarget;
    [SerializeField] LayerMask aimLayerMask;

    Vector2 aimInput;
    RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            isAimingPrecisely = !isAimingPrecisely;

        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        cameraTarget.position =
                    Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private void UpdateAimPosition()
    {
        aim.position = GetMouseHitInfo().point;

        if (!isAimingPrecisely)
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }

    public bool CanAimPrecisely()
    {
        if (isAimingPrecisely)
            return true;

        return false;
    }

    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distance = Vector3.Distance(desiredCameraPosition, transform.position);
        float clampedDistance = Mathf.Clamp(distance, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + (aimDirection * clampedDistance);
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aimInput = Vector2.zero;
    }
}
