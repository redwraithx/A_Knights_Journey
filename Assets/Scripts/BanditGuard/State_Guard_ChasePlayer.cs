using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;


public class State_Guard_ChasePlayer : StateMachineBehaviour
{
    public BanditGuard_Vision enemyVisionScript;
    public BanditGuard_Movement enemyMovementScript;
    public BanditGuard_Attack enemyAttackScript;
    public BanditGuard_Health enemyHealthScript;
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
        enemyVisionScript = animator.GetComponent<BanditGuard_Vision>();
        enemyMovementScript = animator.GetComponent<BanditGuard_Movement>();
        enemyAttackScript = animator.GetComponent<BanditGuard_Attack>();
        enemyHealthScript = animator.GetComponent<BanditGuard_Health>();
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

        if(Math.Abs(animator.GetFloat(MoveSpeed) - enemyMovementScript.animSpeed) <= -1)
            animator.SetFloat(MoveSpeed, enemyMovementScript.animSpeed);
        else if(Math.Abs(animator.GetFloat(MoveSpeed) - enemyMovementScript.animSpeed) >= 0)
            animator.SetFloat(MoveSpeed, enemyMovementScript.animSpeed);
        

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

                    //enemyVisionScript.UpdatePlayersLastKnownPosition();
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


        //         else if (enemyVisionScript.rayToPlayer.transform.CompareTag("Player"))
        //         {
        //             enemyVisionScript.UpdatePlayersLastKnownPosition(enemyVisionScript.rayToPlayer.collider.transform.position);
        //
        //             Debug.Log("THUG ChasePlayer: can see player");
        //
        //
        //             if (distanceToPlayer <= enemyAttributes.GetAttackRange() && !enemyAttackScript.hasHitPlayer) //enemyVisionScript.attackDistance)
        //             {
        //                 Debug.Log("THUG ChasePlayer: Attacked the player, they are in range");
        //                 animator.SetTrigger(Attack);
        //                 
        //                 // ADD KNOCK BACK TO PLAYER WHEN HIT HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //                 
        //             }
        //             else
        //                 Debug.Log("THUG ChasePlayer: ,player is not in range move toward him");
        //             
        //             // if (distanceToPlayer <= enemyVisionScript.player.GetComponent<KinematicInput>().distanceToKnockback)
        //             // {
        //             //     Debug.Log("player gets hit / knocked back");
        //             //     
        //             //     // hit player
        //             //     //enemyVisionScript.player.GetComponent<PlayersHealth>().TakeDamageFromAttack(50);
        //             //     
        //             //     // push player back
        //
        //             //     
        //             //     // set state to flee with cool down timer
        //             //     Debug.Log("move to cooldown");
        //             
        //             // }
        //             
        //         }
        //         
        //     }
        //     
        // }
        // else
        // {
        //     float distanceToLastKnownPosition = Vector3.Distance(enemyVisionScript.lastKnownPlayersLocation.transform.position, animator.transform.position);
        //     
        //     if (distanceToLastKnownPosition <= 0.5f)
        //     {
        //         animator.SetTrigger(Idle);
        //     }
        //     else
        //     {
        //         enemyVisionScript.targetObject = enemyVisionScript.lastKnownPlayersLocation;
        //         
        //         //animator.SetTrigger(Search);
        //         
        //         animator.SetTrigger(GoToOriginPoint);
        //     }
        // }
        //
        //
        //
        //
        //
        // // // check for player, is he visible? or behind a wall or something?
        // // //if (enemyVisionScript.ConeOfVision())
        // // if(enemyMovementScript.CanSeePlayer())
        // // {
        // //     //if (enemyVisionScript.rayToPlayer.collider != null)
        // //     //{
        // //         
        // //         
        // //         // update last known position if player is in view, ELSE transition to search last known position.
        // //         if (enemyVisionScript.rayToPlayer.collider.CompareTag("Wall"))
        // //         {
        // //             Debug.Log("searching for player");
        // //             
        // //             // lost player goto players last known position
        // //             //animator.SetTrigger(Search);
        // //             
        // //             
        // //             animator.SetTrigger(Idle); // THIS IS TEMPORATRY REMOVE THIS LATER
        // //         }
        // //         else if (enemyVisionScript.rayToPlayer.collider.CompareTag("Player"))
        // //         {
        // //             Debug.Log("chasing player still and updating last known position");
        // //             
        // //             if (enemyVisionScript.activeEnemyState != CurrentState.ChasePlayer)
        // //                 enemyVisionScript.activeEnemyState = CurrentState.ChasePlayer;
        // //             
        // //             // update last known location
        // //             enemyVisionScript.UpdatePlayersLastKnownPosition(enemyVisionScript.player.position);
        // //
        // //             // check if player is in range or the player
        // //             if (distanceToPlayer <= enemyVisionScript.attackDistance)
        // //             {
        // //                 Debug.Log("arrived at player");
        // //                 //animator.SetTrigger(ArrivedAtPlayer);
        // //             }
        // //             
        // //         }
        // //         
        // //         
        // //     //}
        // //
        // // }
        // // else
        // // {
        // //     if (animator.transform.position == enemyVisionScript.lastKnownPlayersLocation.transform.position)
        // //     {
        // //         animator.SetTrigger(Idle);
        // //     }
        // //     else
        // //     {
        // //         Debug.Log("search for player than return to idle");
        // //         
        // //         animator.SetTrigger(Search);
        // //     }
        // //         
        // // }
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
