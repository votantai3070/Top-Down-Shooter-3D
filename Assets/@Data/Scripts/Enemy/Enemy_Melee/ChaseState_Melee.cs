using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    public Enemy_Melee enemy;
    public float lastTimeUpdateDistanation;

    public ChaseState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();


        enemy.agent.isStopped = false;

        enemy.agent.speed = enemy.chaseSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.RotateFace(enemy.player.transform.position);

        if (CanUpdateDestinaion())
            enemy.agent.destination = enemy.player.transform.position;


        if (enemy.PlayerInAttackRange())
            stateMachine.ChangeState(enemy.attackState);
    }

    private bool CanUpdateDestinaion()
    {
        if (Time.time > lastTimeUpdateDistanation + .25f)
        {
            lastTimeUpdateDistanation = Time.time;
            return true;
        }

        return false;
    }
}
