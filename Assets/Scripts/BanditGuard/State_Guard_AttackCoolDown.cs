
using System;
using UnityEngine;

public class State_Guard_AttackCoolDown : StateMachineBehaviour
{
    public BanditGuard_Movement banditGuardMovement;
    
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
        banditGuardMovement = animator.GetComponent<BanditGuard_Movement>();
        
        animator.SetFloat(MoveSpeed, banditGuardMovement.animSpeed);
        
        animator.ResetTrigger(AttackCoolDown);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BanditBoss_Health>().isDead)
            animator.SetTrigger(Dead);
        
        
        if(Math.Abs(animator.GetFloat(MoveSpeed) - banditGuardMovement.animSpeed) > Mathf.Epsilon)
            animator.SetFloat(MoveSpeed, banditGuardMovement.animSpeed);
        
        
        
        attackCoolDownTimer -= Time.deltaTime;

        if (attackCoolDownTimer <= 0f)// && !attackCoolDownTimerDone)
        {
            attackCoolDownTimerDone = true;
            
            //animator.SetTrigger(ArrivedAtPlayer);
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
