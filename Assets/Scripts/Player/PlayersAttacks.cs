using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayersAttacks : MonoBehaviour
{
    [SerializeField] private bool canAttack = true;
    
    [SerializeField, Range(0.5f, 15f), Header("Normal Attack Information")] private float normalAttackDelay = 1.5f;
    [SerializeField] private bool canUseNormalAttack = false;
    [SerializeField] private float normalAttacksTimer;
    [SerializeField] private bool normalAttackInUse = false;

    
    [Space, SerializeField, Range(0.5f, 25f), Header("Hard Attack Information")] private float hardAttackDelay = 8f;
    [SerializeField] private bool canUseHardAttack = false;
    [SerializeField] private float hardAttacksTimer;
    [SerializeField] private bool hardAttackInUse = false;

    [Space, SerializeField, Header("Weapon Box-RayCast Information")] private Transform middleOfSwordPointForRayCast = null;
    [SerializeField] private Vector3 halfSizeOfBoxRayCastInEachDimension = Vector3.zero;
    [SerializeField] private Transform startOfSwordsBlade = null;
    [SerializeField] private Transform endOfSwordsBlade = null;
    [SerializeField] private float weaponsBoxRayCastLength = 1f;
    private RaycastHit weaponHit;
    private bool dealingDamageToEnemy = false;

    [Space, SerializeField, Header("Required Components References")] private Animator anim = null;
    [SerializeField] private Transform swordReference;
    [SerializeField] private Transform swordParentReference;
    [SerializeField] private PlayerAttributes _playerAttributesReference;
    [SerializeField] private BoxCollider swordBladeCollider = null;
    
    private static readonly int AttackNormal = Animator.StringToHash("AttackNormal");

    public GameObject targetObject = null;
    
    
    private void Start()
    {
        anim = GetComponent<Animator>();

        _playerAttributesReference = GetComponent<PlayerAttributes>();
        
        normalAttacksTimer = normalAttackDelay;
        hardAttacksTimer = hardAttackDelay;

        canUseNormalAttack = false;
        canUseHardAttack = false;

        foreach(var collider in GetComponentsInParent<Collider>())
            Physics.IgnoreCollision(swordBladeCollider, collider, true);

    }
    

    private void Update()
    {
        
        // normal attacks
        if (!canUseNormalAttack)
        {
            normalAttacksTimer -= Time.deltaTime;
    
            if (normalAttacksTimer <= 0f)
            {
                canUseNormalAttack = true;
                
                normalAttacksTimer = normalAttackDelay;
    
                canAttack = true;
            }
        }
    
        if (canUseNormalAttack && Input.GetMouseButtonDown(0))
        {
            normalAttackInUse = true;
            
        }
        
        // lets see if we hit anything
        if (normalAttackInUse)
        {
            //Debug.Log("debug drawing swords attack");
            //Debug.DrawRay(startOfSwordsBlade.position, (endOfSwordsBlade.position - startOfSwordsBlade.position), Color.red, weaponsBoxRayCastLength);
        
           //UseWeaponAttack();
            
            if (canAttack)  // NEED TO RETHINK THIS ATTACK ITS OFF
            {
                canAttack = false;
        
                anim.SetTrigger(AttackNormal);
        
            }
    //
    //         if (!canAttack && weaponHit.collider != null)
    //         {
    //             targetObject = weaponHit.collider.gameObject.CompareTag("Enemy") ? weaponHit.collider.gameObject : null;
    //             
    //             if (targetObject && !dealingDamageToEnemy)
    //             {
    //                 dealingDamageToEnemy = true;
    //                 
    //                 Debug.Log("sword hit enemy, deal damage");
    //                 
    //                 targetObject?.GetComponent<EnemyHealth>().TakeHitFromPlayer(_playerAttributesReference.GetWeaponDamage());
    //
    //                 targetObject = null;
    //                 
    //                 canUseNormalAttack = false;
    //
    //                 dealingDamageToEnemy = false;
    //             }
    //         }
    //
    //     }
    //     
    //     
    //     // hard attacks
    //     if (!canUseHardAttack)
    //     {
    //         normalAttacksTimer -= Time.deltaTime;
    //
    //         if (normalAttacksTimer <= 0f)
    //         {
    //             canUseHardAttack = true;
    //             
    //             hardAttacksTimer = hardAttackDelay;
    //         }
        }
    
        
        
        
    }
    //
    //
    // public bool UseWeaponAttack() => Physics.BoxCast(middleOfSwordPointForRayCast.position, halfSizeOfBoxRayCastInEachDimension, Vector3.forward, out weaponHit, swordReference.rotation, weaponsBoxRayCastLength);
    //
    // // enable raycast
    // public void WeaponAttackColliderEnabled()
    // {
    //     normalAttackInUse = true;
    // }
    //
    // // disable raycast
    // public void WeaponAttackColliderDisabled()
    // {
    //     normalAttackInUse = false;
    //     canUseNormalAttack = false;
    //     
    //     canAttack = false;
    // }


    private void OnDrawGizmos()
    {
        if (!normalAttackInUse)
            return;

        var forward = transform.TransformDirection(swordReference.forward);
        
        
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(middleOfSwordPointForRayCast.position, swordReference.rotation, swordReference.lossyScale);
        
        Gizmos.matrix = rotationMatrix;
        //Gizmos.DrawWireCube(middleOfSwordPointForRayCast.position, halfSizeOfBoxRayCastInEachDimension * 2f);
        
        Gizmos.DrawWireCube(middleOfSwordPointForRayCast.localPosition - (Vector3.forward * 0.5f), halfSizeOfBoxRayCastInEachDimension * 2f);
        
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawCube(middleOfSwordPointForRayCast.position, halfSizeOfBoxRayCastInEachDimension * 2f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Treasure"))
            return;
        
        Debug.Log("Deal damage");

        if (!other.gameObject.CompareTag("Enemy") || swordBladeCollider.enabled == false)
            return;
        
        switch (other.gameObject.GetComponent<EnemyAttributes>().currentEnemyType)
        {
            case EnemyAttributes.EnemyTypes.Thug: //"Bandit_Thug":
                Debug.Log("hit Thug");
                other.gameObject.GetComponent<BanditThug_Health>().TakeHitFromPlayer(_playerAttributesReference.GetWeaponDamage());
                break;
            case EnemyAttributes.EnemyTypes.Guard: //"Bandit_Guard":
                Debug.Log("hit guard");
                other.gameObject.GetComponent<BanditGuard_Health>().TakeHitFromPlayer(_playerAttributesReference.GetWeaponDamage());
                break;
            case EnemyAttributes.EnemyTypes.Archer: //"Bandit_Archer":
                Debug.Log("hit archer");
                
                other.gameObject.GetComponent<BanditArcher_Health>().TakeHitFromPlayer(_playerAttributesReference.GetWeaponDamage());
                break;
            case EnemyAttributes.EnemyTypes.Boss: //"Bandit_Boss":
                Debug.Log("hit Boss");
                
                other.gameObject.GetComponent<BanditBoss_Health>().TakeHitFromPlayer(_playerAttributesReference.GetWeaponDamage());
                break;
            default:
                Debug.Log("default of switch was called for: " + other.gameObject.name + ", you should not see this");
                break;
        }
    }

    public void WeaponColliderEnabled()
    {
        swordBladeCollider.enabled = true;
    }

    public void WeaponColliderDisabled()
    {
        swordBladeCollider.enabled = false;
        
        normalAttackInUse = false;
        canUseNormalAttack = false;
        
        canAttack = false;
    }
}
