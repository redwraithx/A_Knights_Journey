using System.Collections;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;

public class State_Thug_AttackCoolDown : StateMachineBehaviour
{
    public BanditThug_Vision enemyVisionScript;
    public BanditThug_Movement enemyMovementScript;
    public BanditThug_Health enemyHealthScript;
    public EnemyAttributes enemyAttributes;

    public float attackCoolDownTime = 1.5f;
    public float attackCoolDownTimer;
    public bool attackCoolDownTimerDone = false;
    
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int AttackCoolDown = Animator.StringToHash("AttackCoolDown");
    private static readonly int ArrivedAtPlayer = Animator.StringToHash("ArrivedAtPlayer");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditThug_Vision>();
        enemyMovementScript = animator.GetComponent<BanditThug_Movement>();
        enemyHealthScript = animator.GetComponent<BanditThug_Health>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();
        
        animator.ResetTrigger(Attack);
        animator.ResetTrigger(AttackCoolDown);


        attackCoolDownTimer = attackCoolDownTime;
        
        
        enemyVisionScript.ActivateAttackCoolDown();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyHealthScript.isDead)
            return;

        float distanceToPlayer;

        if (enemyVisionScript.rayToPlayer.collider == null)
        {
            return;
        }
        
        distanceToPlayer = Vector3.Distance(animator.transform.position, enemyVisionScript.rayToPlayer.transform.position);

        attackCoolDownTimer -= Time.deltaTime;

        if (attackCoolDownTimer <= 0f && !attackCoolDownTimerDone)
        {
            attackCoolDownTimerDone = true;
            
            if(distanceToPlayer > enemyAttributes.GetAttackRange())
            {
                animator.SetTrigger(ChasePlayer);
                return;
            }

            animator.SetTrigger(ArrivedAtPlayer);

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
