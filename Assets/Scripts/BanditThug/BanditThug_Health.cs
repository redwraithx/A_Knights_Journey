
using UnityEngine;
using EnemyStates;


public class BanditThug_Health : MonoBehaviour
{
    public BanditThug_Vision enemyVisionScript = null;
    public EnemyAttributes enemyAttributes = null;
    
    public int maxHealth = 50;
    public int currentHealth = 0;

    public float takeDamageDuration = 1f;

    public Animator anim = null;


    public bool isDead = false;
    public bool isTakingDamage = false;


    private static readonly int TakeDamage = Animator.StringToHash("TakeDamage");
    private static readonly int Death = Animator.StringToHash("Death");

    private void Start()
    {
        if (!enemyVisionScript)
            enemyVisionScript = GetComponent<BanditThug_Vision>();

        if (!enemyAttributes)
            enemyAttributes = GetComponent<EnemyAttributes>();
        
        if(!anim)
            anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        isDead = false;

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
            
        }
        
    }




    public void TakeHitFromPlayer(int value)
    {
        Debug.Log("something is trying to hit this enemy: " + gameObject.name);
        
        if (isTakingDamage)
            return;

        if (isDead || !enemyAttributes || !anim)
            return;

        isTakingDamage = true;

        var damageTaken = value - enemyAttributes.GetDamageProtection();

        // if we take less than 0 damage return 0 so we dont heal the enemy
        currentHealth -= (damageTaken < 0 ? 0 : damageTaken);

        anim.SetTrigger(TakeDamage);

        isTakingDamage = false;
    }
}
