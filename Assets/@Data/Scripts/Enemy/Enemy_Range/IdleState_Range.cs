using UnityEngine;

public class IdleState_Range : EnemyState
{
    private Enemy_Range enemy;

    public IdleState_Range(Enemy enemyBase, StateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;

        enemy.visuals.EnableIK(false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0f)
            stateMachine.ChangeState(enemy.moveState);
    }
}
