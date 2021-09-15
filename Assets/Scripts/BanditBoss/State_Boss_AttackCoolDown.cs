using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Boss_AttackCoolDown : StateMachineBehaviour
{
    public BanditBoss_Movement banditBossMovement;
    
    // normal attackCoolDown
    public float attackCoolDownTime = 1.5f;
    public float attackCoolDownTimer;
    public bool attackCoolDownTimerDone = false;

    
    
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int AttackNormal = Animator.StringToHash("AttackNormal");
    private static readonly int AttackCoolDown = Animator.StringToHash("AttackCoolDown");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        banditBossMovement = animator.GetComponent<BanditBoss_Movement>();
        
        animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
        
        animator.ResetTrigger(AttackCoolDown);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BanditBoss_Health>().isDead)
            animator.SetTrigger(Dead);
        
        
        if(Math.Abs(animator.GetFloat(MoveSpeed) - banditBossMovement.animSpeed) > Mathf.Epsilon)
            animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
        
        
        
        attackCoolDownTimer -= Time.deltaTime;

        if (attackCoolDownTimer <= 0f)
        {
            attackCoolDownTimerDone = true;
            
            animator.SetTrigger(ChasePlayer);
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
