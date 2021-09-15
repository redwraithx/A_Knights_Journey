
using UnityEngine;
using EnemyStates;


public class BanditThug_Movement : MonoBehaviour
{
    public BanditThug_Vision banditThugVision = null;
    public EnemyAttributes enemyAttributes = null;
    
    public Animator anim = null;
    public Rigidbody rigidBody = null;
    
    public GameObject targetObject;

    public Vector3 originPoint;
    public Quaternion originRotation = Quaternion.identity;
    public Vector3 originForwardPoint;
    public bool isRotatingAfterReturningToOrigin = false;
    public float originStoppingMinimumRange = 1f;

    public float banditRotationSpeed = 4f;
    public float maxSpeed = 3.0f;

    public bool canSeePlayer = false;
    public bool canHitPlayer = false;
    public bool hasHitPlayer = false;
    private float timer = 0f;

    public float animSpeed = 0f;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int ReturnToOriginIdleDelay = Animator.StringToHash("ReturnToOriginIdleDelay");
    private static readonly int Attack = Animator.StringToHash("Attack");


    void Start()
    {
        banditThugVision = GetComponent<BanditThug_Vision>();
        
        enemyAttributes = GetComponent<EnemyAttributes>();

        anim = GetComponent<Animator>();

        targetObject = banditThugVision.targetObject;

        originPoint = transform.position;
        originRotation = transform.rotation;
        originForwardPoint = transform.position + (banditThugVision.localForward.position * 2f);
        
    }

