using UnityEngine;

public class ThrowGrenadeState_Range : EnemyState
{
    private Enemy_Range enemy;

    public ThrowGrenadeState_Range(Enemy enemyBase, StateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableWeapon(false);
        enemy.visuals.EnableIK(false, false);
        enemy.visuals.EnableSecondaryWeaponModel(true);
    }

    public override void Exit()
    {
        base.Exit();


    }

    public override void Update()
    {
        base.Update();

        Vector3 playerPos = enemy.player.position + Vector3.up;

        enemy.RotateFace(playerPos);

        enemy.aim.position = playerPos;

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        enemy.ThrowGrenade();
        enemy.visuals.EnableIK(true, true, 1.5f);

    }
}
