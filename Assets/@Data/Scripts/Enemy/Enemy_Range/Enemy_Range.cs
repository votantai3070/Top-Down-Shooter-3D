using UnityEngine;

public class Enemy_Range : Enemy
{
    public Transform weaponHolder;

    public float fireRate = 1; //Bullets per second
    public GameObject bulletPrefab;
    public Transform gunPoint;
    public float bulletSpeed = 20f;

    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");

        Vector3 bulletDirection = (player.position + Vector3.up - gunPoint.position).normalized;

        GameObject bullet = ObjectPool.instance.GetObject(bulletPrefab);
        bullet.transform.position = gunPoint.position;
        bullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        bullet.GetComponent<Enemy_Bullet>().BulletSetup();

        bullet.GetComponent<Rigidbody>().linearVelocity = bulletDirection * bulletSpeed;
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(battleState);
    }
}
