using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private Enemy_Melee enemy;
    private Rigidbody rb;

    [SerializeField] private int durability;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponentInParent<Enemy_Melee>();


        //rb.isKinematic = true;
    }

    public void ReduceDurability(int amount)
    {
        durability -= amount;
        if (durability <= 0)
        {
            BreakShield();
        }
    }

    private void BreakShield()
    {
        if (enemy != null)
        {
            enemy.anim.SetFloat("ChaseIndex", 0);
        }

        //rb.isKinematic = false;
        gameObject.SetActive(false);
        //Destroy(gameObject);1
    }
}
