using UnityEngine;

public class AdvancePlayer_Range : EnemyState
{
    private Enemy_Range enemy;
    private Vector3 playerPos;

    public AdvancePlayer_Range(Enemy enemyBase, StateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.visuals.EnableIK(true, false);

        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.advanceSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        playerPos = enemy.player.transform.position;

        enemy.agent.SetDestination(enemy.player.transform.position);
        enemy.RotateFace(enemy.agent.steeringTarget);

        if (Vector3.Distance(enemy.transform.position, playerPos) < enemy.advanceStoppingDistance)
            stateMachine.ChangeState(enemy.battleState);
    }
}
