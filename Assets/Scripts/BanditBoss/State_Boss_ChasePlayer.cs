
using System;
using UnityEngine;

public class State_Boss_ChasePlayer : StateMachineBehaviour
{
    public BanditBoss_Vision enemyVisionScript;
    public BanditBoss_Movement banditBossMovement;
    public EnemyAttributes enemyAttributes;
    
    
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int AttackNormal = Animator.StringToHash("AttackNormal");
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int ReturnToOrigin = Animator.StringToHash("ReturnToOrigin");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditBoss_Vision>();
        banditBossMovement = animator.GetComponent<BanditBoss_Movement>();
        
        animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
        
        //enemyAttackScript = animator.GetComponent<BanditBoss_ChasePlayer>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();

        if(enemyVisionScript.bossDoorGameObject.activeSelf == false)
            enemyVisionScript.bossDoorGameObject.SetActive(true);
        
        
        animator.ResetTrigger(ChasePlayer);
        
        enemyVisionScript.ActivateChasePlayer();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BanditBoss_Health>().isDead)
            animator.SetTrigger(Dead);

        if (!enemyVisionScript.player)
            animator.SetTrigger(Idle);
        
        
        if(Math.Abs(animator.GetFloat(MoveSpeed) - banditBossMovement.animSpeed) <= -1)
            animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
        else if(Math.Abs(animator.GetFloat(MoveSpeed) - banditBossMovement.animSpeed) >= 0)
            animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
        

        if (enemyVisionScript.ConeOfVision())
        {
            if (enemyVisionScript.rayToPlayer.collider != null)
            {
                if (enemyVisionScript.rayToPlayer.transform.CompareTag("Wall"))
                {
                    // player is hidden from view
                    Debug.Log("BOSS ChasePlayer: player behind wall");


                    if (Vector3.Distance(animator.transform.position, banditBossMovement.originPoint) <= enemyAttributes.GetAttackRange())
                    {
                        Debug.Log("BOSS ChasePlayer: close to origin, switching to idle");
                        
                        animator.SetTrigger(Idle);
                    }
                    else
                    {
                        Debug.Log("BOSS ChasePlayer: moving back to origin point, switching to Return to origin behavior");
                        
                        animator.SetTrigger(ReturnToOrigin);
                    }

                    // //if enemyVisionScript.activeEnemyState = CurrentState.Searching;
                    // //animator.SetTrigger(Idle);
                    // float distanceToLastKnownPosition = Vector3.Distance(enemyVisionScript.lastKnownPlayersLocation.transform.position, animator.transform.position);
                    //
                    // if (distanceToLastKnownPosition <= 0.5f)
                    // {
                    //     Debug.Log("going to Idle");
                    //     
                    //     animator.SetTrigger(Idle);
                    // }
                    // else
                    // {
                    //     Debug.Log("going to search");
                    //     //enemyVisionScript.targetObject = enemyVisionScript.lastKnownPlayersLocation;
                    //     // animator.SetTrigger(Search);
                    //
                    //     //enemyVisionScript.targetObject = enemyMovementScript.originPoint;
                    // }

                }
                else if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
                {
                    Debug.Log("BOSS ChasePlayer: chasing player still, update last known position");

                    enemyVisionScript.UpdatePlayersLastKnownPosition();
                    // are we close enough to hit the player?

                    if (Vector3.Distance(animator.transform.position, enemyVisionScript.rayToPlayer.collider.transform.position) <= enemyAttributes.GetAttackRange())
                    {
                        Debug.Log("BOSS ChasePlayer: player in attack range, attack player");
                        
                        animator.SetTrigger(AttackNormal);
                    }

                }
            }
        }
        else
        {
            Debug.Log("BOSS ChasePlayer: can't see player going to Idle");
            
            animator.SetTrigger(Idle);
        }
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
