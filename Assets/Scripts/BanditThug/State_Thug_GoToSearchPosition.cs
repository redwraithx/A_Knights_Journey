using System.Collections;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;

public class State_Thug_GoToSearchPosition : StateMachineBehaviour
{
    public BanditThug_Vision enemyVisionScript;
    public EnemyAttributes enemyAttributes;
    
    // wall avoidance variables
    [SerializeField] private float toleranceRadius = 3f;
    // [SerializeField] private float minimumAvoidanceDistance = 20f;
    // [SerializeField] private float force = 50.0f;
    // [SerializeField] private float rotationSpeed = 5.0f;
    // [SerializeField] private float currentSpeed;
    // [SerializeField] private float movementSpeed = 20f;
    // [SerializeField] private float charactersNormalY = 0.0f;
    // public LayerMask layerMask; // Obstacles layer
    // private Vector3 direction;
    // private Vector3 targetPoint;
    // private Vector3 hitNormal;
    // private Quaternion targetRotation;


    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int Search = Animator.StringToHash("Search");
    private static readonly int ResetToOriginIdleDelay = Animator.StringToHash("ResetToOriginIdleDelay");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Dead = Animator.StringToHash("Dead");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(Search);
        
        enemyVisionScript = animator.GetComponent<BanditThug_Vision>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();

        //layerMask = 1 << 8;
        
