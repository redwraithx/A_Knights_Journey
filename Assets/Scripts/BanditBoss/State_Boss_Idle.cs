using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Boss_Idle : StateMachineBehaviour
{
    public BanditBoss_Vision enemyVisionScript;
    public BanditBoss_Movement banditBossMovement;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditBoss_Vision>();
        banditBossMovement = animator.GetComponent<BanditBoss_Movement>();
        
        animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);
        
        
        // reset triggers to Idle
        animator.ResetTrigger(Idle);
        animator.ResetTrigger(ChasePlayer);
        animator.ResetTrigger(Dead);
        
        
        
        
        // update the last known players location to this enemies location for next shot
        enemyVisionScript.UpdatePlayersLastKnownPosition(animator.transform.position);
        
        enemyVisionScript.ActivateIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BanditBoss_Movement>().IsThisEnemyDead())
            return;
        
        
        if(Math.Abs(animator.GetFloat(MoveSpeed) - banditBossMovement.animSpeed) > Mathf.Epsilon)
            animator.SetFloat(MoveSpeed, banditBossMovement.animSpeed);


        if (enemyVisionScript.ConeOfVision() && enemyVisionScript.rayToPlayer.collider != null)
        {
            if (enemyVisionScript.rayToPlayer.transform.CompareTag("Wall"))
            {
                Debug.Log("BOSS Idle: player is behind a wall, should we search for the player?");
                
            }
            
            else if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
            {
                Debug.Log($"{animator.gameObject.name} see's the player");

                //enemyVisionScript.activeEnemyState = CurrentState.ChasePlayer;
                animator.SetTrigger(ChasePlayer);
            
            }
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
