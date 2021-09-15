using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    [SerializeField, Range(0, 100)] private int damageProtection = 0;
    [SerializeField, Range(0, 100)] private int damageAttack = 0;
    [SerializeField, Range(0f, 100f)] private float magicReuseTimeReduction = 0f;
    
    [SerializeField] private int weaponDamage = 10;
    [SerializeField] private float playerHealingRecastTime = 60f; // 60 seconds, as there is no mana the limitation is in time, but power ups can reduce this
    
    public int GetDamageProtection() => damageProtection;
    public void AddArmorProtectionUpgrade(int value) => damageProtection += value;
    
    public int GetWeaponDamage() => (weaponDamage + damageAttack);
    public void AddWeaponDamageUpgrade(int value) => damageAttack += value;

    public float GetMagicTimeReduction() => magicReuseTimeReduction;
    public void AddMagicReuseTimeReductionUpgrade(float value) => magicReuseTimeReduction += value;
    
    public float GetRecastTimeForHealingSpell() => (playerHealingRecastTime - GetMagicTimeReduction() >= 1f ? (playerHealingRecastTime - GetMagicTimeReduction()) : 15f); // never get heals too often. needs a limit


    private void Start()
    {
        GameManager.Instance.playerReference = gameObject;
    }
}
