using System.Collections;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;

public class State_Thug_ResetGoToOrigin : StateMachineBehaviour
{
    public BanditThug_Vision enemyVisionScript;
    public BanditThug_Movement enemyMovementScript;
    public BanditThug_Health enemyHealthScript;


    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int ResetGoToOrigin = Animator.StringToHash("ResetGoToOrigin");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditThug_Vision>();
        enemyMovementScript = animator.GetComponent<BanditThug_Movement>();
        enemyHealthScript = animator.GetComponent<BanditThug_Health>();
        
        animator.ResetTrigger(ResetGoToOrigin);
        
        enemyVisionScript.ActivateReturnToOriginPoint();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyHealthScript.isDead)
            return;
        
        
        // check if we see the player, if so chase the player
        if (enemyVisionScript.ConeOfVision())
        {
            Debug.Log("returning to origin, but see's player going to chase");
            
            animator.SetTrigger(ChasePlayer);
        }
        
        
        // did we reach our origin point position
        if (Vector3.Distance(animator.transform.position, enemyMovementScript.originPoint) <= enemyMovementScript.originStoppingMinimumRange)
        {
            Debug.Log("Thug is at origin position, switching to idle");
            
            enemyMovementScript.isRotatingAfterReturningToOrigin = true;

            enemyVisionScript.ActivateIdle();

            animator.SetTrigger(Idle);
        }
        else
        {
            Debug.Log("returning to origin ELSE should be moving");
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
