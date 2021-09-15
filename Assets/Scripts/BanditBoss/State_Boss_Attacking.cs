using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Boss_Attacking : StateMachineBehaviour
{
    public BanditBoss_Vision banditBossVision;
    public BanditBoss_Movement banditBossMovement;
    
    
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int AttackNormal = Animator.StringToHash("AttackNormal");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        banditBossMovement = animator.GetComponent<BanditBoss_Movement>();
        banditBossVision = animator.GetComponent<BanditBoss_Vision>();
        
        
        animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
        
        animator.ResetTrigger(AttackNormal);

        banditBossVision.ActivateSeek();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BanditBoss_Health>().isDead)
            animator.SetTrigger(Dead);
        
        
        if(Math.Abs(animator.GetFloat(MoveSpeed) - banditBossMovement.animSpeed) > Mathf.Epsilon)
            animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
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
