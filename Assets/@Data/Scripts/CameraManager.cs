using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private CinemachineCamera cineCamera;
    private CinemachinePositionComposer positionComposer;

    [Header("Camera Distance")]
    [SerializeField] bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    private float targetCameraDistance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one CameraManager in the scene!");
            Destroy(gameObject);
        }
        cineCamera = GetComponentInChildren<CinemachineCamera>();
        positionComposer = cineCamera.GetComponent<CinemachinePositionComposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance) return;

        float currentCameraDistance = positionComposer.CameraDistance;
        float cameraTreshold = 0.1f;

        if (Mathf.Abs(targetCameraDistance - currentCameraDistance) < cameraTreshold) return;

        positionComposer.CameraDistance =
            Mathf.Lerp(currentCameraDistance, targetCameraDistance, Time.deltaTime * distanceChangeRate);

    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;

}