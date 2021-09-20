using System;
using UnityEngine;
using EnemyStates;


public class BanditBoss_Vision : MonoBehaviour
{
#region EnemyAI_Vision_Variables

    public GameObject bossDoorGameObject;

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

    public float attackDistance = 1f;

    public BanditBoss_Movement enemyMovement = null;

    // marker for players last known location
    //public GameObject lastKnownPlayersLocation = null;
    //public float lastKnownPlayersLocationOffsetY = 1f;
    public RaycastHit rayToLastKnownPOS;
    
    #endregion // End EnemyAI_Vision_Variables

    
    #region EnemyAI_Vision_Pathing_Variables

    [Header("Pathing Nodes")] 
    private Animator anim = null;
    //public List<Transform> pathingNodes = new List<Transform>();
    //public Vector3 lastKnownPlayersLocation = Vector3.zero;
    //public GameObject lastKnownPlayersLocation = null;
    
    
    //public bool isUpdatingIndex = false;
   // public int currentPathingNodeIndex = 0;
    //public bool isPathingInIncreasingOrder = true;

    [Header("Enemy Movement and Vision States")]
    public AIHelpers.MovementBehaviors ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
    public CurrentState activeEnemyState = CurrentState.Idle;
    
    [Header("Current Target Enemy is looking at")]
    public GameObject targetObject = null;

    
    internal RaycastHit rayToPlayer;

    [SerializeField] private EnemyAttributes bossEnemyAttributes = null;
    //private static readonly int Patrolling = Animator.StringToHash("Patrolling");


    #endregion // End EnemyAI_Vision_Pathing_Variables


    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        
        if (!localForward)
            throw new Exception("Error local forward transform is missing");

        if (!anim)
            anim = GetComponent<Animator>();

        if (!bossEnemyAttributes)
            bossEnemyAttributes = GetComponent<EnemyAttributes>();

        if (!player)
            player = GameManager.Instance.playerReference.transform;

        
        InitializeEnemyAIVision();
        //InitializeEnemyPathingVariables();

        if(player)
            targetObject = player.gameObject;

        enemyMovement = GetComponent<BanditBoss_Movement>();
        
