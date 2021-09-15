using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Archer_Idle : StateMachineBehaviour
{
    public BanditArcher_Vision banditArcherVision;
    public EnemyAttributes enemyAttributes;
    public BanditArcher_Health banditArcherHealth;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        banditArcherVision = animator.GetComponent<BanditArcher_Vision>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();
        banditArcherHealth = animator.GetComponent<BanditArcher_Health>();
        
        animator.ResetTrigger(Idle);
        
        
        
        banditArcherVision.ActivateArcherIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (banditArcherHealth.isDead)
            return;

        if (banditArcherVision.ConeOfVision())
        {
            if (banditArcherVision.rayToPlayer.collider != null)
            {
                if (banditArcherVision.rayToPlayer.collider.CompareTag("Wall"))
                {
                    Debug.Log("ARCHER Idle: ray to player is blocked by a wall or obstruction");
                    
                }
                else if (banditArcherVision.rayToPlayer.collider.CompareTag("Player"))
                {
                    Debug.Log("ARCHER Idle: see player");

                    if (Vector3.Distance(animator.transform.position, banditArcherVision.rayToPlayer.collider.transform.position) <= enemyAttributes.GetAttackRange())
                    {
                        Debug.Log("ARCHER Idle: Player is in range to be attacked, shooting");
                        
                        animator.SetTrigger(Attack);
                    }
                }
                
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