        enemyVisionScript.ActivateSearchPosition();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
       //  if(animator.GetComponent<BanditThug_Health>().isDead)
       //      animator.SetTrigger(Dead);
       //
       //  float distanceToLastPlayerPOS = Vector3.Distance(animator.transform.position, enemyVisionScript.lastKnownPlayersLocation.transform.position);
       //  
       //  
       //  if(!enemyVisionScript.player || distanceToLastPlayerPOS <= toleranceRadius)
       //      animator.SetTrigger(ResetToOriginIdleDelay);
       //
       //  if(enemyVisionScript.ConeOfVision())
       //      if(enemyVisionScript.rayToPlayer.collider != null)
       //          if(enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
       //              animator.SetTrigger(ChasePlayer);
       //  
       //  if (enemyVisionScript.rayToLastKnownPOS.collider != null)
       //  {
       //
       //      if (enemyVisionScript.rayToLastKnownPOS.collider.CompareTag("Wall"))
       //      {
       //          Debug.Log("ray to last known POS is fall, see wall");
       //          
       //          animator.SetTrigger(ResetToOriginIdleDelay);
       //      }
       //      else if (enemyVisionScript.rayToLastKnownPOS.collider.CompareTag("PlayersLastKnownPOS"))
       //      {
       //          Debug.Log("Ray to last know POS is true, see last POS");
       //
       //          //enemyVisionScript.targetObject = enemyVisionScript.lastKnownPlayersLocation;
       //      }
       //
       //
       //      if (distanceToLastPlayerPOS <= toleranceRadius)
       //      {
       //          Debug.Log("ray to something? " + enemyVisionScript.rayToLastKnownPOS.collider.gameObject.name);
       //
       //          animator.SetTrigger(ResetToOriginIdleDelay);
       //      }
       //
       //
       //  }
       //
       //
       //
       //
       //  // float distanceToPlayer = Vector3.Distance(animator.transform.position, enemyVisionScript.player.transform.position);
       //  //
       //  //
       //  // if (enemyVisionScript.ConeOfVision())
       //  // {
       //  //     if (enemyVisionScript.rayToLastKnownPOS.collider != null)
       //  //     {
       //  //         if (enemyVisionScript.rayToLastKnownPOS.transform.CompareTag("Wall"))
       //  //         {
       //  //             // player is hidden from veiw
       //  //             Debug.Log("player behind wall");
       //  //
       //  //             
       //  //             //if enemyVisionScript.activeEnemyState = CurrentState.Searching;
       //  //             //animator.SetTrigger(Idle);
       //  //             float distanceToLastKnownPosition = Vector3.Distance(enemyVisionScript.lastKnownPlayersLocation.transform.position, animator.transform.position);
       //  //
       //  //             if (distanceToLastKnownPosition <= 0.5f)
       //  //             {
       //  //                 Debug.Log("going to Idle");
       //  //                 
       //  //                 animator.SetTrigger(Idle);
       //  //             }
       //  //             else
       //  //             {
       //  //                 Debug.Log("going to search");
       //  //                 enemyVisionScript.targetObject = enemyVisionScript.lastKnownPlayersLocation;
       //  //                 animator.SetTrigger(Search);
       //  //             }
       //  //
       //  //         }
       //  //         else if (enemyVisionScript.rayToLastKnownPOS.transform.CompareTag("Player"))
       //  //         {
       //  //             enemyVisionScript.UpdatePlayersLastKnownPosition(enemyVisionScript.rayToPlayer.collider.transform.position);
       //  //
       //  //             Debug.Log("can see player");
       //  //             
       //  //             animator.SetTrigger(ChasePlayer);
       //
       //
       //              // if (distanceToPlayer <= enemyAttributes.GetAttackRange() && !enemyAttackScript.hasHitPlayer) //enemyVisionScript.attackDistance)
       //              // {
       //              //     Debug.Log("Attacked the player, they are in range");
       //              //     animator.SetTrigger(Attack);
       //              //     
       //              //     // ADD KNOCK BACK TO PLAYER WHEN HIT HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
       //              //     
       //              // }
       //              // else
       //              //     Debug.Log("player is not in range move toward him");
       //              //
       //              // // if (distanceToPlayer <= enemyVisionScript.player.GetComponent<KinematicInput>().distanceToKnockback)
       //              // // {
       //              // //     Debug.Log("player gets hit / knocked back");
       //              // //     
       //              // //     // hit player
       //              // //     //enemyVisionScript.player.GetComponent<PlayersHealth>().TakeDamageFromAttack(50);
       //              // //     
       //              // //     // push player back
       //              //
       //              // //     
       //              // //     // set state to flee with cool down timer
       //              // //     Debug.Log("move to cooldown");
       //              //
       //              // // }
       //              
       //  //             
       //  //             
       //  //         }
       //  //         else
       //  //         {
       //  //             Debug.Log("move to last POS now!");
       //  //         }
       //  //         
       //  //     }
       //  //     
       //  // }
       //  // else
       //  // {
       //  //     float distanceToLastKnownPosition = Vector3.Distance(enemyVisionScript.lastKnownPlayersLocation.transform.position, animator.transform.position);
       //  //     
       //  //     if (distanceToLastKnownPosition <= 0.5f)
       //  //     {
       //  //         animator.SetTrigger(Idle);
       //  //     }
       //  //     else
       //  //     {
       //  //         enemyVisionScript.targetObject = enemyVisionScript.lastKnownPlayersLocation;
       //  //         
       //  //         animator.SetTrigger(Search);
       //  //     }
       //  // }
       //  
       //  
       //  
       //  // if (animator.GetComponent<BanditThug_Movement>().IsThisEnemyDead())
       //  //     return;
       //  //
       //  //
       //  // if (enemyVisionScript.ConeOfVision())
       //  // {
       //  //     if (enemyVisionScript.rayToPlayer.collider != null)
       //  //     {
       //  //         if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
       //  //         {
       //  //             animator.SetTrigger(ChasePlayer);
       //  //         }
       //  //         else if (enemyVisionScript.rayToPlayer.collider.CompareTag("Wall"))
       //  //         {
       //  //             
       //  //         }
       //  //         
       //  //     }
       //  //     else if(enemyVisionScript.rayToLastKnownPOS.collider != null && enemyVisionScript.rayToLastKnownPOS.transform.position != animator.transform.position)
       //  //     {
       //  //         // move to last known pos
       //  //         Debug.Log("move to last known pos of player");
       //  //         
       //  //         
       //  //         
       //  //     }
       //      
       //  
       //  
       //  
       //  // Debug.Log($"searching for player at his last position, currentPosition: {animator.transform.position}");
       //  //
       //  // //currentSpeed = 0.0f;
       //  //
       //  //  float distanceToMarker = Vector3.Distance(animator.transform.position, enemyVisionScript.lastKnownPlayersLocation.transform.position);
       //  //
       //  //  Debug.DrawLine(animator.transform.position, enemyVisionScript.lastKnownPlayersLocation.transform.position, Color.cyan);
       //  //
       //  //  if (Physics.Raycast(animator.transform.position, enemyVisionScript.lastKnownPlayersLocation.transform.position, out enemyVisionScript.rayToLastKnownPOS, distanceToMarker))
       //  //  {
       //  //      if (enemyVisionScript.rayToLastKnownPOS.collider != null)
       //  //      {
       //  //          if (enemyVisionScript.ConeOfVision() && enemyVisionScript.rayToPlayer.collider != null)
       //  //          {
       //  //              if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player")) //&& distanceToMarker <= enemyVisionScript.attackDistance)
       //  //              {
       //  //                  // delay before going back to originating guard position
       //  //                  animator.SetTrigger(ChasePlayer);
       //  //              }
       //  //          }
       //  //          else if (enemyVisionScript.rayToLastKnownPOS.collider.CompareTag("Wall"))
       //  //          {
       //  //              Debug.Log("can't walk through walls or other obstacles  need to deal with going around things");
       //  //               
       //  //          //     targetPoint = enemyVisionScript.rayToLastKnownPOS.point;
       //  //          //     // set last known position relative to the wall that is in the wall using the walls normal from the point of the ray.
       //  //          //     hitNormal = enemyVisionScript.rayToLastKnownPOS.normal;
       //  //          //     hitNormal.y = charactersNormalY;
       //  //          //
       //  //          //     direction = (targetPoint - animator.transform.position);
       //  //          //     direction.Normalize();
       //  //          //     
       //  //          //     // avoid the wall
       //  //          //     direction = animator.transform.forward + hitNormal * force;
       //  //          //
       //  //          //     direction.x = 0f;
       //  //          //     direction.z = 0f;
       //  //          //
       //  //          //     if (Vector3.Distance(targetPoint, animator.transform.position) < toleranceRadius)
       //  //          //         return;
       //  //          //
       //  //          //     currentSpeed = movementSpeed * Time.deltaTime;
       //  //          //
       //  //          //     targetRotation = Quaternion.LookRotation(direction);
       //  //          //     animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
       //  //          //
       //  //          //     Debug.Log("direction: " + direction);
       //  //          //     Debug.Log("current pos: " + animator.transform.position + ", new Pos: " + (animator.transform.forward * currentSpeed));
       //  //          //     
       //  //          //     // newMovePosition
       //  //          //     animator.transform.position = animator.transform.forward * currentSpeed;
       //  //          }
       //  //          else
       //  //          {
       //  //              Debug.Log("still moving to the last players known location");
       //  //              
       //  //              // check if you reached the markers location
       //  //              if (distanceToMarker <= toleranceRadius)
       //  //              {
       //  //                  animator.SetTrigger(ResetToOriginIdleDelay);
       //  //              }
       //  //          }
       //  //      }
       //  //      // else
       //  //      // {
       //  //      //     
       //  //      //     
       //  //      // }
       // // }
        
        
        
    }
    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
