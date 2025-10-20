using UnityEngine;

public class AbilityState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    private Vector3 movementDirection;
    private const float MAX_MOVEMENT_DISTANCE = 20;
    private float moveSpeed;

    public AbilityState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        moveSpeed = enemy.moveSpeed;
        movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.moveSpeed = moveSpeed;
        enemy.anim.SetFloat("RecoveryIndex", 0);
        enemy.anim.SetFloat("ChaseIndex", 0);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.RotateFace(enemy.player.position);
            movementDirection = enemy.transform.position + (enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        }

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, movementDirection, enemy.moveSpeed * Time.deltaTime);
        }



        if (triggerCalled)
        {
            //if (enemy.PlayerOutAttackRange())
            //{
            //    stateMachine.ChangeState(enemy.chaseState);
            //    return;
            //}
            stateMachine.ChangeState(enemy.recoveryState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        GameObject axe = ObjectPool.instance.GetObject(enemy.axePrefab);
        axe.transform.position = enemy.axeThrowStartPoint.position;

        axe.GetComponent<EnemyAxe>().AxeSetup(enemy.axeFlySpeed, enemy.player, enemy.aimTimer);
    }
}
