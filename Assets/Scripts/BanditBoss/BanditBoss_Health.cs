
using System.Collections.Generic;
using UnityEngine;
using EnemyStates;


public class BanditBoss_Health : MonoBehaviour
{
    public BanditBoss_Vision enemyVisionScript = null;
    public EnemyAttributes enemyAttributes = null;


    public List<GameObject> enemiesInWaiting;
    
    public int maxHealth = 50;
    public int currentHealth = 0;

    public float takeDamageDuration = 1f;
    //public float currentTakeDamageTimer = 0f;

    public Animator anim = null;

    public bool isEnraged = false;

    public bool isDead = false;
    public bool isTakingDamage = false;

    public bool usedShield = false;
    public bool isShieldDown = false;
    public GameObject shieldObj;
    
    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Enraged = Animator.StringToHash("Enraged");
    private static readonly int Shield = Animator.StringToHash("Shield");
    private static readonly int ChasePlayer = Animator.StringToHash("ChasePlayer");

    private void Start()
    {
        anim = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        isDead = false;

        //currentTakeDamageTimer = takeDamageDuration;
        
        enemyVisionScript = GetComponent<BanditBoss_Vision>();
        enemyAttributes = GetComponent<EnemyAttributes>();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " is dead");
            if (!isDead)
            {
                //enemyVisionScript.activeEnemyState = CurrentState.Dead;

                isDead = true;
                
                anim.SetTrigger(Death);
            }
            
            return;
        }

        
        // are we at 50% life?
        var lifeRemaining = maxHealth * 0.50;
        
        Debug.Log("Life remaining: " + lifeRemaining);

        if (currentHealth <= (int)lifeRemaining  && !usedShield)
        {
            usedShield = true;

            enemyVisionScript.ActiveMovementBehavior = AIHelpers.MovementBehaviors.Idle;
            enemyVisionScript.activeEnemyState = CurrentState.Idle;

            enemyVisionScript.enabled = false;

            shieldObj.SetActive(true);
            
            foreach (var enemy in enemiesInWaiting)
            {
                enemy.SetActive(true);
            }
            
            anim.SetTrigger(Shield);
        }


        if (usedShield && !isShieldDown)
        {
            if (enemiesInWaiting.Count > 0)
            {
                foreach (var gameObj in enemiesInWaiting)
                {
                    if (!gameObj)
                        enemiesInWaiting.Remove(gameObj);
                }
            }
        }
        

        if (enemiesInWaiting.Count == 0 && shieldObj.activeInHierarchy)
        {
            isShieldDown = true;
            

            Destroy(shieldObj);

            enemyVisionScript.enabled = true;
            enemyVisionScript.ActiveMovementBehavior = AIHelpers.MovementBehaviors.SeekKinematic;
            enemyVisionScript.activeEnemyState = CurrentState.ChasePlayer;

            anim.SetTrigger(ChasePlayer);
        }
        
        
        
        // are we are 10% life?
        lifeRemaining = maxHealth * 0.90;
        
        Debug.Log("Life reminaing: " + lifeRemaining);

        if (currentHealth <= (int)lifeRemaining  && !isEnraged)
        {
            isEnraged = true;
            
            anim.SetTrigger(Enraged);
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

        if (isDead)
            return;

        isTakingDamage = true;

        var damageTaken = value - enemyAttributes.GetDamageProtection();

        // if we take less than 0 damage return 0 so we dont heal the enemy
        currentHealth -= (damageTaken < 0 ? 0 : damageTaken);

        GetComponent<Animator>().SetTrigger(TakeDamage);

        isTakingDamage = false;
    }
}
