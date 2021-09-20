
using UnityEngine;
using EnemyStates;



public class BanditBoss_Movement : MonoBehaviour
{
    public BanditBoss_Vision banditBossVision = null;
    public EnemyAttributes enemyAttributes = null;
    
    public Animator anim = null;
    public Rigidbody rigidBody = null;

    public GameObject targetObject;

    public Vector3 originPoint;
    public Quaternion originRotation = Quaternion.identity;
    public Vector3 originForwardPoint;
    public bool isRotatingAfterReturningToOrigin = false;
    public float pointProximityRange = 1f;

    public float banditRotationSpeed = 4f;
    public float maxSpeed = 3.0f;

    public bool canSeePlayer = false;
    public bool canHitPlayer = false;
    public bool hasHitPlayer = false;
    private float timer = 0f;

    public float animSpeed = 0f; // for animation 
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int ReturnToOriginIdleDelay = Animator.StringToHash("ReturnToOriginIdleDelay");

    void Start()
    {
        if(!banditBossVision)
            banditBossVision = GetComponent<BanditBoss_Vision>();
        
        if(!enemyAttributes)
            enemyAttributes = GetComponent<EnemyAttributes>();

        if(!anim)
            anim = GetComponent<Animator>();
        
        targetObject = banditBossVision.targetObject;


        originPoint = transform.position;
        originRotation = transform.rotation;
        originForwardPoint = transform.position + (banditBossVision.localForward.position * 2f);
        
        //anim.SetTrigger("Walking");
    }
    
    
        void Update()
    {
        if (banditBossVision.activeEnemyState == CurrentState.Dead || banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.Dead)
            return;

        if (banditBossVision.activeEnemyState == CurrentState.Idle && banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.Idle)
            return;


        if (!enemyAttributes.GetMovementStatus())
        {
            return; // enemy cant move so return
        }
        
        transform.position = new Vector3(transform.position.x, GetComponent<CapsuleCollider>().bounds.size.y * 0.5f, transform.position.z);
        
        
        if (gameObject.name == "Enemy" && !canHitPlayer)// || gameObject.CompareTag("EnemyBoo"))
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
        
//       var visionRef = GetComponent<BanditThug_Vision>(); 
        //var playerRef = visionRef.player.GetComponent<PlayerMovement>();

        if (banditBossVision.player && banditBossVision.ConeOfVision() && Vector3.Distance(transform.position, banditBossVision.player.position) > banditBossVision.rangeOfVision || !banditBossVision.targetObject)
        {
            Debug.Log("BOSS Movement: targetObject null returning");
            return;
        }

        canSeePlayer = LookForPlayer();
            
        
        targetObject = banditBossVision.targetObject;
        
        AIHelpers.InputParameters inputData = new AIHelpers.InputParameters(gameObject.transform, targetObject.transform, Time.deltaTime, maxSpeed);
        AIHelpers.MovementResult movementResult = new AIHelpers.MovementResult();
        
        

        if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic)
            AIHelpers.SeekKinematic(inputData, ref movementResult);
        else if(banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.FleeKinematic)
            AIHelpers.FleeKinematic(inputData, ref movementResult);
        else if(banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.WanderKinematic)
            AIHelpers.WanderKinematic(inputData, ref movementResult);
        else if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
        {
            if (banditBossVision.activeEnemyState == CurrentState.Dead)
                return;
            
            Debug.Log("BOSS Movement: set return position");

            //targetObject = originPoint.gameObject;

            inputData = new AIHelpers.InputParameters(gameObject.transform, originPoint, Time.deltaTime, maxSpeed);

            Debug.Log("BOSS Movement: originPoint loc: " + originPoint);
            AIHelpers.BackToOriginPosKinematic(inputData, ref movementResult);
        }
        else
            AIHelpers.SearchKinematic(inputData, ref movementResult);
        
        
        var distance = Vector3.Distance(transform.position, banditBossVision.player.position);
        
