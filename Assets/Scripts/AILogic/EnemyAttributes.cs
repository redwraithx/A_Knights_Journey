

using System;
using EnemyStates;
using UnityEngine;

public class EnemyAttributes : MonoBehaviour
{
    public enum EnemyTypes
    {
        Object,
        Thug,
        Guard,
        Archer,
        Boss
    }
    
    
    
    
    
    [SerializeField] private bool canMove = true; 
    private bool CanMove
    {
        get => canMove;
        set => canMove = value;
    }

    public void EnableMovement(bool newValue) => CanMove = newValue;
    public bool GetMovementStatus() => CanMove; // use this to check if the enemy can move at all or is frozen
    
    
    
    
    [SerializeField] private int canHearPlayerRange = 5; 
    public int CanHearPlayerRange
    {
        get => canHearPlayerRange;
        set => canHearPlayerRange = value;
    }
    
    
    
    
    [SerializeField] private int attackDamage = 15; 
    private int AttackDamage
    {
        get => (attackDamage + attackBonus);
        set => attackDamage = value;
    }
    
    [SerializeField] private float attackRange = 1.2f; 
    private float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }
    
    [SerializeField, Range(0.2f, 25f)] private float attackSpeed = 1.2f; 
    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = value;
    }
    
    
    [SerializeField] private float attackRotationOffsetY = 0f; 
    public float AttackRotationOffsetY
    {
        get => attackRotationOffsetY;
        set => attackRotationOffsetY = value;
    }
    
    
    
    [SerializeField] private float knockBackForce = 3f; 
    private float KnockBackForce
    {
        get => knockBackForce;
        set => knockBackForce = value;
    }
    
    
    [SerializeField] private int attackBonus = 0; 
    private int AttackBonus 
    {
        get => attackBonus;
        set => attackBonus += value;
    }
    
    [SerializeField] private int damageProtection = 0; 
    private int DamageProtection
    {
        get => damageProtection;
        set => damageProtection = value;
    }
    
    
    // can add armor and attack here if there is time
    public int GetWeaponDamage() => (AttackDamage + AttackBonus);
    public void SetWeaponDamage(int value) => AttackDamage += value;
    public int GetDamageProtection() => DamageProtection;
    public float GetAttackRange() => AttackRange;
    public float GetAttackSpeed() => AttackSpeed;
    public float GetKnockBackForce() => KnockBackForce;
    
    // what type of character is this
    public EnemyTypes currentEnemyType;


    private void Start()
    {
        gameObject.name = "Bandit " + currentEnemyType;
    }


    public AIHelpers.MovementBehaviors GetCurrentMovementBehavior()//EnemyTypes enemyType)
    {

        switch (currentEnemyType)
        {
            case EnemyTypes.Thug:

                return GetComponent<BanditThug_Vision>().ActiveMovementBehavior;
            
            case EnemyTypes.Guard:
                
                return GetComponent<BanditGuard_Vision>().ActiveMovementBehavior;
            
            case EnemyTypes.Archer:
                
                return GetComponent<BanditArcher_Vision>().ActiveMovementBehavior;
            
            case EnemyTypes.Boss:
                
                return GetComponent<BanditBoss_Vision>().ActiveMovementBehavior;
            
            default:
                throw new Exception($"Error! getting enemy movement behavior for {gameObject.name} has incorrect or not set EnemyType in the EnemyHealth script");
        }

    }
    
    public RaycastHit GetCurrentRaycastHitToPlayer()//EnemyTypes enemyType)
    {

        switch (currentEnemyType)
        {
            case EnemyTypes.Thug:

                return GetComponent<BanditThug_Vision>().rayToPlayer;
            
            case EnemyTypes.Guard:
                
                return GetComponent<BanditGuard_Vision>().rayToPlayer;
            
            case EnemyTypes.Archer:
                
                return GetComponent<BanditArcher_Vision>().rayToPlayer;
            
            case EnemyTypes.Boss:
                
                return GetComponent<BanditBoss_Vision>().rayToPlayer;
            
            default:
                throw new Exception($"Error! getting enemy movement behavior for {gameObject.name} has incorrect or not set EnemyType in the EnemyHealth script");
        }

    }
    
    public CurrentState GetCurrentState()//EnemyTypes enemyState)
    {

        switch (currentEnemyType)
        {
            case EnemyTypes.Thug:

                return GetComponent<BanditThug_Vision>().activeEnemyState;
            
            case EnemyTypes.Guard:
                
                return GetComponent<BanditGuard_Vision>().activeEnemyState;
            
            case EnemyTypes.Archer:
                
                return GetComponent<BanditArcher_Vision>().activeEnemyState;
            
            case EnemyTypes.Boss:
                
                return GetComponent<BanditBoss_Vision>().activeEnemyState;
            
            default:
                throw new Exception($"Error! getting enemy state for {gameObject.name} has incorrect or not set EnemyType in the EnemyHealth script");
        }

    }
    
    public RaycastHit GetVisionRayToPlayer()//EnemyTypes enemyState)
    {

        switch (currentEnemyType)
        {
            case EnemyTypes.Thug:

                return GetComponent<BanditThug_Vision>().rayToLastKnownPOS;
            
            case EnemyTypes.Guard:
                
                return GetComponent<BanditGuard_Vision>().rayToLastKnownPOS;
            
            case EnemyTypes.Archer:
                // the archer will never use this as they will protect the spot they are in at all times until death
                //return GetComponent<BanditArcher_Vision>().rayToLastKnownPOS;
                return new RaycastHit();
            
            case EnemyTypes.Boss:
                
                return GetComponent<BanditBoss_Vision>().rayToLastKnownPOS;
            
            default:
                throw new Exception($"Error! getting enemy state for {gameObject.name} has incorrect or not set EnemyType in the EnemyHealth script");
        }

    }

    public bool GetIsDeadStatus()
    {
        
        switch (currentEnemyType)
        {
            case EnemyTypes.Thug:

                return GetComponent<BanditThug_Health>().isDead;
            
            case EnemyTypes.Guard:
                
                return GetComponent<BanditGuard_Health>().isDead;
            
            case EnemyTypes.Archer:
                // the archer will never use this as they will protect the spot they are in at all times until death
                //return GetComponent<BanditArcher_Vision>().rayToLastKnownPOS;
                return GetComponent<BanditArcher_Health>().isDead;
            
            case EnemyTypes.Boss:
                
                return GetComponent<BanditBoss_Health>().isDead;
            
            default:
                throw new Exception($"Error! getting enemy state for {gameObject.name} has incorrect or not set EnemyType in the EnemyHealth script");
        }
        
    }
    
        
    
}
