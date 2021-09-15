using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Guard_Dead : StateMachineBehaviour
{
    public BanditGuard_Vision enemyVisionScript;

    public float decayTimer;
    public float decayTime = 10f;
    public bool isDecayTimerRunning = true;

    private static readonly int Death = Animator.StringToHash("Death");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditGuard_Vision>();

        // update kill counter
        GameManager.Instance.EnemyKilled();
        
        animator.ResetTrigger(Death);

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        float totalWaitDeathAndDecayTimer = animationLength + decayTime;
        decayTimer = totalWaitDeathAndDecayTimer;

        enemyVisionScript.ActivateDead();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = new Vector3(animator.transform.position.x, animator.GetComponent<CapsuleCollider>().bounds.size.y * 0.5f, animator.transform.position.z);
        
        if (isDecayTimerRunning)
        {
            decayTimer -= Time.deltaTime;

            if (decayTimer <= 0f)
            {
                isDecayTimerRunning = false;
                
                
                
                Destroy(enemyVisionScript.gameObject);
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