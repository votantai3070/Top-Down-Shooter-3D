using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk { Unavailable, CanTakeCover, CanTakeAndChangeCover }
public enum UnstoppablePerk { Unavailable, CanBecomeUnstoppable }
public enum GrenadePerk { Unavailable, CanThrowGrenade }
public class Enemy_Range : Enemy
{
    [Header("Cover perks")]
    public CoverPerk coverPerk;
    public UnstoppablePerk unstoppablePerk;
    public GrenadePerk grenadePerk;

    [Header("Grenade perks")]
    public float grenadeCooldown;
    private float lastTimeThrewGrenade = -10;

    [Header("Advance perks")]
    public float advanceSpeed;
    public float advanceStoppingDistance;
    public float advanceDuration = 2.5f;

    [Header("Cover system")]
    public float safeDistance = 3f;
    public float minCoverTime;
    public CoverPoint lastCover { get; private set; }
    public CoverPoint currentCover { get; private set; }

    public bool canUseCover = true;
    public List<Cover> allCovers = new();

    [Header("Weapon details")]
    public float attackDelay;
    public Enemy_RangeWeaponType weaponType;
    public Enemy_RangeWeaponData weaponData;

    [Space]
    public Transform gunPoint;
    public Transform weaponHolder;
    public GameObject bulletPrefab;

    [Header("Aim details")]
    public float slowAim = 4;
    public float fastAim = 4;
    public Transform playerBody { get; private set; }
    public Transform aim;
    public LayerMask whatToIgnore;

    [SerializeField] List<Enemy_RangeWeaponData> availableWeaponData;

    #region States
    public IdleState_Range idleState { get; private set; }
    public MoveState_Range moveState { get; private set; }
    public BattleState_Range battleState { get; private set; }
    public RunToCoverState_Range runToCoverState { get; private set; }
    public AdvancePlayer_Range advancePlayerState { get; private set; }
    public ThrowGrenadeState_Range throwGrenadeState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayer_Range(this, stateMachine, "Advance");
        throwGrenadeState = new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
    }

    protected override void Start()
    {
        base.Start();

        playerBody = player.GetComponent<Player>().playerBody;
        aim.parent = null;

        InitializePerk();

        stateMachine.Initialize(idleState);

        visuals.SetupLook();
        SetupWeaponData();

        allCovers.AddRange(CollectNearByCovers());
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    protected override void InitializePerk()
    {
        if (IsUnstoppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("AdvanceAnimIndex", 1f);
        }
    }

    public bool CanThrowGrenade()
    {
        if (grenadePerk == GrenadePerk.Unavailable)
            return false;

        if (Vector3.Distance(player.position, transform.position) < safeDistance)
            return false;

        if (grenadePerk == GrenadePerk.CanThrowGrenade)
        {
            if (Time.time >= lastTimeThrewGrenade + grenadeCooldown)
                return true;
        }
        return false;
    }

    public void ThrowGrenade()
    {
        lastTimeThrewGrenade = Time.time;
        Debug.Log("Grenade Thrown!");
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        if (CanGetCover())
            stateMachine.ChangeState(runToCoverState);
        else
            stateMachine.ChangeState(battleState);
    }

    #region Cover System

    public bool CanGetCover()
    {
        if (coverPerk == CoverPerk.Unavailable)
            return false;

        currentCover = AttempToFindCover()?.GetComponent<CoverPoint>();

        if (lastCover != currentCover && currentCover != null)
            return true;

        Debug.LogWarning("No cover found!");
        return false;
    }

    private Transform AttempToFindCover()
    {
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

        foreach (var cover in allCovers)
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        CoverPoint closestCoverPoint = null;
        float shortestDistance = float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);

            if (currentDistance < shortestDistance)
            {
                closestCoverPoint = coverPoint;
                shortestDistance = currentDistance;
            }
        }

        if (closestCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;

            currentCover = closestCoverPoint;
            currentCover.SetOccupied(true);

            return currentCover.transform;
        }

        return null;
    }

    private List<Cover> CollectNearByCovers()
    {
        float coverRadiusCheck = 30;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRadiusCheck);
        List<Cover> collectedCovers = new List<Cover>();

        foreach (Collider collider in hitColliders)
        {
            Cover cover = collider.GetComponent<Cover>();

            if (cover != null && !collectedCovers.Contains(cover))
                collectedCovers.Add(cover);
        }

        return collectedCovers;
    }

    #endregion

    public void FireSingleBullet()
    {
        anim.SetTrigger("Shoot");

        Vector3 bulletDirection = (aim.position - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<Enemy_Bullet>().BulletSetup();

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.linearVelocity = bulletDirectionWithSpread * weaponData.bulletSpeed;
    }

    private void SetupWeaponData()
    {
        List<Enemy_RangeWeaponData> filteredData = new List<Enemy_RangeWeaponData>();

        foreach (var weapon in availableWeaponData)
        {
            if (weapon.weaponType == weaponType)
                filteredData.Add(weapon);
        }

        if (filteredData.Count > 0)
        {
            int random = Random.Range(0, filteredData.Count);
            weaponData = filteredData[random];
        }
        else
            Debug.Log("No Avaiable Weapon");

        gunPoint = visuals.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;
    }

    #region Aim System
    public void UpdateAimPosition()
    {
        float aimSpeed = IsAimOnPlayer() ? fastAim : slowAim;

        aim.position = Vector3.MoveTowards(aim.position, playerBody.position, aimSpeed * Time.deltaTime);
    }

    public bool IsAimOnPlayer()
    {
        float distanceAimToPlayer = Vector3.Distance(aim.position, player.position);

        return distanceAimToPlayer < 2f;
    }

    public bool IsSeeingPlayer()
    {
        Vector3 enemyPos = transform.position + Vector3.up;
        Vector3 directionToPlayer = playerBody.position - enemyPos;

        if (Physics.Raycast(enemyPos, directionToPlayer.normalized, out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
        {
            if (hit.collider.GetComponent<Player>())
            {
                UpdateAimPosition();
                return true;
            }
        }

        return false;
    }
    #endregion

    public bool IsUnstoppable()
    {
        return unstoppablePerk == UnstoppablePerk.CanBecomeUnstoppable;
    }
}
