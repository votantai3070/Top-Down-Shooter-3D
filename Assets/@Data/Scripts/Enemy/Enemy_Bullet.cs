using UnityEngine;

public class Enemy_Bullet : Bullet
{
    protected override void OnCollisionEnter(Collision collision)
    {
        CreateInpactFx(collision);
        ReturnBulletPool();

        Player player = collision.gameObject.GetComponentInParent<Player>();

        if (player != null)
        {
            Debug.Log("Player hit by enemy bullet");
        }
    }
}
