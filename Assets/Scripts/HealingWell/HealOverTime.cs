using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class HealOverTime : MonoBehaviour
{
    [SerializeField] internal int initialValue = 15;
    [SerializeField] internal int valueOverTime = 5;
    [SerializeField] private bool otherIsInRange = false;
    [SerializeField] private float healingRangeOfHealingWell;
    [SerializeField] private bool isValueOverTimeTimerRunning = false;
    [SerializeField] private float valueOverTimeTimer = 0;
    [SerializeField] private float valueOverTimeDelayTime = 1;
    

    [SerializeField] private bool firstPlayerHeal = true;

    [SerializeField] private float distanceToPlayer;
    
    private bool canAdjustOthersHealth = false;
    private PlayersHealth playersHealth = null;
    //private EnemyHealth othersHealth2 = null;


    private float coolDownTimer;
    private float coolDownDelayTime = 5f;

    private void Start()
    {
        playersHealth = GameObject.FindWithTag("Player").GetComponent<PlayersHealth>();

        healingRangeOfHealingWell = GetComponent<SphereCollider>().radius;

        valueOverTimeTimer = valueOverTimeDelayTime;

        coolDownTimer = coolDownDelayTime;
    }


    private void Update()
    {
        if (!playersHealth)
        {
            Debug.Log("player is dead");

            return;
        }

        distanceToPlayer = Vector3.Distance(transform.position, playersHealth.transform.position);

        bool isPlayerInRange = IsPlayerInRange();

        if (isPlayerInRange)
        {
            valueOverTimeTimer -= Time.deltaTime;

            if (valueOverTimeTimer <= 0f)
            {
                if (firstPlayerHeal)
                {
                    playersHealth.HealPlayer(initialValue);
                    firstPlayerHeal = false;
                }
                else
                {
                    playersHealth.HealPlayer(valueOverTime);
                }

                valueOverTimeTimer = valueOverTimeDelayTime;
            }
        }
        else
        {
            if (!firstPlayerHeal)
            {
                coolDownTimer -= Time.deltaTime;

                if (coolDownTimer <= 0f)
                {
                    coolDownTimer = coolDownDelayTime;

                    firstPlayerHeal = true;
                }
            }
            else
            {
                
            }
            
            
            
        }
        
        
    }
    //
    //     distanceToPlayer = Vector3.Distance(transform.position, playersHealth.transform.position);
    //     
    //     
    //     if (otherIsInRange && isValueOverTimeTimerRunning)
    //     {
    //         valueOverTimeTimer -= Time.deltaTime;
    //         
    //         if (valueOverTimeTimer < 0f )
    //         {
    //             canAdjustOthersHealth = true;
    //             
    //             valueOverTimeTimer = 1f;
    //         }
    //     }
    //     else if (!otherIsInRange)
    //     {
    //         isValueOverTimeTimerRunning = false;
    //         canAdjustOthersHealth = false;
    //
    //         firstPlayerHeal = true;
    //
    //         valueOverTimeTimer = 1f;
    //     }
    //
    //     var playerInRange = IsPlayerInRange();
    //
    //     if (playerInRange && canAdjustOthersHealth)
    //     {
    //         if (firstPlayerHeal)
    //         {
    //             playersHealth.HealPlayer(initialValue);
    //             
    //             Debug.Log("first heal");
    //             
    //             firstPlayerHeal = false;
    //         }
    //         else
    //         {
    //             Debug.Log("heal over time");
    //             playersHealth.HealPlayer(valueOverTime);
    //         }
    //     }
    //     else if (IsPlayerInRange() && !canAdjustOthersHealth)
    //     {
    //         Debug.Log("heal for player not ready");
    //     }
    //     else
    //     {
    //         // set timer for this to reset first heal after a set time limit
    //         Debug.Log("Player is out of range");
    //         firstPlayerHeal = true;
    //     }
    // }

    


    private bool IsPlayerInRange()
    {
        bool isInRange = false;
        
        isInRange = distanceToPlayer <= healingRangeOfHealingWell;

        otherIsInRange = isInRange;

        isValueOverTimeTimerRunning = isInRange;
        
        return isInRange;
    }
    
    
}
