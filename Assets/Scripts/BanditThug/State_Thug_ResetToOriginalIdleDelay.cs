using System.Collections;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;

public class State_Thug_ResetToOriginalIdleDelay : StateMachineBehaviour
{
    public BanditThug_Vision enemyVisionScript;
    public EnemyAttributes enemyAttributes;

    public float returnToOriginDelayTime = 5f;
    public float returnToOriginDelayTimer;
    
    private static readonly int ResetToOriginIdleDelay = Animator.StringToHash("ResetToOriginIdleDelay");
    private static readonly int GoToOriginPoint = Animator.StringToHash("GoToOriginPoint");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(ResetToOriginIdleDelay);
        
        enemyVisionScript = animator.GetComponent<BanditThug_Vision>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();

        returnToOriginDelayTimer = returnToOriginDelayTime;
        
       // enemyVisionScript.ActivateReturnToOriginPointDelay();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyVisionScript.activeEnemyState == CurrentState.Dead)
            return;
        
         // check if we see the player, if so chase the player
         if (enemyVisionScript.ConeOfVision())
         {
             animator.SetTrigger(ChasePlayer);
         }
         
         // check if the delay time before returning back to origin is up
         returnToOriginDelayTimer -= Time.deltaTime;
         
         if (returnToOriginDelayTimer <= 0f)
         {
             animator.SetTrigger(GoToOriginPoint);
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
