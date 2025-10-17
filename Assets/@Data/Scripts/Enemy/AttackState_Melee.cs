using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 attackDirection;
    private float attackMoveSpeed;

    private const float MAX_ATTACK_DISTANCE = 50f;

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        attackMoveSpeed = enemy.attackData.attackMoveSpeed;
        enemy.anim.SetFloat("AttackAnimationSpeed", enemy.attackData.animationSpeed);


        //enemy.PullWeapon();

        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex);

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.anim.SetFloat("RecoveryIndex", 0);

        if (enemy.PlayerInAttackRange())
            enemy.anim.SetFloat("RecoveryIndex", 1);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualRotationActive())
        {
            enemy.RotateFace(enemy.player.position);
            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position =
                Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            if (enemy.PlayerInAttackRange())
                stateMachine.ChangeState(enemy.recoveryState);
            else
                stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
