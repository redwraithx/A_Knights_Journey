
using UnityEngine;
using EnemyStates;

public class State_Thug_ArrivedAtPlayer : StateMachineBehaviour
{
    public BanditThug_Vision enemyVisionScript;
    public BanditThug_Movement enemyMovementScript;
    public BanditThug_Health enemyHealthScript;
    public EnemyAttributes enemyAttributes;


    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");
    private static readonly int ArrivedAtPlayer = Animator.StringToHash("ArrivedAtPlayer");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int AttackCoolDown = Animator.StringToHash("AttackCoolDown");


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyVisionScript = animator.GetComponent<BanditThug_Vision>();
        enemyMovementScript = animator.GetComponent<BanditThug_Movement>();
        enemyHealthScript = animator.GetComponent<BanditThug_Health>();
        enemyAttributes = animator.GetComponent<EnemyAttributes>();
        
        animator.ResetTrigger(ArrivedAtPlayer);
        
        enemyVisionScript.ActivateArrivedAtPlayer();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyHealthScript.isDead)
            return;

        Transform player = null;
        if (enemyVisionScript.rayToPlayer.collider == null)
            return;
        
        player = enemyVisionScript.rayToPlayer.transform;
        
        float distanceToPlayer = Vector3.Distance(animator.transform.position, player.position);
        
        //Debug.Log("distance to player in arrived at player: " + distanceToPlayer);
        
        // check if player is still within the range of the enemies attack range
        if (distanceToPlayer <= enemyVisionScript.attackDistance)
        {
 //           enemyVisionScript.activeEnemyState = CurrentState.Attack;
 
            Debug.Log("player in range of thug");
            
            
            animator.SetTrigger(Attack);
            
            enemyVisionScript.ActivateAttackCoolDown();
        }
        
        // player is out of range, switch to chase player state
        else if(distanceToPlayer > enemyAttributes.GetAttackRange())
        {
            Debug.Log("enemy is to far away enemy is chansing player");
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
