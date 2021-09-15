using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Guard_Idle : StateMachineBehaviour
{
    public BanditGuard_Vision enemyVisionScript;
    public BanditGuard_Movement banditGuardMovement;
    public BanditGuard_Health banditGuardHealth;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    
    
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditGuard_Vision>();
        banditGuardMovement = animator.GetComponent<BanditGuard_Movement>();
        banditGuardHealth = animator.GetComponent<BanditGuard_Health>();
        
        animator.SetFloat(MoveSpeed, banditGuardMovement.animSpeed);
        
        // reset triggers to Idle
        animator.ResetTrigger(Idle);
        animator.ResetTrigger(ChasePlayer);
        animator.ResetTrigger(Dead);
        
        // update the last known players location to this enemies location for next shot
        //enemyVisionScript.UpdatePlayersLastKnownPosition(animator.transform.position);
        
        enemyVisionScript.ActivateIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (banditGuardHealth.isDead)
            return;

        if (Mathf.Abs(animator.GetFloat(MoveSpeed) - banditGuardMovement.animSpeed) > Mathf.Epsilon)
            animator.SetFloat(MoveSpeed, banditGuardMovement.animSpeed);

        if (enemyVisionScript.ConeOfVision() && enemyVisionScript.rayToPlayer.collider != null)
        {
            if (enemyVisionScript.rayToPlayer.transform.CompareTag("Wall"))
            {
                Debug.Log("Guard Idle: Player is behind a wall, should we search for the player?");
            }
            else if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
            {
                Debug.Log(animator.gameObject.name + " see's the player");
                
                animator.SetTrigger(ChasePlayer);
            }
            
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
