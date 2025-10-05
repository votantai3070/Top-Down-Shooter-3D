using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Information")]
    [Range(.5f, 1f)]
    [SerializeField] float minCameraDistance = 1.5f;
    [Range(1f, 3f)]
    [SerializeField] float maxCameraDistance = 4f;
    [Range(3f, 5f)]
    [SerializeField] float senSensetivity = 5f;

    [SerializeField] Transform aim;
    [SerializeField] LayerMask aimLayerMask;

    Vector2 aimInput;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        aim.position = Vector3.Lerp(aim.position, DesiredAimPosition(), senSensetivity * Time.deltaTime);
    }

    private Vector3 DesiredAimPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredAimPosition = GetMousePosition();
        Vector3 aimDirection = (desiredAimPosition - transform.position).normalized;

        float distance = Vector3.Distance(desiredAimPosition, transform.position);
        float clampedDistance = Mathf.Clamp(distance, minCameraDistance, actualMaxCameraDistance);

        desiredAimPosition = transform.position + (aimDirection * clampedDistance);
        desiredAimPosition.y = transform.position.y + 1;

        return desiredAimPosition;
    }

    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        controls.Character.Aim.canceled += ctx => aimInput = Vector2.zero;
    }
}
