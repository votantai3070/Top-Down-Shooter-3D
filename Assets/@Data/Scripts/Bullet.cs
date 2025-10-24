using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;


    [SerializeField] GameObject bulletImpactFX;


    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }


    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisabledBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }
    public void BulletSetup(float flyDistance = 100, float impactForce = 100)
    {
        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.time = .25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f; // magic number .5f is a length of tip the laser ( Check method UpdateAimVisuals on PlayerAim script)
        this.impactForce = impactForce;
    }

    protected void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletPool();
    }

    protected void ReturnBulletPool()
    {
        ObjectPool.instance.DelayReturnToPool(gameObject);
    }

    protected virtual void DisabledBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime; //magic number 2 is choosen through testing
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        Enemy_Shield enemyShield = collision.gameObject.GetComponent<Enemy_Shield>();

        CreateInpactFx(collision);
        ReturnBulletPool();

        if (enemyShield != null)
        {
            enemyShield.ReduceDurability(1);
            return;
        }

        if (enemy != null)
        {
            Vector3 force = rb.linearVelocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

            enemy.GetHit();

            if (hitRigidbody != null)
                enemy.DeathImpact(force, collision.contacts[0].point, hitRigidbody);
        }


    }

    protected void CreateInpactFx(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFx = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFx.transform.position = contact.point;

            ObjectPool.instance.DelayReturnToPool(newImpactFx, 1);
        }
    }

}
