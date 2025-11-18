using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;

    private float lastTimeShot = -10;
    private int bulletShot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    private float coverCheckTimer;

    public BattleState_Range(Enemy enemyBase, StateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();

        enemy.visuals.EnableIK(true, true);
        enemy.agent.speed = 0;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.visuals.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();

        if (!enemy.IsPlayerInAgrresionRange())
            stateMachine.ChangeState(enemy.advancePlayerState);

        ChangeCoverIfShould();

        enemy.RotateFace(enemy.player.position);

        if (WeaponOutOfBullets())
        {
            if (WeaponCooldown())
                AttemptToResetWeapon();

            return;
        }

        if (CanShoot())
        {
            Shoot();
        }
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover)
            return;

        coverCheckTimer -= Time.deltaTime;

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = 1f;

            if (IsPlayerInClearSight() || IsPlayerClose())
                if (enemy.CanGetCover())
                    stateMachine.ChangeState(enemy.runToCoverState);
        }
    }

    #region Cover system

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance;
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;

        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit))
        {
            return hit.collider.gameObject.GetComponentInParent<Player>();
        }

        return false;
    }

    #endregion

    #region Weapon region
    private void AttemptToResetWeapon()
    {
        bulletShot = 0;
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemy.weaponData.GetWeaponCooldown();

    }
    private bool WeaponCooldown() => Time.time > lastTimeShot + weaponCooldown;
    private bool WeaponOutOfBullets() => bulletShot >= bulletsPerAttack;
    private bool CanShoot() => Time.time >= lastTimeShot + 1 / enemy.weaponData.fireRate;
    #endregion

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletShot++;
    }
}
