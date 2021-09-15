using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName="RedWraith's Assets/TreasureChestLoot/CharacterUpgrade")]
public class LootableCharacterUpgrade : ScriptableObject
{
    public enum StatType
    {
        Armor,
        Weapon,
        Magic
    }

    public StatType statType;
    [Tooltip("Magic \"StatType\" is a float, Armor & Weapon \"StatType's\" are integers"), Range(0, 100)] public float stateValueAdjustment;

    public Image upgradeIcon;
    
    
    [Tooltip("the amount given will be added to the end of the object. EXAMPLE(Armor Plus = Armor Plus 2)")] 
    public string displayedItemName;

    public Color32 displayedItemNameColor;
    // returns the value of the type needed, rounds for armor and weapon
    public float GetArmorOrWeaponUpgradeValue() => statType == StatType.Magic ? stateValueAdjustment : Mathf.Ceil(stateValueAdjustment);

    public Image GetUpgradeIcon() => upgradeIcon;
    
}
