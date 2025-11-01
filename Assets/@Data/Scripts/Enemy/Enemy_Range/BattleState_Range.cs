using System;
using UnityEngine;

public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy;

    private float lastTimeShot = -10;
    private int bulletShot = 0;

    public BattleState_Range(Enemy enemyBase, StateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableIK(true);
        enemy.agent.speed = 0;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.visuals.EnableIK(false);
    }

    public override void Update()
    {
        base.Update();

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

    private void AttemptToResetWeapon() => bulletShot = 0;
    private bool WeaponCooldown() => Time.time > lastTimeShot + enemy.weaponCooldown;
    private bool WeaponOutOfBullets() => bulletShot >= enemy.bulletsToShoot;
    private bool CanShoot() => Time.time >= lastTimeShot + 1 / enemy.fireRate;

    private void Shoot()
    {
        enemy.FireSingleBullet();
        lastTimeShot = Time.time;
        bulletShot++;
    }
}
