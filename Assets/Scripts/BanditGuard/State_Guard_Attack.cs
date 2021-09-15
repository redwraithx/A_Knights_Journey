using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Guard_Attack : StateMachineBehaviour
{
    public BanditGuard_Vision banditGuardVision;
    public BanditGuard_Movement banditGuardMovement;
    public BanditGuard_Health enemyHealthScript;
    
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int AttackNormal = Animator.StringToHash("AttackNormal");
    
    
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        banditGuardMovement = animator.GetComponent<BanditGuard_Movement>();
        banditGuardVision = animator.GetComponent<BanditGuard_Vision>();
        enemyHealthScript = animator.GetComponent<BanditGuard_Health>();
        
        animator.SetFloat(MoveSpeed, banditGuardMovement.animSpeed);
        
        animator.ResetTrigger(AttackNormal);

        banditGuardVision.ActivateSeek();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyHealthScript.isDead)
            animator.SetTrigger(Dead);
        
        
        if(Math.Abs(animator.GetFloat(MoveSpeed) - banditGuardMovement.animSpeed) > Mathf.Epsilon)
            animator.SetFloat(MoveSpeed, banditGuardMovement.animSpeed);
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
