
using UnityEngine;
using EnemyStates;


public class BanditGuard_Attack : MonoBehaviour
{
    public BanditGuard_Vision banditThugVision = null;
    public EnemyAttributes enemyAttributes = null;
    public BoxCollider boxCollider = null;

    public bool canDealDamageToPlayer = true;
    public bool hasHitPlayer = false;

    [Header("Can Attack Timer")] 
    public bool canAttack = false;
    public float canAttackDelayTime = 1.5f;
    public float canAttackTimer;
    public bool isAttackTimerRunning = true;

    private void Start()
    {
        if(!banditThugVision)
            banditThugVision = GetComponent<BanditGuard_Vision>();
        
        if(!enemyAttributes)
            enemyAttributes = GetComponent<EnemyAttributes>();
        
        if(!boxCollider)
            boxCollider = GetComponent<BoxCollider>();

        canAttackTimer = canAttackDelayTime;
        isAttackTimerRunning = true;
    }


    private void Update()
    {
        if (banditThugVision.activeEnemyState == CurrentState.Dead)
            return;


        if (isAttackTimerRunning)
        {
            canAttackTimer -= Time.deltaTime;
            if (canAttackTimer <= 0f)
            {
                isAttackTimerRunning = false;
                
            }


        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (banditThugVision.activeEnemyState == CurrentState.Dead)
        {
            boxCollider.enabled = false;
            return;
        }
        
        if (!canDealDamageToPlayer || !other.gameObject.CompareTag("Player") || hasHitPlayer)
            return;

        hasHitPlayer = true;
        
        other.gameObject.GetComponent<PlayersHealth>().TakeDamageFromAttack(enemyAttributes.GetWeaponDamage(), (other.transform.position - transform.position), enemyAttributes.GetKnockBackForce());
    }

    public void AttackColliderEnabled() => boxCollider.enabled = true;

    public void AttackColliderDisabled()
    {
        boxCollider.enabled = false;
        canDealDamageToPlayer = true;
        
        canAttackTimer = canAttackDelayTime;
        
        isAttackTimerRunning = true;

        hasHitPlayer = false;
    }
}
