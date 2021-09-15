using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Boss_Enraged : StateMachineBehaviour
{
    public EnemyAttributes enemyAttributes;
    public BanditBoss_Vision banditBossVision;


    public float enragedTime = 5f;
    public float enragedTimer;
    public bool hasAppliedEnraged = false;
    public Vector3 scaleChanged;
    public float currentSize = 0f;
    public float amountToAddToSize = 0.003f;
    
    
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int Enraged = Animator.StringToHash("Enraged");
    private static readonly int Death = Animator.StringToHash("Death");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(Enraged);
        
        if (animator.transform.localScale.x > 1f)
        {
            // if its already run enraged change back to Idle or chaseplayer
            animator.SetTrigger(ChasePlayer);
        }

        banditBossVision = animator.GetComponent<BanditBoss_Vision>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();

        enragedTimer = enragedTime;

        scaleChanged = new Vector3(0.001f, 0.001f, 0.001f);

        banditBossVision.ActivateEnraged();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<BanditBoss_Health>().isDead)
        {
            Debug.Log("is boss dead while enraged? " + animator.GetComponent<BanditBoss_Health>().isDead);
            
            animator.SetTrigger(Death);
        }


        animator.transform.position = new Vector3(animator.transform.position.x, animator.GetComponent<CapsuleCollider>().bounds.size.y * 0.5f, animator.transform.position.z);

        enragedTimer -= Time.deltaTime;

        if (enragedTimer < 0f && !hasAppliedEnraged)
        {
            hasAppliedEnraged = true;
            
            // update position on Y
            animator.transform.position = new Vector3(animator.transform.position.x, 2.68f, animator.transform.position.z);
            
            // apply damage buff of 25 points
            enemyAttributes.SetWeaponDamage(25);
            
            // goto chasePLayer
            animator.SetTrigger(ChasePlayer);
        }
        else if (!hasAppliedEnraged)
        {
            //var scale = animator.transform.localScale;

            //scale += scaleChanged;
            
            animator.transform.localScale += scaleChanged;


            currentSize += amountToAddToSize;

            animator.transform.position = new Vector3(animator.transform.position.x, currentSize, animator.transform.position.z);
        }
        else
            animator.SetTrigger(ChasePlayer);


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