    void Update()
    {
        if (IsThisEnemyDead())
        {
            Debug.Log($"start of update. our states are movement behavior: {banditThugVision.ActiveMovementBehavior}, enemy state: {banditThugVision.activeEnemyState}");

            if (targetObject)
                targetObject = null;

            return;
        }


        transform.position = new Vector3(transform.position.x, GetComponent<CapsuleCollider>().bounds.size.y * 0.5f, transform.position.z);


        if (gameObject.name == "Enemy" && !canHitPlayer)
        {
            if (hasHitPlayer)
                return;

            hasHitPlayer = true;

            timer += Time.deltaTime;

            if (timer > 2f)
            {
                canHitPlayer = true;

                timer = 0f;
            }

            hasHitPlayer = false;

        }


        if (banditThugVision.player && banditThugVision.ConeOfVision() && Vector3.Distance(transform.position, banditThugVision.player.position) > banditThugVision.rangeOfVision)
        {
            Debug.Log("THUG Movement: targetObject null returning");
            return;
        }
        


        canSeePlayer = LookForPlayer();

        targetObject = banditThugVision.targetObject;

        // update movement and package up inputData
        AIHelpers.InputParameters inputData = new AIHelpers.InputParameters(gameObject.transform, targetObject.transform, Time.deltaTime, maxSpeed);
        AIHelpers.MovementResult movementResult = new AIHelpers.MovementResult();

        AIHelpers.BackToOriginPosKinematic(inputData, ref movementResult);

        if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic)
            AIHelpers.SeekKinematic(inputData, ref movementResult);
        else if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.FleeKinematic)
            AIHelpers.FleeKinematic(inputData, ref movementResult);
        else if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.WanderKinematic)
            AIHelpers.WanderKinematic(inputData, ref movementResult);
        else if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
        {
            if (banditThugVision.activeEnemyState == CurrentState.Dead)
                return;

            Debug.Log("THUG Movement: set return position");

            inputData = new AIHelpers.InputParameters(gameObject.transform, originPoint, Time.deltaTime, maxSpeed);

            Debug.Log("THUG Movement: originPoint Loc: " + originPoint);

            AIHelpers.BackToOriginPosKinematic(inputData, ref movementResult);

        }
        else
            AIHelpers.SearchKinematic(inputData, ref movementResult);




        var distance = Vector3.Distance(transform.position, banditThugVision.player.position);

        if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic && banditThugVision.activeEnemyState == CurrentState.BackToOrigin)
        {
            if (banditThugVision.ConeOfVision())
            {
                if (banditThugVision.rayToPlayer.collider != null)
                {

                    Debug.Log("THUG Movement: ray to player is not null");

                    if (banditThugVision.rayToPlayer.collider.CompareTag("Player"))
                    {
                        var lookingDirection = originPoint - transform.position;
                        lookingDirection.y = 0f;

                        var rotation = Quaternion.LookRotation(lookingDirection);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
                    }

                }
            }
        }
        // may do reverse checks here, to minimize checks
        else if (banditThugVision.activeEnemyState == CurrentState.ChasePlayer || banditThugVision.activeEnemyState == CurrentState.Attack ||
                 banditThugVision.activeEnemyState == CurrentState.Fleeing || banditThugVision.activeEnemyState == CurrentState.Patrolling)
        {
            if (banditThugVision.ConeOfVision())
            {
                if (banditThugVision.rayToPlayer.collider != null)
                {
                    if (banditThugVision.rayToPlayer.collider.CompareTag("Wall"))
                    {
                        Debug.Log("THUG Movement ray to player is blocked by a wall, changing state");

                        anim.SetTrigger(Idle);
                    }
                    else if (banditThugVision.rayToPlayer.collider.CompareTag("Player"))
                    {
                        Debug.Log("THUG Movement: ray to player is not null, rotating thug");

                        if (Vector3.Distance(transform.position, banditThugVision.rayToPlayer.collider.transform.position) <= enemyAttributes.GetAttackRange())
                        {
                            Debug.Log("THUG Movement: attacking player transition");

                            anim.SetTrigger(Attack);
                        }
                        
                        
                        // rotate toward X
                        var lookingDirection = targetObject.transform.position - transform.position;
                        lookingDirection.y = 0f;

                        var rotation = Quaternion.LookRotation(lookingDirection);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);

                        
                    }
                }
            }
            else
            {
                Debug.Log("THUG Movement player is not visible");

                anim.SetTrigger(Idle);
            }
        }
        else if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic && banditThugVision.activeEnemyState == CurrentState.Idle &&
                 isRotatingAfterReturningToOrigin)
        {
            Debug.Log("THUG Movement: rotating back to origin");

            Debug.Log("THUG Movement: rotating to origin");
            float speed = 0.1f;

            transform.rotation = Quaternion.Slerp(transform.rotation, originRotation, speed);

            if (Quaternion.Angle(transform.rotation, originRotation) < Mathf.Epsilon)
            {
                Debug.Log("THUG Movement: rotating to origin end");
                isRotatingAfterReturningToOrigin = false;
            }
        }

        if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic) // && visionRef.activeEnemyState == CurrentState.Patrolling)
        {
            Debug.Log("THUG Movement: returning to origin point From movement");

            float returnDistance = Vector3.Distance(transform.position, originPoint);

            MovePosition(movementResult);

        }
        else if (banditThugVision.activeEnemyState != CurrentState.Idle || banditThugVision.activeEnemyState != CurrentState.BackToOrigin) // && !gameObject.CompareTag("EnemyBoo"))
        {
            if (banditThugVision.ConeOfVision())
            {
                if (banditThugVision.rayToPlayer.collider != null)
                {
                    Debug.Log("THUG Movement: ray to player is not null");

                    if (!banditThugVision.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f)
                    {
                        Debug.Log("THUG Movement: ray to player, not hitting a wall, move towards player");

                        MovePosition(movementResult);

                    }
                    else if (banditThugVision.activeEnemyState == CurrentState.Fleeing)
                    {
                        Debug.Log("THUG Movement: Fleeing use this direction");
                        MovePosition(movementResult);
                    }

                }
            }
        }

        if (banditThugVision.activeEnemyState == CurrentState.Idle)
        {
        }
    }

    internal bool LookForPlayer() => banditThugVision.ConeOfVision();

    // returns true if you can see the player and the raycast is not null
    internal bool CanSeePlayer() => (IsThisEnemyDead() == false && banditThugVision.rayToPlayer.collider != null);

    internal bool GetRayToPlayer()
    {
        if (canSeePlayer && enemyAttributes.GetCurrentState() != CurrentState.Dead)
            if(banditThugVision.rayToPlayer.collider != null)
                return true;

        return false;
    }


    internal bool IsThisEnemyDead() =>  (canSeePlayer && (enemyAttributes.GetCurrentState() == CurrentState.Dead  || enemyAttributes.GetCurrentMovementBehavior() == AIHelpers.MovementBehaviors.Dead));



    private void MovePosition(AIHelpers.MovementResult newMovePos)
    {
        if (banditThugVision.activeEnemyState == CurrentState.Dead)
            return;
    
        Debug.Log("THUG Movement: rb move");
        gameObject.transform.position = newMovePos.newPosition;
    }


    // this is a value of 0f for idle or 1f for moving
    public void UpdateAnimationMoveSpeed(float value) => animSpeed = value;


    public void RotateTowardsTarget(Transform target)
    {
        if (IsThisEnemyDead())
            return;
    
        Debug.Log("THUG Movement: rotating towards target");
    
        var lookingDirection = target.position - transform.position;
        lookingDirection.y = 0f;
    
        var rotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * banditRotationSpeed);
    }
    
    
}
