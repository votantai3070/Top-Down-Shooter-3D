using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Cover Points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<CoverPoint> coverPoints = new();
    [SerializeField] private float xOffset = 1;
    [SerializeField] private float yOffset = .2f;
    [SerializeField] private float zOffset = 1;

    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindAnyObjectByType<Player>().transform;
    }

    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints =
        {
            new Vector3 (0, yOffset, zOffset), //Front
            new Vector3 (0, yOffset, -zOffset), //Back
            new Vector3 (xOffset, yOffset, 0), //Right
            new Vector3 (-xOffset, yOffset, 0), //Left
        };

        foreach (var localPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localPoint);
            CoverPoint coverPoint =
                Instantiate(coverPointPrefab, worldPoint, Quaternion.identity, transform).GetComponent<CoverPoint>();

            coverPoints.Add(coverPoint);
        }
    }

    public List<CoverPoint> GetValidCoverPoints(Transform enemy)
    {
        List<CoverPoint> validCoverPoints = new();

        foreach (var coverPoint in coverPoints)
        {
            if (IsValidCoverPoint(coverPoint, enemy))
                validCoverPoints.Add(coverPoint);
        }

        return validCoverPoints;
    }

    private bool IsValidCoverPoint(CoverPoint coverPoint, Transform enemyTransform)
    {
        if (coverPoint.occupied)
            return false;

        if (!IsFurthestFromPlayer(coverPoint))
            return false;

        if (IsCoverBehindPlayer(coverPoint, enemyTransform))
            return false;

        if (IsCoverCloseToPlayer(coverPoint))
            return false;

        if (IsCoverCloseToLastCover(coverPoint, enemyTransform))
            return false;

        return true;
    }

    // Search Cover Point Far Player
    private bool IsFurthestFromPlayer(CoverPoint coverPoint)
    {
        CoverPoint furthestPoint = null;
        float furthestDistance = 0;

        foreach (var point in coverPoints)
        {
            float distance = Vector3.Distance(point.transform.position, playerTransform.position);

            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestPoint = point;
            }
        }

        return furthestPoint == coverPoint;
    }

    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemyTransform)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemyTransform.position);

        return distanceToPlayer < distanceToEnemy;
    }

    private bool IsCoverCloseToPlayer(CoverPoint coverPoint)
    {
        return Vector3.Distance(coverPoint.transform.position, playerTransform.position) < 2f;
    }

    private bool IsCoverCloseToLastCover(CoverPoint coverPoint, Transform enemy)
    {
        CoverPoint lastCover = enemy.GetComponent<Enemy_Range>().currentCover;

        return lastCover != null &&
            Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3;
    }
}
