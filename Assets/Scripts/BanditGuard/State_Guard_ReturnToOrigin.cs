using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Guard_ReturnToOrigin : StateMachineBehaviour
{
    public BanditGuard_Vision enemyVisionScript;
    public BanditGuard_Movement enemyMovementScript;

       
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int ReturnToOrigin = Animator.StringToHash("ReturnToOrigin");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditGuard_Vision>();
        enemyMovementScript = animator.GetComponent<BanditGuard_Movement>();

        animator.ResetTrigger(ReturnToOrigin);
        
        
        enemyVisionScript.ActivateReturnToOriginPoint();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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
