
using System;
using UnityEngine;

public class State_Thug_ChasePlayer : StateMachineBehaviour
{
    public BanditThug_Vision enemyVisionScript;
    public BanditThug_Movement enemyMovementScript;
    public BanditThug_Attack enemyAttackScript;
    public BanditThug_Health enemyHealthScript;
    public EnemyAttributes enemyAttributes;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int GoToOriginPoint = Animator.StringToHash("GoToOriginPoint");
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int ReturnToOrigin = Animator.StringToHash("ReturnToOrigin");
    private static readonly int Attack = Animator.StringToHash("Attack");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        enemyVisionScript = animator.GetComponent<BanditThug_Vision>();
        enemyMovementScript = animator.GetComponent<BanditThug_Movement>();
        enemyAttackScript = animator.GetComponent<BanditThug_Attack>();
        enemyHealthScript = animator.GetComponent<BanditThug_Health>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();
        
        animator.SetFloat(MoveSpeed, enemyMovementScript.animSpeed);

        animator.ResetTrigger(ChasePlayer);
        
        enemyVisionScript.ActivateChasePlayer();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(enemyHealthScript.isDead)
            animator.SetTrigger(Dead);

        if(!enemyVisionScript.player)
            animator.SetTrigger(Idle);

        float distanceToPlayer = Vector3.Distance(animator.transform.position, enemyVisionScript.player.transform.position);


        if (distanceToPlayer > enemyAttributes.GetAttackRange())
        {
            Debug.Log("thug is moving towards the player");
            
            if (Math.Abs(animator.GetFloat(MoveSpeed) - enemyMovementScript.animSpeed) <= -1)
                animator.SetFloat(MoveSpeed, enemyMovementScript.animSpeed);
            else if (Math.Abs(animator.GetFloat(MoveSpeed) - enemyMovementScript.animSpeed) >= 0)
                animator.SetFloat(MoveSpeed, enemyMovementScript.animSpeed);
        }
        else
        {
            Debug.Log("thug is within range to attack the player");
        }

        if (enemyVisionScript.ConeOfVision())
        {
            if (enemyVisionScript.rayToPlayer.collider != null)
            {
                if (enemyVisionScript.rayToPlayer.transform.CompareTag("Wall"))
                {
                    // player is hidden from view
                    Debug.Log("THUG ChasePlayer: player behind wall");

                    //float distanceToLastKnownPosition = Vector3.Distance(enemyVisionScript.lastKnownPlayersLocation.transform.position, animator.transform.position);
                    if (Vector3.Distance(animator.transform.position, enemyMovementScript.originPoint) <= enemyAttributes.GetAttackRange())
                    {
                        Debug.Log("THUG ChasePlayer: close to origin, switching to idle");
                        
                        animator.SetTrigger(Idle);
                    }
                    else
                    {
                        Debug.Log("THUG ChasePlayer: moving back to origin point, switching to Return to origin behavior");
                        
                        animator.SetTrigger(ReturnToOrigin);
                    }

                }
                else if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
                {
                    Debug.Log("THUG ChasePlayer: chasing player still, update last known position");

                    // are we close enough to hit the player?

                    if (Vector3.Distance(animator.transform.position, enemyVisionScript.rayToPlayer.collider.transform.position) <= enemyAttributes.GetAttackRange())
                    {
                        Debug.Log("THUG ChasePlayer: player in attack range, attack player");
                        
                        animator.SetTrigger(Attack);
                    }
                    else
                    {
                        animator.SetTrigger(ReturnToOrigin);
                    }

                }
            }
        }
        else
        {
            Debug.Log("THUG ChasePlayer: can't see player going to Idle");

            animator.SetTrigger(Idle);
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
