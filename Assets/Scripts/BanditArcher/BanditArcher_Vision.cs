using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;


public class BanditArcher_Vision : MonoBehaviour
{
    #region EnemyAI_Vision_Variables

    //[Header("Player Reference")]
    internal Transform player = null;
    
    //[Header("Vectors References")]
    internal Vector3 enemy = Vector3.zero;
    internal Vector3 v1 = Vector3.zero;
    internal Vector3 v2 = Vector3.zero;
    
    [Header("Vision Field Of View")]
    public float FOVInDegrees = 0f;
    public float FOVInRadians = 0f;
    public float minCosine = 0f;
    public float dotProduct = 0f;
    public float rangeOfVision = 40f;
    public Transform localForward = null;
    public Transform localShotForwardDirection = null;

    //public BanditThug_Movement enemyMovement = null;

    #endregion // End EnemyAI_Vision_Variables

    
    #region EnemyAI_Vision_Pathing_Variables

    [Header("Pathing Nodes")] 
    private Animator anim = null;
    //public Vector3 lastKnownPlayersLocation = Vector3.zero;
    //public GameObject lastKnownPlayersLocation = null;
    
    
    [Header("Enemy Movement and Vision States")]
    public AIHelpers.MovementBehaviors ActiveMovementBehavior = AIHelpers.MovementBehaviors.Idle;
    public CurrentState activeEnemyState = CurrentState.Idle;
    
    [Header("Current Target Enemy is looking at")]
    public GameObject targetObject = null;
    
    internal RaycastHit rayToPlayer;
    private static readonly int AttackCoolDown = Animator.StringToHash("AttackCoolDown");

    #endregion // End EnemyAI_Vision_Pathing_Variables


    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.92f, transform.position.z);
        
        if (!localForward)
            throw new Exception("Error local forward transform is missing");

        if (!anim)
            anim = GetComponent<Animator>();

        InitializeEnemyAIVision();

        if(player)
            targetObject = player.gameObject;

        //enemyMovement = GetComponent<BanditArcher_Movement>();
        
        //lastKnownPlayersLocation = Instantiate(Resources.Load("PlayersLastKnownPosition_TrackingPoint"), transform.position, Quaternion.identity) as GameObject;
        ActivateIdle();
    }

    private void Update()
    {
        //Debug.Log("last known location of player: " + lastKnownPlayersLocation.transform.position);

        ConeOfVision();
    }



    private bool CanSeePlayer()
    {
        var direction = (player.position - transform.position);
        direction.y = 0.5f;

        float distance = Vector3.Distance(transform.position, player.position);
        
        Debug.DrawLine(transform.position, player.position, Color.white);

        return Physics.SphereCast(transform.position, 0.4f, direction, out rayToPlayer, distance);
    }
    

    private void InitializeEnemyAIVision()
    {
        FOVInDegrees = 90f;
        FOVInRadians = FOVInDegrees * Mathf.Deg2Rad;
        minCosine = Mathf.Cos(FOVInRadians / 2);

        player = GameObject.FindWithTag("Player").transform;
    }

    // private void InitializeEnemyPathingVariables()
    // {
    //     // thug has no pathing
    //     
    // }

    // thug has no pathing
   // private void SetFirstTarget() => targetObject = pathingNodes[currentPathingNodeIndex].gameObject;


    public bool ConeOfVision()
    {
        Vector3 direction = localForward.position - transform.position;
        float distanceToObject = 100f;

        if (targetObject)
            distanceToObject = Vector3.Distance(transform.position, targetObject.transform.position);

        //Debug.Log("distance to player: " + distanceToObject);
        
        // visualize the cone of vision
        Debug.DrawRay(transform.position, transform.rotation * Quaternion.Euler(0, 45, 0) * Vector3.forward * 5, Color.yellow);
        Debug.DrawRay(transform.position,  direction * 5, Color.red);
        Debug.DrawRay(transform.position, transform.rotation * Quaternion.Euler(0, -45, 0) * Vector3.forward * 5, Color.yellow);


        enemy = transform.position;
        v1 = transform.forward;
        v2 = player.position - enemy;
        dotProduct = Vector3.Dot(v1.normalized, v2.normalized);
        if (distanceToObject <= rangeOfVision && ActiveMovementBehavior !=  AIHelpers.MovementBehaviors.BackToOriginPosKinematic)// && rayToPlayer.collider != null)
        {
            if (dotProduct > minCosine && dotProduct <= 1)// && !rayToPlayer.collider.CompareTag("Wall"))
            {
                // Vector3 newDirection = player.position - transform.position;
                // newDirection.y = 0f;

                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                Vector3 currentFacingXZ = transform.forward;
                currentFacingXZ.y = 0.0f;

                // visualized line from enemy to player
                Debug.DrawLine(GetComponent<Rigidbody>().position, /* GetComponent<Rigidbody>().position */ transform.position + currentFacingXZ * distanceToPlayer, Color.magenta, 0.0f, false);

                if (CanSeePlayer())
                    if(!rayToPlayer.collider.CompareTag("Wall"))
                        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic || activeEnemyState != CurrentState.Attack)
                            ActivateSeekAttack();

                return true;
            }
        }
        // else
        // {
        //     if (activeEnemyState == CurrentState.Attack)
        //     {
        //         activeEnemyState = CurrentState.Searching;
        //     }
        // }

        return false;
    }

    public void AttackIsOnCoolDown()
    {
        anim.SetTrigger(AttackCoolDown);
    }
    

    public AIHelpers.MovementBehaviors GetMovementState()
    {
        return ActiveMovementBehavior;
    }
 
    
    public void ActivateIdle()
    {
        Debug.Log("changed to IDLE");

        //if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.ArcherKinematic;
        
        
        activeEnemyState = CurrentState.Idle;
    }

    public void ActivateSeekAttack()
    {
        Debug.Log("changed to seek");
        
        if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.ArcherKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.ArcherKinematic;

        activeEnemyState = CurrentState.Attack;
    } 
    
    public void ActivateAttackCoolDown()
    {
        Debug.Log("changed to seek");
        
        if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.ArcherKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.ArcherKinematic;

        activeEnemyState = CurrentState.AttackCoolDown;
    } 
    
    public void ActivateDead()
    {
        Debug.Log("Changed to dead state");

        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.Dead)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.Dead;

        activeEnemyState = CurrentState.Dead;
    }

    public void ActivateArcherIdle()
    {
        Debug.Log("Archer idle");

        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.ArcherKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.ArcherKinematic;

        activeEnemyState = CurrentState.Idle;
    }
}
