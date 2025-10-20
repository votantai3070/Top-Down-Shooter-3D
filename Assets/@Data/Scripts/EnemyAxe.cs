using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;
    [SerializeField] private GameObject newFX;

    private Vector3 direction;
    private Transform player;
    private float flySpeed;
    private float rotateSpeed;
    private float timer;

    public void AxeSetup(float flySpeed, Transform player, float timer)
    {
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
        rotateSpeed = Random.Range(720, 1440);
    }

    private void Update()
    {
        axeVisual.Rotate(rotateSpeed * Time.deltaTime * Vector3.right);
        timer -= Time.deltaTime;

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position;


        rb.linearVelocity = direction.normalized * flySpeed;
        transform.forward = rb.linearVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if (bullet != null || player != null)
        {
            GameObject fx = ObjectPool.instance.GetObject(newFX);
            fx.transform.position = transform.position;

            ObjectPool.instance.DelayReturnToPool(this.gameObject);
            ObjectPool.instance.DelayReturnToPool(fx, 1f);
        }

    }
}
