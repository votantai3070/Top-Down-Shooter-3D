using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData_EnemyMelee
{
    public string attackName;
    public float attackRange;
    public float attackMoveSpeed;
    public float attackIndex;
    [Range(1, 2)] public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee { Close, Charge }

public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }

public class Enemy_Melee : Enemy
{
    #region States
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }
    #endregion

    [Header("Attack data")]
    public AttackData_EnemyMelee attackData;
    public List<AttackData_EnemyMelee> attackList = new();

    [Header("Axe throwing ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float aimTimer;
    public float axeThrowCooldown;
    public Transform axeThrowStartPoint;
    private float lastAxeThrowTime;

    [Header("Enemy settings")]
    public EnemyMelee_Type meleeType;
    public Transform shieldTransform;
    [SerializeField] float dodgeRollCooldown = 5f;
    private float lastDodgeRollTime = -10;


    protected override void Awake()
    {
        base.Awake();


        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); // Use Ragdoll
        abilityState = new AbilityState_Melee(this, stateMachine, "Ability");

    }

    protected override void Start()
    {
        base.Start();

        InitialPerk();

        //attackData = visuals.CurrentWeaponModel().GetComponent<Enemy_WeaponModel>().weaponData.attackData;

        visuals.SetupRandomLook();

        lastDodgeRollTime = Time.time;

        stateMachine.Initialize(idleState);

        visuals.SetupWeaponLook();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        stateMachine.ChangeState(recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        EnableWeapon(false);
    }

    public void UpdateAttackData()
    {
        Enemy_WeaponModel currentWeaponModel = visuals.CurrentWeaponModel().GetComponent<Enemy_WeaponModel>();

        if (currentWeaponModel != null)
        {
            attackList = new(currentWeaponModel.weaponData.attackData);
            turnSpeed = currentWeaponModel.weaponData.turnSpeed;
        }
    }

    public void ActivateDodgeRoll()
    {
        if (meleeType != EnemyMelee_Type.Dodge)
            return;

        if (stateMachine.currentState != chaseState)
            return;

        if (Vector3.Distance(transform.position, player.position) < attackData.attackRange)
            return;

        float dodgeAnimationDuration = GetAnimationClipDuration("Stand To Roll");

        if (Time.time > lastDodgeRollTime + dodgeAnimationDuration + dodgeRollCooldown)
        {
            lastDodgeRollTime = Time.time;
            anim.SetTrigger("Dodge");
        }
    }

    private void InitialPerk()
    {
        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            visuals.SetupWeaponType(EnemyWeaponModelType.Throw);
        }
        else

        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            visuals.SetupWeaponType(EnemyWeaponModelType.OneHand);
        }

        if (meleeType == EnemyMelee_Type.Dodge)
            visuals.SetupWeaponType(EnemyWeaponModelType.Unarmed);
    }

    public override void GetHit()
    {
        base.GetHit();

        if (healthPoints <= 0)
            stateMachine.ChangeState(deadState);
    }


    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;
    public bool PlayerOutAttackRange() => Vector3.Distance(transform.position, player.position) > attackData.attackRange;

    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow)
            return false;

        if (Time.time > lastAxeThrowTime + axeThrowCooldown)
        {
            lastAxeThrowTime = Time.time;
            return true;
        }
        return false;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        Debug.LogWarning("Clip not found: " + clipName);
        return 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
        Gizmos.color = Color.blue;
    }

}
