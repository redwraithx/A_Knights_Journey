

using System;
using UnityEngine;
using EnemyStates;


public class BanditThug_Vision : MonoBehaviour
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
    public float rangeOfVision = 20f;
    public Transform localForward = null;

    public float attackDistance = 1.5f;

    public BanditThug_Movement enemyMovement = null;
    public EnemyAttributes enemyHealth = null;

    public RaycastHit rayToLastKnownPOS;
    
    #endregion // End EnemyAI_Vision_Variables

    
    #region EnemyAI_Vision_Pathing_Variables

    [Header("Pathing Nodes")] 
    private Animator anim = null;

    [Header("Enemy Movement and Vision States")]
    public AIHelpers.MovementBehaviors ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
    public CurrentState activeEnemyState = CurrentState.Idle;
    
    [Header("Current Target Enemy is looking at")]
    public GameObject targetObject = null;

    
    internal RaycastHit rayToPlayer;


    #endregion // End EnemyAI_Vision_Pathing_Variables


    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.92f, transform.position.z);
        
        if (!localForward)
            throw new Exception("Error local forward transform is missing");

        if (!anim)
            anim = GetComponent<Animator>();

        if (!player)
            player = GameManager.Instance.playerReference.transform;

        if (!enemyHealth)
            enemyHealth = GetComponent<EnemyAttributes>();
        
        enemyMovement = GetComponent<BanditThug_Movement>();

        InitializeEnemyAIVision();

        if(player)
            targetObject = player.gameObject;

    }

    private void Update()
    {
        //Debug.Log("last known location of player: " + lastKnownPlayersLocation.transform.position);

       
    }



    private bool CanSeePlayer()
    {
        if (enemyHealth.GetIsDeadStatus())
        {
            rayToPlayer = new RaycastHit();
            player = null;
            
            return false;
        }
        
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

    

   public bool CheckIfDead() => activeEnemyState == CurrentState.Dead || ActiveMovementBehavior == AIHelpers.MovementBehaviors.Dead;

    public bool ConeOfVision()
    {

        if (GetComponent<BanditThug_Health>().isDead)
        {
            rayToPlayer = new RaycastHit();
            
            return false;
        }
        
        Vector3 direction = localForward.position - transform.position;
        
        float distanceToObject = 100f;

        if (targetObject)
            distanceToObject = Vector3.Distance(transform.position, targetObject.transform.position);

        
        // visualize the cone of vision
        Debug.DrawRay(transform.position, transform.rotation * Quaternion.Euler(0, 45, 0) * Vector3.forward * 5, Color.yellow);
        Debug.DrawRay(transform.position,  direction * 5, Color.red);
        Debug.DrawRay(transform.position, transform.rotation * Quaternion.Euler(0, -45, 0) * Vector3.forward * 5, Color.yellow);


        enemy = transform.position;
        v1 = transform.forward;
        v2 = player.position - enemy;
        dotProduct = Vector3.Dot(v1.normalized, v2.normalized);
        
        if (distanceToObject <= rangeOfVision && ActiveMovementBehavior != AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
        {
            
            
            if (dotProduct > minCosine && dotProduct <= 1)
            {
                Debug.Log("target is within vision range");
                
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                Vector3 currentFacingXZ = transform.forward;
                currentFacingXZ.y = 0.0f;

                // visualized line from enemy to player
                Debug.DrawLine(GetComponent<Rigidbody>().position, transform.position + currentFacingXZ * distanceToPlayer, Color.magenta, 0.0f, false);

                if (CanSeePlayer())
                    if(rayToPlayer.collider.CompareTag("Player"))
                        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
                            ActivateChasePlayer();

                return true;
            }
        }


        return false;
    }

    public AIHelpers.MovementBehaviors GetMovementState()
    {
        return ActiveMovementBehavior;
    }
 
    
    public void ActivateIdle()
    {
        Debug.Log("changed to IDLE");

        ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
        
        
        activeEnemyState = CurrentState.Idle;
    }


    public void ActivateSeek()
    {
        Debug.Log("changed movementBehavior to seek");
        
        if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
        
        //activeEnemyState = CurrentState.Attack;
    }

    
    public void ActivateChasePlayer()
    {

        Debug.Log("changed movementBehavior to chasing player");

        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;

        activeEnemyState = CurrentState.ChasePlayer;
    }


    public void ActivateArrivedAtPlayer()
    {
        
        
        Debug.Log("changed movementBehavior to Arrived At Player");
        
        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
    
        activeEnemyState = CurrentState.ArrivedAtPlayer;
    }


    public void ActivateAttackCoolDown()
    {
        
        
        Debug.Log("changed movementBehavior to Attack cool down");
        
        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
    
        activeEnemyState = CurrentState.AttackCoolDown;
    }
    
    
    

    public void ActivateSearchPosition()
    {
        
        
        Debug.Log("changed movementBehavior to searching last known position");
        
        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
    
        activeEnemyState = CurrentState.SearchPlayersLastKnownPOS;
    }
    
    
    
    public void ActivateSearch()
    {
        Debug.Log("changed to searching");
        
        if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.SearchKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SearchKinematic;
    
        activeEnemyState = CurrentState.Searching;
    }
    
    public void ActivateReturnToOriginPoint()
    {
        
        
        Debug.Log("changed to return to origin point");
        
        if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.BackToOriginPosKinematic;
    
        activeEnemyState = CurrentState.BackToOrigin;
    }

    public void ActivateReturnToOriginPointDelay()
    {
        
        
        Debug.Log("changed to idle delay before returning to origin point");
        
        if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.BackToOriginPosKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.BackToOriginPosKinematic;
    
        activeEnemyState = CurrentState.ResetToOriginDelay;
    }

    public void ActivateDead()
    {
        Debug.Log("Changed to dead state");

        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.Dead)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.Dead;

        activeEnemyState = CurrentState.Dead;
    }
    
}
