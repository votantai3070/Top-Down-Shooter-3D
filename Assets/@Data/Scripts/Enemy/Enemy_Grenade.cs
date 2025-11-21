using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetupGrenade(Vector3 target, float timeToTarget)
    {
        rb.linearVelocity = CalcualteLaunchVelocity(target, timeToTarget);
    }

    private Vector3 CalcualteLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        Vector3 velocityXZ = directionXZ / timeToTarget;

        float velocityY =
            (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + (Vector3.up * velocityY);

        return launchVelocity;
    }
}