        //anim.SetFloat(MoveSpeed, banditThugVision.ActiveMovementBehavior != AIHelpers.MovementBehaviors.Idle ? 1f : 0f);

        
        if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic && banditBossVision.activeEnemyState == CurrentState.BackToOrigin)
        {
             if (banditBossVision.ConeOfVision())
             {
                 if (banditBossVision.rayToPlayer.collider != null)
                 {
                     
                     Debug.Log("BOSS Movement: ray to player is not null");

                     if (banditBossVision.rayToPlayer.collider.CompareTag("Player"))
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
        else if (banditBossVision.activeEnemyState == CurrentState.ChasePlayer || banditBossVision.activeEnemyState == CurrentState.Attack || banditBossVision.activeEnemyState == CurrentState.Fleeing || banditBossVision.activeEnemyState == CurrentState.Patrolling)
        {
            if (banditBossVision.ConeOfVision())
            {
                if (banditBossVision.rayToPlayer.collider != null)
                {
                    if (banditBossVision.rayToPlayer.collider.CompareTag("Wall"))
                    {
                        Debug.Log("BOSS Movement ray to player is blocked by a wall, changing state");
                        
                        anim.SetTrigger(Idle);
                    }
                    else if (banditBossVision.rayToPlayer.collider.CompareTag("Player"))
                    {
                        Debug.Log("BOSS Movement: ray to player is not null, rotating thug");

                        // rotate toward X
                        var lookingDirection = targetObject.transform.position - transform.position;
                        lookingDirection.y = 0f;

                        var rotation = Quaternion.LookRotation(lookingDirection);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 6);
                    }
                }
            }
            else
            {
                Debug.Log("BOSS Movement player is not visible");
                        
                anim.SetTrigger(Idle);
            }
        }
        else if(banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic && banditBossVision.activeEnemyState == CurrentState.Idle && isRotatingAfterReturningToOrigin)
        {
            Debug.Log("BOSS Movement: rotating back to origin");
            
            // if idle and not looking in origin direction turn
            //if(Quaternion.Angle(transform.rotation, originRotation) > 0.1f)
            //{
                Debug.Log("BOSS Movement: rotating to origin");
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
            if(Quaternion.Angle(transform.rotation, originRotation) < Mathf.Epsilon)
            {
                Debug.Log("BOSS Movement: rotating to origin end");
                isRotatingAfterReturningToOrigin = false;
            }
        }
        // else if (visionRef.activeEnemyState == CurrentState.Searching)
        // {
        //     if (visionRef.lastKnownPlayersLocation.transform.position != Vector3.zero)
        //     {
        //         Debug.Log("going to last known player location");
        //
        //         // look at last known location
        //         var lookingDirection = visionRef.lastKnownPlayersLocation.transform.position - transform.position;
        //         lookingDirection.y = 0f;
        //
        //         var rotation = Quaternion.LookRotation(lookingDirection);
        //         transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
        //
        //         targetObject = visionRef.lastKnownPlayersLocation;
        //     }
        // }
        

        // reset Y position
        //movementResult.newPosition.y = 1.13f;
        //movementResult.newPosition.y = 0f; //GetComponent<CapsuleCollider>().bounds.size.y * 0.5f;

        
        
        
        
        
        // move if you can
        // if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.WanderKinematic)// && !gameObject.CompareTag("EnemyBoo"))
        // {
        //     if (visionRef.activeEnemyState == CurrentState.Patrolling)
        //     {
        //         gameObject.transform.position = movementResult.newPosition;
        //
        //         if (Mathf.Abs(distance) < 1.2f)
        //         {
        //             visionRef.player.GetComponent<PlayersHealth>().CurrentHealth -= enemyAttributes.AttackDamage;
        //
        //             canHitPlayer = false;
        //         }
        //     }
        //      
        // }
        // else if (activeMovementBehavior == AIHelpers.MovementBehaviors.SearchKinematic)// && gameObject.CompareTag("EnemyBoo"))
        // {
        //     if (visionRef.activeEnemyState == CurrentState.Searching)
        //     {
        //         //visionRef.activeEnemyState = CurrentState.Attack;
        //         Debug.Log("Moving to last players position");
        //
        //         var dist = Vector3.Distance(transform.position, visionRef.player.transform.position);
        //         
        //         gameObject.transform.position = movementResult.newPosition;
        //
        //         if (Mathf.Abs(dist) < 1.2f && canHitPlayer)
        //         {
        //             visionRef.player.GetComponent<PlayersHealth>().TakeDamageFromAttack(visionRef.GetComponent<EnemyAttributes>().AttackDamage, (visionRef.player.transform.position - transform.position));  //.CurrentHealth -= 50;
        //
        //             canHitPlayer = false;
        //         }
        //     }
        //     else
        //     {
        //         activeMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
        //         visionRef.activeEnemyState = CurrentState.Idle;
        //         Debug.Log("enemy stops searching going to idle state");
        //     }
        // }
        //else 
        if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)// && visionRef.activeEnemyState == CurrentState.Patrolling)
        {
            Debug.Log("BOSS Movement: returning to origin point From movement");
            
            float returnDistance = Vector3.Distance(transform.position, originPoint);

            //if (Mathf.Abs(returnDistance) >= 1f)
           // {
                //gameObject.transform.position = movementResult.newPosition;
                MovePosition(movementResult);
                
                //animSpeed = 1f;
                //UpdateAnimationMoveSpeed(1f);
                
                //}

        }
        else if (banditBossVision.activeEnemyState != CurrentState.Idle || banditBossVision.activeEnemyState != CurrentState.BackToOrigin)// && !gameObject.CompareTag("EnemyBoo"))
        {
            if(banditBossVision.ConeOfVision())
            {
                if (banditBossVision.rayToPlayer.collider != null)
                {
                    Debug.Log("BOSS Movement: ray to player is not null");

                    if (!banditBossVision.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f)
                    {
                        Debug.Log("BOSS Movement: ray to player, not hitting a wall, move towards player");

                        //gameObject.transform.position = movementResult.newPosition;
                        MovePosition(movementResult);
                        
                        //animSpeed = 1f;
                        //UpdateAnimationMoveSpeed(1f);


//                        if (banditGuardVision.activeEnemyState != CurrentState.Attack)
                        //                           banditGuardVision.activeEnemyState = CurrentState.Attack;
                    }
                    else if (banditBossVision.activeEnemyState == CurrentState.Fleeing)
                    {
                        Debug.Log("BOSS Movement: Fleeing use this direction");
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
        
        if(banditBossVision.activeEnemyState == CurrentState.Idle)
        {
            //animSpeed = 0f;
            //UpdateAnimationMoveSpeed(0f);
        }
        
        
    }

        

    // void Update()
    // {
    //     if (IsThisEnemyDead())
    //     {
    //         Debug.Log($"start of update. our states are movement behavior: {banditBossVision.ActiveMovementBehavior}, enemy state: {banditBossVision.activeEnemyState}");
    //         
    //         if (targetObject)
    //             targetObject = null;
    //
    //         return;
    //     }
    //     
    //     
    //     
    //     // if (gameObject.name == "Enemy" && !canHitPlayer)// || gameObject.CompareTag("EnemyBoo"))
    //     // {
    //     //     if (hasHitPlayer)
    //     //         return;
    //     //
    //     //     hasHitPlayer = true;
    //     //     
    //     //     timer += Time.deltaTime;
    //     //
    //     //     if (timer > 2f)
    //     //     {
    //     //         canHitPlayer = true;
    //     //
    //     //         timer = 0f;
    //     //     }
    //     //     hasHitPlayer = false;
    //     //     
    //     // }
    //     
    //     
    //     
    //     canSeePlayer = LookForPlayer();
    //
    //     if (!canSeePlayer)
    //         return;
    //     
    //     
    //     
    //     if (banditBossVision.ConeOfVision() && Vector3.Distance(transform.position, banditBossVision.player.position) > banditBossVision.rangeOfVision)
    //     {
    //         Debug.Log("targetObject null returning");
    //         return;
    //     }
    //     
    //     
    //     
    //     targetObject = banditBossVision.targetObject;
    //     
    //     
    //     
    //     
    //     AIHelpers.InputParameters inputData = new AIHelpers.InputParameters(gameObject.transform, targetObject.transform, Time.deltaTime, maxSpeed);
    //     AIHelpers.MovementResult movementResult = new AIHelpers.MovementResult();
    //     
    //     
    //
    //     if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic)
    //         AIHelpers.SeekKinematic(inputData, ref movementResult);
    //     // else if(banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.FleeKinematic)
    //     //     AIHelpers.FleeKinematic(inputData, ref movementResult);
    //     else if(banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.WanderKinematic)
    //         AIHelpers.WanderKinematic(inputData, ref movementResult);
    //     else if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
    //     {
    //         if (banditBossVision.activeEnemyState == CurrentState.Dead)
    //             return;
    //         
    //         Debug.Log("set return position");
    //
    //         //targetObject = originPoint.gameObject;
    //
    //         inputData = new AIHelpers.InputParameters(gameObject.transform, originPoint, Time.deltaTime, maxSpeed);
    //
    //         Debug.Log("originPoint loc: " + originPoint);
    //         AIHelpers.BackToOriginPosKinematic(inputData, ref movementResult);
    //     }
    //     // else
    //     //     AIHelpers.SearchKinematic(inputData, ref movementResult);
    //     
    //     
    //     var distance = Vector3.Distance(transform.position, banditBossVision.player.position);
    //     
    //     //anim.SetFloat(MoveSpeed, banditThugVision.ActiveMovementBehavior != AIHelpers.MovementBehaviors.Idle ? 1f : 0f);
    //     
    //     
    //     if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic && banditBossVision.activeEnemyState == CurrentState.BackToOrigin) 
    //     {
    //         // if (visionRef.ConeOfVision())
    //         // {
    //         //     if (visionRef.rayToPlayer.collider != null)
    //         //     {
    //         //         Debug.Log("ray to player is not null");
    //
    //                 // if (!visionRef.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f && distance <= visionRef.rangeOfVision)
    //                 // {
    //
    //                     var lookingDirection = originPoint - transform.position;
    //                     lookingDirection.y = 0f;
    //
    //                     var rotation = Quaternion.LookRotation(lookingDirection);
    //                     transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
    //         //         }
    //         //     }
    //         // }
    //         
    //         
    //         float returnDistance = Vector3.Distance(transform.position, originPoint);
    //
    //         if (Mathf.Abs(returnDistance) >= 1f)
    //         {
    //             gameObject.transform.position = movementResult.newPosition;
    //             MovePosition(movementResult);
    //             
    //         }
    //         
    //     }
    //     // may do reverse checks here, to minimize checks
    //     else if (banditBossVision.activeEnemyState == CurrentState.Attack || banditBossVision.activeEnemyState == CurrentState.Fleeing || banditBossVision.activeEnemyState == CurrentState.Patrolling)
    //     {
    //         if (banditBossVision.ConeOfVision())
    //         {
    //             //if (banditBossVision.rayToPlayer.collider != null)
    //             //{
    //                //if (banditBossVision.rayToPlayer.collider.CompareTag("Player"))
    //                 //{
    //                     Debug.Log("ray to player is not null, rotating thug");
    //
    //                     // rotate toward X
    //                     var lookingDirection = targetObject.transform.position - transform.position;
    //                     lookingDirection.y = 0f;
    //
    //                     var rotation = Quaternion.LookRotation(lookingDirection);
    //                     transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
    //                 //}
    //             //}
    //         }
    //     }
    //     else if(banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic && banditBossVision.activeEnemyState == CurrentState.Idle && isRotatingAfterReturningToOrigin)
    //     {
    //         Debug.Log("rotating back to origin");
    //         
    //         // if idle and not looking in origin direction turn
    //         //if(Quaternion.Angle(transform.rotation, originRotation) > 0.1f)
    //         //{
    //             Debug.Log("rotating to origin");
    //             float speed = 0.1f; 
    //             //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * speed);
    //             
    //             
    //             transform.rotation = Quaternion.Slerp(transform.rotation, originRotation, speed);
    //             
    //             //transform.rotation = Quaternion.Inverse(transform.rotation);
    //             
    //             
    //             // var lookingDirection = originForwardPoint - transform.position;
    //             // lookingDirection.y = 0f;
    //             //
    //             // var rotation = Quaternion.LookRotation(lookingDirection);
    //             // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 40);
    //             
    //         //}
    //         //else
    //         if(Quaternion.Angle(transform.rotation, originRotation) < Mathf.Epsilon)
    //         {
    //             Debug.Log("rotating to origin end");
    //             isRotatingAfterReturningToOrigin = false;
    //         }
    //     }
    //     // else if (visionRef.activeEnemyState == CurrentState.Searching)
    //     // {
    //     //     if (visionRef.lastKnownPlayersLocation.transform.position != Vector3.zero)
    //     //     {
    //     //         Debug.Log("going to last known player location");
    //     //
    //     //         // look at last known location
    //     //         var lookingDirection = visionRef.lastKnownPlayersLocation.transform.position - transform.position;
    //     //         lookingDirection.y = 0f;
    //     //
    //     //         var rotation = Quaternion.LookRotation(lookingDirection);
    //     //         transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
    //     //
    //     //         targetObject = visionRef.lastKnownPlayersLocation;
    //     //     }
    //     // }
    //     
    //
    //     // reset Y position
    //     movementResult.newPosition.y = 1f;//0.92f;
    //
    //     
    //     
    //     
    //     
    //     
    //     // move if you can
    //     // if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.WanderKinematic)// && !gameObject.CompareTag("EnemyBoo"))
    //     // {
    //     //     if (visionRef.activeEnemyState == CurrentState.Patrolling)
    //     //     {
    //     //         gameObject.transform.position = movementResult.newPosition;
    //     //
    //     //         if (Mathf.Abs(distance) < 1.2f)
    //     //         {
    //     //             visionRef.player.GetComponent<PlayersHealth>().CurrentHealth -= enemyAttributes.AttackDamage;
    //     //
    //     //             canHitPlayer = false;
    //     //         }
    //     //     }
    //     //      
    //     // }
    //     // else if (activeMovementBehavior == AIHelpers.MovementBehaviors.SearchKinematic)// && gameObject.CompareTag("EnemyBoo"))
    //     // {
    //     //     if (visionRef.activeEnemyState == CurrentState.Searching)
    //     //     {
    //     //         //visionRef.activeEnemyState = CurrentState.Attack;
    //     //         Debug.Log("Moving to last players position");
    //     //
    //     //         var dist = Vector3.Distance(transform.position, visionRef.player.transform.position);
    //     //         
    //     //         gameObject.transform.position = movementResult.newPosition;
    //     //
    //     //         if (Mathf.Abs(dist) < 1.2f && canHitPlayer)
    //     //         {
    //     //             visionRef.player.GetComponent<PlayersHealth>().TakeDamageFromAttack(visionRef.GetComponent<EnemyAttributes>().AttackDamage, (visionRef.player.transform.position - transform.position));  //.CurrentHealth -= 50;
    //     //
    //     //             canHitPlayer = false;
    //     //         }
    //     //     }
    //     //     else
    //     //     {
    //     //         activeMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
    //     //         visionRef.activeEnemyState = CurrentState.Idle;
    //     //         Debug.Log("enemy stops searching going to idle state");
    //     //     }
    //     // }
    //     //else 
    // // if (banditBossVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)// && visionRef.activeEnemyState == CurrentState.Patrolling)
    // // {
    // //     Debug.Log("returning to origin point From movement");
    // //     
    // //     float returnDistance = Vector3.Distance(transform.position, originPoint);
    // //
    // //     //if (Mathf.Abs(returnDistance) >= 1f)
    // //    // {
    // //         //gameObject.transform.position = movementResult.newPosition;
    // //         MovePosition(movementResult);
    // //     //}
    // //     
    // // }
    // // else 
    //     if (banditBossVision.activeEnemyState != CurrentState.Idle || banditBossVision.activeEnemyState != CurrentState.BackToOrigin)// && !gameObject.CompareTag("EnemyBoo"))
    //     {
    //         if(banditBossVision.ConeOfVision() && banditBossVision.rayToPlayer.collider != null)
    //         {
    //             if (banditBossVision.rayToPlayer.collider.CompareTag("Player"))
    //             {
    //                 
    //                 
    //                 Debug.Log("BOSS Movement: ray to player is not null");
    //
    //                 if (!banditBossVision.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f)
    //                 {
    //                     Debug.Log("BOSS Movement: ray to player, not hitting a wall, move towards player");
    //
    //                     //gameObject.transform.position = movementResult.newPosition;
    //                     MovePosition(movementResult);
    //
    //                     if (banditBossVision.activeEnemyState != CurrentState.Attack)
    //                     {
    //                         Debug.Log("BOSS Movement: can attack player they are in range and visible");
    //                         
    //                       //  banditBossVision.activeEnemyState = CurrentState.Attack;
    //                     }
    //                 }
    //                 // else if (banditBossVision.activeEnemyState == CurrentState.Fleeing)
    //                 // {
    //                 //     Debug.Log("Fleeing use this direction");
    //                 //     //gameObject.transform.position = movementResult.newPosition;
    //                 //
    //                 //     RotateTowardsTarget(targetObject.transform);
    //                 //     
    //                 //     // range limit later
    //                 //     movementResult.newPosition.y = 1f;
    //                 //     MovePosition(movementResult);
    //                 // }
    //                 // else if (visionRef.activeEnemyState == CurrentState.Searching)
    //                 // {
    //                 //     Debug.Log("SEARCHING! ");
    //                 //     gameObject.transform.position = movementResult.newPosition;
    //                 // }
    //                 
    //             }
    //
    //             if (banditBossVision.rayToPlayer.collider.CompareTag("Wall"))
    //             {
    //                 Debug.Log("BOSS Movement: player behind wall, go to idle");
    //                 
    //                 anim.SetTrigger(Idle);
    //             }
    //         }
    //         // else
    //         // {
    //         //     Debug.Log("BOSS Movement: go to a idle state");
    //         //     
    //         //     anim.SetTrigger(Idle);
    //         // }
    //     }
    //     // else if(banditBossVision.activeEnemyState == CurrentState.SearchPlayersLastKnownPOS)
    //     // {
    //     //     Debug.Log("searching for the player, movement");
    //     //     
    //     //     // move to last known location
    //     //     if (banditBossVision.rayToLastKnownPOS.collider.CompareTag("PlayersLastKnownPOS"))
    //     //     {
    //     //         Debug.Log("going to last POS");
    //     //         
    //     //         RotateTowardsTarget(targetObject.transform);
    //     //
    //     //         // range limit later
    //     //         movementResult.newPosition.y = 0.92f;
    //     //
    //     //         gameObject.transform.position = movementResult.newPosition;
    //     //     }
    //     //     else if (banditBossVision.rayToLastKnownPOS.collider.CompareTag("Wall"))
    //     //     {
    //     //         Debug.Log("going to last POS but wall is in the way");
    //     //         anim.SetTrigger(ReturnToOriginIdleDelay);
    //     //     }
    //     //         
    //     // }
    //     
    //     
    //
    //     canSeePlayer = false;
    //
    // }
    
    
    
    internal bool LookForPlayer() => banditBossVision.ConeOfVision();

    // returns true if you can see the player and the raycast is not null
    internal bool CanSeePlayer() => (IsThisEnemyDead() == false && banditBossVision.rayToPlayer.collider != null);

    internal bool GetRayToPlayer()
    {
        if (canSeePlayer && enemyAttributes.GetCurrentState() != CurrentState.Dead)
            if(banditBossVision.rayToPlayer.collider != null)
                return true;

        return false;
    }
    
    
    internal bool IsThisEnemyDead() =>  (canSeePlayer && (enemyAttributes.GetCurrentState() == CurrentState.Dead  || enemyAttributes.GetCurrentMovementBehavior() == AIHelpers.MovementBehaviors.Dead));
    
    

    private void MovePosition(AIHelpers.MovementResult newMovePos)
    {
        if (banditBossVision.activeEnemyState == CurrentState.Dead)
            return;
        
        Debug.Log("BOSS Movement: rb move");
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
        
        Debug.Log("BOSS Movement: rotating towards target");
        
        var lookingDirection = target.position - transform.position;
        lookingDirection.y = 0f;
        
        var rotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * banditRotationSpeed);
    }
    
    
}
