
using UnityEngine;
using EnemyStates;


public class BanditGuard_Movement : MonoBehaviour
{
    public BanditGuard_Vision banditGuardVision = null;
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


    //public float animSpeed = 0f; // for animation

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
        banditGuardVision = GetComponent<BanditGuard_Vision>();
        
        enemyAttributes = GetComponent<EnemyAttributes>();

        anim = GetComponent<Animator>();

        targetObject = banditGuardVision.targetObject;

        //originPoint = Instantiate(new GameObject(), transform).transform;
        
        originPoint = transform.position;
        originRotation = transform.rotation;
        originForwardPoint = transform.position + (banditGuardVision.localForward.position * 2f);
        
    }

    void Update()
    {
        if (IsThisEnemyDead())
        {
            Debug.Log($"start of update. our states are movement behavior: {banditGuardVision.ActiveMovementBehavior}, enemy state: {banditGuardVision.activeEnemyState}");

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


        if (banditGuardVision.player && banditGuardVision.ConeOfVision() && Vector3.Distance(transform.position, banditGuardVision.player.position) > banditGuardVision.rangeOfVision)
        {
            Debug.Log("Guard Movement: targetObject null returning");
            return;
        }
        


        canSeePlayer = LookForPlayer();

        targetObject = banditGuardVision.targetObject;

        // update movement and package up inputData
        AIHelpers.InputParameters inputData = new AIHelpers.InputParameters(gameObject.transform, targetObject.transform, Time.deltaTime, maxSpeed);
        AIHelpers.MovementResult movementResult = new AIHelpers.MovementResult();

        AIHelpers.BackToOriginPosKinematic(inputData, ref movementResult);

        if (banditGuardVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic)
            AIHelpers.SeekKinematic(inputData, ref movementResult);
        else if (banditGuardVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.FleeKinematic)
            AIHelpers.FleeKinematic(inputData, ref movementResult);
        else if (banditGuardVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.WanderKinematic)
            AIHelpers.WanderKinematic(inputData, ref movementResult);
        else if (banditGuardVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
        {
            if (banditGuardVision.activeEnemyState == CurrentState.Dead)
                return;

            Debug.Log("Guard Movement: set return position");

            inputData = new AIHelpers.InputParameters(gameObject.transform, originPoint, Time.deltaTime, maxSpeed);

            Debug.Log("Guard Movement: originPoint Loc: " + originPoint);

            AIHelpers.BackToOriginPosKinematic(inputData, ref movementResult);

        }
        else
            AIHelpers.SearchKinematic(inputData, ref movementResult);




        var distance = Vector3.Distance(transform.position, banditGuardVision.player.position);

        //anim.SetFloat(MoveSpeed, banditGuardVision.ActiveMovementBehavior != AIHelpers.MovementBehaviors.Idle ? 1f : 0f);


        if (banditGuardVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic && banditGuardVision.activeEnemyState == CurrentState.BackToOrigin)
        {
            if (banditGuardVision.ConeOfVision())
            {
                if (banditGuardVision.rayToPlayer.collider != null)
                {

                    Debug.Log("Guard Movement: ray to player is not null");

                    if (banditGuardVision.rayToPlayer.collider.CompareTag("Player"))
                    {
                        //if (!banditBossVision.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f && distance <= banditBossVision.rangeOfVision)
                        //{

                        var lookingDirection = originPoint - transform.position;
                        lookingDirection.y = 0f;

                        var rotation = Quaternion.LookRotation(lookingDirection);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
                        //}
                    }

                }
            }
        }
        // may do reverse checks here, to minimize checks
        else if (banditGuardVision.activeEnemyState == CurrentState.ChasePlayer || banditGuardVision.activeEnemyState == CurrentState.Attack ||
                 banditGuardVision.activeEnemyState == CurrentState.Fleeing || banditGuardVision.activeEnemyState == CurrentState.Patrolling)
        {
            if (banditGuardVision.ConeOfVision())
            {
                if (banditGuardVision.rayToPlayer.collider != null)
                {
                    if (banditGuardVision.rayToPlayer.collider.CompareTag("Wall"))
                    {
                        Debug.Log("Guard Movement ray to player is blocked by a wall, changing state");

                        anim.SetTrigger(Idle);
                    }
                    else if (banditGuardVision.rayToPlayer.collider.CompareTag("Player"))
                    {
                        Debug.Log("Guard Movement: ray to player is not null, rotating Guard");

                        if (Vector3.Distance(transform.position, banditGuardVision.rayToPlayer.collider.transform.position) <= enemyAttributes.GetAttackRange())
                        {
                            Debug.Log("Guard Movement: attacking player transition");

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
                Debug.Log("Guard Movement player is not visible");

                anim.SetTrigger(Idle);
            }
        }
        else if (banditGuardVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic && banditGuardVision.activeEnemyState == CurrentState.Idle &&
                 isRotatingAfterReturningToOrigin)
        {
            Debug.Log("Guard Movement: rotating back to origin");

            // if idle and not looking in origin direction turn
            //if(Quaternion.Angle(transform.rotation, originRotation) > 0.1f)
            //{
            Debug.Log("Guard Movement: rotating to origin");
            float speed = 0.1f;
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * speed);


            transform.rotation = Quaternion.Slerp(transform.rotation, originRotation, speed);

            //transform.rotation = Quaternion.Inverse(transform.rotation);


            // var lookingDirection = originForwardPoint - transform.position;
            // lookingDirection.y = 0f;
            //
            // var rotation = Quaternion.LookRotation(lookingDirection);
            // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 40);

            //}
            //else
            if (Quaternion.Angle(transform.rotation, originRotation) < Mathf.Epsilon)
            {
                Debug.Log("Guard Movement: rotating to origin end");
                isRotatingAfterReturningToOrigin = false;
            }
        }

        if (banditGuardVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic) // && visionRef.activeEnemyState == CurrentState.Patrolling)
        {
            Debug.Log("Guard Movement: returning to origin point From movement");

            float returnDistance = Vector3.Distance(transform.position, originPoint);

            //if (Mathf.Abs(returnDistance) >= 1f)
            // {
            //gameObject.transform.position = movementResult.newPosition;
            MovePosition(movementResult);

            //animSpeed = 1f;
            //UpdateAnimationMoveSpeed(1f);

            //}

        }
        else if (banditGuardVision.activeEnemyState != CurrentState.Idle || banditGuardVision.activeEnemyState != CurrentState.BackToOrigin) // && !gameObject.CompareTag("EnemyBoo"))
        {
            if (banditGuardVision.ConeOfVision())
            {
                if (banditGuardVision.rayToPlayer.collider != null)
                {
                    Debug.Log("Guard Movement: ray to player is not null");

                    if (!banditGuardVision.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f)
                    {
                        Debug.Log("Guard Movement: ray to player, not hitting a wall, move towards player");

                        //gameObject.transform.position = movementResult.newPosition;
                        MovePosition(movementResult);

                        //animSpeed = 1f;
                        //UpdateAnimationMoveSpeed(1f);


//                        if (banditGuardVision.activeEnemyState != CurrentState.Attack)
                        //                           banditGuardVision.activeEnemyState = CurrentState.Attack;
                    }
                    else if (banditGuardVision.activeEnemyState == CurrentState.Fleeing)
                    {
                        Debug.Log("Guard Movement: Fleeing use this direction");
                        //gameObject.transform.position = movementResult.newPosition;
                        MovePosition(movementResult);
                    }
                    // else if (visionRef.activeEnemyState == CurrentState.Searching)
                    // {
                    //     Debug.Log("SEARCHING! ");
                    //     gameObject.transform.position = movementResult.newPosition;
                    // }

                }
            }
        }

        if (banditGuardVision.activeEnemyState == CurrentState.Idle)
        {
            //animSpeed = 0f;
            //UpdateAnimationMoveSpeed(0f);
        }
    }

    internal bool LookForPlayer() => banditGuardVision.ConeOfVision();

    // returns true if you can see the player and the raycast is not null
    internal bool CanSeePlayer() => (IsThisEnemyDead() == false && banditGuardVision.rayToPlayer.collider != null);

    internal bool GetRayToPlayer()
    {
        if (canSeePlayer && enemyAttributes.GetCurrentState() != CurrentState.Dead)
            if(banditGuardVision.rayToPlayer.collider != null)
                return true;

        return false;
    }


    internal bool IsThisEnemyDead() =>  (canSeePlayer && (enemyAttributes.GetCurrentState() == CurrentState.Dead  || enemyAttributes.GetCurrentMovementBehavior() == AIHelpers.MovementBehaviors.Dead));

    
    public void RotateTowardsPlayer()
    {
        var lookingDirection = GameManager.Instance.playerReference.transform.position - transform.position;
        lookingDirection.y = 0f;

        var rotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
    }


    private void MovePosition(AIHelpers.MovementResult newMovePos)
    {
        if (banditGuardVision.activeEnemyState == CurrentState.Dead)
            return;
    
        Debug.Log("Guard Movement: rb move");
        gameObject.transform.position = newMovePos.newPosition;
    
        //rigidBody.MovePosition(newMovePos.newPosition);
    
        // update the bandits movement animation idle, walk
        //Debug.Log("enemy movement: " + rigidBody.velocity.magnitude);
        //anim.SetFloat(MoveSpeed, rigidBody.velocity.magnitude);

    
    }


    // this is a value of 0f for idle or 1f for moving
    public void UpdateAnimationMoveSpeed(float value) => animSpeed = value;


    public void RotateTowardsTarget(Transform target)
    {
        if (IsThisEnemyDead())
            return;
    
        Debug.Log("Guard Movement: rotating towards target");
    
        var lookingDirection = target.position - transform.position;
        lookingDirection.y = 0f;
    
        var rotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * banditRotationSpeed);
    }
}
