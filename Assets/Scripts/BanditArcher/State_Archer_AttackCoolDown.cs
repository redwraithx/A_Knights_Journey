using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Archer_AttackCoolDown : StateMachineBehaviour
{
    public BanditArcher_Vision enemyVisionScript;
    public EnemyAttributes enemyAttributes;

    public float coolDownTimer;
    
    
    
    private static readonly int AttackCoolDown = Animator.StringToHash("AttackCoolDown");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditArcher_Vision>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();
        
        animator.ResetTrigger(AttackCoolDown);

        coolDownTimer = enemyAttributes.GetAttackSpeed();
        
        enemyVisionScript.ActivateAttackCoolDown();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (animator.GetComponent<BanditArcher_Movement>().IsThisEnemyDead())
        if(animator.GetComponent<BanditArcher_Health>().isDead)
            return;

        coolDownTimer -= Time.deltaTime;

        if (coolDownTimer <= 0f)
        {
            if (enemyVisionScript.ConeOfVision())
            {
                if (enemyVisionScript.rayToPlayer.collider != null)
                {
                    if (enemyVisionScript.rayToPlayer.collider.CompareTag("Wall"))
                    {
                        Debug.Log("ARCHER AttackCoolDown: can't see player, changing to idle");
                        
                        animator.SetTrigger(Idle);
                    }
                    else if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
                    {
                        Debug.Log("ARCHER AttackCoolDown: player in view attacking player again");
                        
                        animator.SetTrigger(Attack);
                    }
                }
            }
            else
            {
                Debug.Log("ARCHER AttackCoolDown: can't see player, changing to idle");
                        
                animator.SetTrigger(Idle);
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
