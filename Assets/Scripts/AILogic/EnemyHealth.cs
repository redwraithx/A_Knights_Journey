
using System;
using EnemyStates;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    
    
    
    public int maxHealth = 50;
    public int currentHealth = 0;

    public float takeDamageDuration = 1f;
    //public float currentTakeDamageTimer = 0f;

    public Animator anim = null;


    public bool isDead = false;
    public bool isTakingDamage = false;


    
    
    
    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    private static readonly int Death = Animator.StringToHash("Death");

    private void Start()
    {
        anim = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        isDead = false;
        
        //currentTakeDamageTimer = takeDamageDuration;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            if (!isDead)
            {
                anim.SetTrigger(Death);

                GetComponent<BanditThug_Vision>().activeEnemyState = CurrentState.Dead;
                    
                isDead = true;
            }
            
            return;
        }



        // if (isTakingDamage)
        // {
        //     currentTakeDamageTimer -= Time.deltaTime;
        //
        //     if (currentTakeDamageTimer <= 0f)
        //     {
        //         isTakingDamage = false;
        //
        //         currentTakeDamageTimer = takeDamageDuration;
        //     }
        // }
    }




    public void TakeHitFromPlayer(int value)
    {
        if (isTakingDamage)
            return;
        
        if(isDead)
            return;

        isTakingDamage = true;

        var damageTaken = value - GetComponent<EnemyAttributes>().GetDamageProtection();
        
        // if we take less than 0 damage return 0 so we dont heal the enemy
        currentHealth -= (damageTaken < 0 ? 0 : damageTaken);
        
        GetComponent<Animator>().SetTrigger(TakeDamage);
        
        isTakingDamage = false;
    }



    
    
}