        //lastKnownPlayersLocation = Instantiate(Resources.Load("PlayersLastKnownPosition_TrackingPoint"), transform.position, Quaternion.identity) as GameObject;
        //lastKnownPlayersLocation.transform.position = transform.position;
    }

    private void Update()
    {
        //Debug.Log("last known location of player: " + lastKnownPlayersLocation.transform.position);

       
    }
    
    public void UpdatePlayersLastKnownPosition(Vector3 lastPos)
    {
        //lastKnownPlayersLocation.transform.position = new Vector3(lastPos.x, lastKnownPlayersLocationOffsetY, lastPos.z);
        
        Debug.Log("players last known location successfully updated");
    }
    
    public void UpdatePlayersLastKnownPosition()
    {
        //lastKnownPlayersLocation.transform.position = new Vector3(rayToPlayer.transform.position.x, lastKnownPlayersLocationOffsetY, rayToPlayer.transform.position.z);
        
        Debug.Log("players last known location successfully updated");
    }



    private bool CanSeePlayer()
    {
        if (activeEnemyState == CurrentState.Dead)
        {
            rayToPlayer = new RaycastHit();
            player = null;
            
            return false;
        }
        
        var direction = (player.position - transform.position);
        //direction.y = 0.5f;
        direction.y = 0f;

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
        if (activeEnemyState == CurrentState.Dead)
            return false;

        if (!bossEnemyAttributes.GetMovementStatus())
        {
            Debug.Log("boss can not move currently");

            return false;
        }
            

        if(player)
            if (Vector3.Distance(transform.position, player.transform.position) <= 60f)
            {
                CanSeePlayer();

                return true;
            }




        return false;
        
        // ///////////////
        
        
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
                    if(rayToPlayer.collider.CompareTag("Player"))
                        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
                            ActivateChasePlayer();
                
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

    public AIHelpers.MovementBehaviors GetMovementState()
    {
        return ActiveMovementBehavior;
    }
 
    
    public void ActivateIdle()
    {
        Debug.Log("changed to IDLE");

        //if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
        
        
        activeEnemyState = CurrentState.Idle;
    }

    // public void ActivateWander()
    // {
    //     Debug.Log("changed to wanderer");
    //     
    //     ActiveMovementBehavior = AIHelpers.MovementBehaviors.WanderKinematic;
    //
    //     activeEnemyState = CurrentState.Patrolling;
    // }

    public void ActivateSeek()
    {
        Debug.Log("changed movementBehavior to seek");
        
        if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.SeekKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;

        activeEnemyState = CurrentState.Attack;
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
    
    
    
    // public void ActivateSearch()
    // {
    //     Debug.Log("changed to searching");
    //     
    //     if(ActiveMovementBehavior != AIHelpers.MovementBehaviors.SearchKinematic)
    //         ActiveMovementBehavior = AIHelpers.MovementBehaviors.SearchKinematic;
    //
    //     activeEnemyState = CurrentState.Searching;
    // }
    
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

        //activeEnemyState = CurrentState.ResetToOriginDelay;
        activeEnemyState = CurrentState.Idle;
    }

    public void ActivateEnraged()
    {
        Debug.Log("Changed to enraged state");

        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.ArcherKinematic)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.ArcherKinematic;

        activeEnemyState = CurrentState.Idle;
    }
    
    public void ActivateDead()
    {
        Debug.Log("Changed to dead state");

        if (ActiveMovementBehavior != AIHelpers.MovementBehaviors.Dead)
            ActiveMovementBehavior = AIHelpers.MovementBehaviors.Dead;

        activeEnemyState = CurrentState.Dead;
    }
    
    
    // public void ActivateFlee()
    // {
    //     Debug.Log("changed to flee");
    //     
    //     ActiveMovementBehavior = AIHelpers.MovementBehaviors.FleeKinematic;
    //
    //     activeEnemyState = CurrentState.Fleeing;
    // }

    // public void ActivateArrived()
    // {
    //     Debug.Log("enemy has arrived, changed to Arrived");
    //     
    //     ActiveMovementBehavior = AIHelpers.MovementBehaviors.WanderKinematic;
    //
    //     activeEnemyState = CurrentState.Arrived;
    // }


    // public void ActivatePatrolling()
    // {
    //     Debug.Log("changed to Patrolling");
    //     
    //     ActiveMovementBehavior = AIHelpers.MovementBehaviors.WanderKinematic;
    //
    //     activeEnemyState = CurrentState.Patrolling;
    // }
    //
    // // thug does not need this unless there is time
    // private int GetNextIndex()
    // {
    //     if (isUpdatingIndex)
    //         return currentPathingNodeIndex;
    //
    //     isUpdatingIndex = true;
    //
    //     if (isPathingInIncreasingOrder)
    //     {
    //         if (currentPathingNodeIndex + 1 > pathingNodes.Count - 1)
    //             currentPathingNodeIndex = 0;
    //         else
    //             currentPathingNodeIndex += 1;
    //     }
    //     else
    //     {
    //         if (currentPathingNodeIndex - 1 < 0)
    //             currentPathingNodeIndex = pathingNodes.Count - 1;
    //         else
    //             currentPathingNodeIndex -= 1;
    //     }
    //
    //
    //     isUpdatingIndex = false;
    //
    //     return currentPathingNodeIndex;
    // }
    //
    // // thug will not patrol unless i have time
    // public void SetPatrollingTarget()
    // {
    //     if (pathingNodes.Count == 0)
    //         return;
    //     
    //     int index = GetNextIndex();
    //
    //     targetObject = pathingNodes[index].gameObject;
    // }
}
