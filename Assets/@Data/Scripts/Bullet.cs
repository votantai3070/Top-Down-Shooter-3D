using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject bulletImpactFX;

    private Rigidbody rb => GetComponent<Rigidbody>();

    private void OnEnable()
    {
        Invoke(nameof(Return), 5f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void Return()
    {
        ObjectPool.instance.ReturnPool(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateInpactFx(collision);

        ObjectPool.instance.ReturnPool(gameObject);
    }

    private void CreateInpactFx(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFx = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

            Destroy(newImpactFx, 1f);
        }
    }
}
