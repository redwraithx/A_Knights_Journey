using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;



public class BanditArcher_Movement : MonoBehaviour
{
    public BanditArcher_Vision banditArcherVision = null;
    
    public GameObject targetObject;

    public Vector3 originPoint = Vector3.zero;
    public Quaternion originRotation = Quaternion.identity;
    public Vector3 originForwardPoint = Vector3.zero;
    public bool isRotatingAfterReturningToOrigin = false;
    public float pointProximityRange = 1f;
    
    public float maxSpeed = 3.0f;
    public float animSpeed = 0f; // for animation 

    public bool canHitPlayer = false;
    public bool hasHitPlayer = false;
    private float timer = 0f;

    public EnemyAttributes enemyAttributes = null;
    public Animator anim = null;
    public Rigidbody rigidBody = null;
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Idle = Animator.StringToHash("Idle");

    void Start()
    {
        banditArcherVision = GetComponent<BanditArcher_Vision>();
        
        enemyAttributes = GetComponent<EnemyAttributes>();

        targetObject = banditArcherVision.targetObject;

        originPoint = transform.position;
        originRotation = transform.rotation;
        originForwardPoint = transform.position + (banditArcherVision.localForward.position * 2f);
        
        //anim.SetTrigger("Walking");
    }

    void Update()
    {
        Debug.Log("animation move speed: " + anim.GetFloat(MoveSpeed));
        anim.SetFloat(MoveSpeed, animSpeed);
        
        // if(animSpeed > 0 && !anim.GetBool(IsWalking))
        //     anim.SetBool(IsWalking, true);
        // else
        //     anim.SetBool(IsWalking, false);
        //
        
        
        
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
        
        var visionRef = GetComponent<BanditThug_Vision>(); 
        //var playerRef = visionRef.player.GetComponent<PlayerMovement>();

        if (visionRef.ConeOfVision() && Vector3.Distance(transform.position, visionRef.player.position) > visionRef.rangeOfVision)
        {
            Debug.Log("targetObject null returning");
            return;
        }
            
        
        targetObject = visionRef.targetObject;
        
        AIHelpers.InputParameters inputData = new AIHelpers.InputParameters(gameObject.transform, targetObject.transform, Time.deltaTime, maxSpeed);
        AIHelpers.MovementResult movementResult = new AIHelpers.MovementResult();
        
        

        if (banditArcherVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.SeekKinematic)
            AIHelpers.SeekKinematic(inputData, ref movementResult);
        else if(banditArcherVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.FleeKinematic)
            AIHelpers.FleeKinematic(inputData, ref movementResult);
        else if(banditArcherVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.WanderKinematic)
            AIHelpers.WanderKinematic(inputData, ref movementResult);
        else if (banditArcherVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
        {
            Debug.Log("set return position");

            //targetObject = originPoint.gameObject;

            inputData = new AIHelpers.InputParameters(gameObject.transform, originPoint, Time.deltaTime, maxSpeed);

            Debug.Log("originPoint loc: " + originPoint);
            AIHelpers.BackToOriginPosKinematic(inputData, ref movementResult);
        }
        else
        {
            //AIHelpers.ArcherKinematic(inputData, ref movementResult);
            
            
            
            
        }
        
        
        var distance = Vector3.Distance(transform.position, visionRef.player.position);
        
        //anim.SetFloat(MoveSpeed, banditThugVision.ActiveMovementBehavior != AIHelpers.MovementBehaviors.Idle ? 1f : 0f);
        

        // if (visionRef.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic && visionRef.activeEnemyState == CurrentState.BackToOrigin)
        // {
        //     // if (visionRef.ConeOfVision())
        //     // {
        //     //     if (visionRef.rayToPlayer.collider != null)
        //     //     {
        //     //         Debug.Log("ray to player is not null");
        //
        //             // if (!visionRef.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f && distance <= visionRef.rangeOfVision)
        //             // {
        //
        //                 var lookingDirection = originPoint - transform.position;
        //                 lookingDirection.y = 0f;
        //
        //                 var rotation = Quaternion.LookRotation(lookingDirection);
        //                 transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
        //     //         }
        //     //     }
        //     // }
        // }
        // // may do reverse checks here, to minimize checks
        // else 
        if (visionRef.activeEnemyState == CurrentState.Attack)// || visionRef.activeEnemyState == CurrentState.Fleeing || visionRef.activeEnemyState == CurrentState.Patrolling)
        {
            if (visionRef.ConeOfVision() && visionRef.rayToPlayer.collider != null)
            {
                if (visionRef.rayToPlayer.collider.CompareTag("Player"))
                {
                    Debug.Log("ray to player is not null, rotating Archer");
        
                    // rotate toward X
                    var lookingDirection = targetObject.transform.position - transform.position;
                    lookingDirection.y = 0f;
        
                    var rotation = Quaternion.LookRotation(lookingDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
                }
                
            }
        }
        else if (visionRef.ActiveMovementBehavior == AIHelpers.MovementBehaviors.ArcherKinematic && visionRef.activeEnemyState == CurrentState.Attack)
        {
            Debug.Log("ARCHER Movement: Rotate toward the player");

            if (visionRef.ConeOfVision())
            {
                if (visionRef.rayToPlayer.collider != null)
                {
                    if (visionRef.rayToPlayer.collider.CompareTag("Wall"))
                    {
                        Debug.Log("ARCHER Movement: player is behind a wall, returning to idle");
                        
                        anim.SetTrigger(Idle);
                    }
                    else if (visionRef.rayToPlayer.collider.CompareTag("Player"))
                    {
                        Debug.Log("ARCHER Movement: sees player rotate toward them");
                        
                        var lookingDirection = targetObject.transform.position - transform.position;
                        lookingDirection.y = 0f;

                        var rotation = Quaternion.LookRotation(lookingDirection);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
                    }
                }
            }
            else
            {
                if (visionRef.activeEnemyState != CurrentState.Idle)
                    anim.SetTrigger(Idle);
                
            }
        }
        else if(visionRef.ActiveMovementBehavior == AIHelpers.MovementBehaviors.ArcherKinematic && visionRef.activeEnemyState == CurrentState.Idle && isRotatingAfterReturningToOrigin) // USE ORIGIN STATE
        {
            Debug.Log("rotating back to origin");
            
            // if idle and not looking in origin direction turn
            //if(Quaternion.Angle(transform.rotation, originRotation) > 0.1f)
            //{
                Debug.Log("rotating to origin");
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
                Debug.Log("rotating to origin end");
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
        // if (banditThugVision.ActiveMovementBehavior == AIHelpers.MovementBehaviors.BackToOriginPosKinematic)// && visionRef.activeEnemyState == CurrentState.Patrolling)
        // {
        //     Debug.Log("");
        //     
        //     float returnDistance = Vector3.Distance(transform.position, originPoint);
        //
        //     //if (Mathf.Abs(returnDistance) >= 1f)
        //    // {
        //         //gameObject.transform.position = movementResult.newPosition;
        //     //}
        //     
        // }
        //else 
        if (visionRef.activeEnemyState != CurrentState.Idle || visionRef.activeEnemyState != CurrentState.BackToOrigin)// && !gameObject.CompareTag("EnemyBoo"))
        {
            if(visionRef.ConeOfVision())
            {
                if (visionRef.rayToPlayer.collider != null)
                {
                    Debug.Log("ray to player is not null");

                    if (!visionRef.rayToPlayer.transform.CompareTag("Wall") && distance >= 0.5f)
                    {
                        Debug.Log("ray to player, not hitting a wall, shoot at player");

                        if (visionRef.activeEnemyState != CurrentState.Attack)
                            visionRef.activeEnemyState = CurrentState.Attack;
                    }
                    
                }
            }
        }
        
        if(visionRef.activeEnemyState == CurrentState.Idle)
        {
            
        }

    }
    
    public void RotateTowardsPlayer()
    {
        var lookingDirection = GameManager.Instance.playerReference.transform.position - transform.position;
        lookingDirection.y = 0f;

        var rotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
    }
    
}
