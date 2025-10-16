using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;

    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.updateRotation = false;
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAggresionRange())
        {
            stateMachine.ChangeState(enemy.recoveryState);
            return;
        }

        enemy.RotateFace(enemy.agent.steeringTarget);

        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .5f)
            stateMachine.ChangeState(enemy.idleState);
    }



    public override void Exit()
    {
        base.Exit();
    }


    //private Vector3 GetNextPathPoint()
    //{
    //    NavMeshAgent agent = enemy.agent;
    //    NavMeshPath path = agent.path;

    //    if (path.corners.Length < 2)
    //        return agent.destination;

    //    for (int i = 0; i < path.corners.Length; i++)
    //    {
    //        if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
    //            return path.corners[i + 1];
    //    }

    //    return agent.destination;
    //}
}
