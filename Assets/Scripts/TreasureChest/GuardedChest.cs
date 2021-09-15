
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;




public class GuardedChest : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform playersPos = null;
    
    [Header("Item's loot Table")]
    [SerializeField] private LootableCharacterUpgrade[] lootTable;
    
    [Header("Chest Information and State")]
    [SerializeField] private List<GameObject> guardsOfTheChest = new List<GameObject>();
    [SerializeField] private bool isChestOpen = false;
    [SerializeField] private float distanceToOpenChest = 1f;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimatorController animControllerOpen;

    [Header("spawn loot table Text start / end positions")]
    [SerializeField] private Vector3 pointA = Vector3.zero;
    [SerializeField] private Vector3 pointB = Vector3.zero;
    [SerializeField] private float[] timeStartedLerping = new float[] {0f};
    private float[] changeInElapsedTime = new float[] {0f};  // each item in loot table will have its own change in time as to not reset any other objects already in motion.

    [SerializeField] private GameObject lootUI_TextPrefab = null;
    
    private void Start()
    {
        if (!lootUI_TextPrefab)
            throw new Exception($"Error! lootUI Prefab is missing from a chest: {gameObject.name}");
        
        playersPos = GameObject.FindWithTag("Player").transform;

        
    }

    public void AddGuard(GameObject guard)
    {
        foreach (GameObject chestGuard in guardsOfTheChest)
        {
            if (guard == chestGuard)
            {
                Debug.Log("found this guard in list already skipping");

                return;
            }
        }
        
        guardsOfTheChest.Add(guard);
    }

    private void Update()
    {
        // if (isChestOpen)
        //     return;
        //
        //
        // if (guardsOfTheChest.Count <= 0 && Vector3.Distance(transform.position, playersPos.position) <= distanceToOpenChest)
        // {
        //     
        // }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("guards of this chest count: " + GetGuardCount());
        if(GetGuardCount() <= 0 && playersPos)
        {
            if (!isChestOpen && Input.GetKeyDown(KeyCode.E))
            {
                anim.runtimeAnimatorController = animControllerOpen;

                // give the player the type of loot in the chest
                foreach (LootableCharacterUpgrade loot in lootTable)
                {
                    if (loot.statType == LootableCharacterUpgrade.StatType.Armor)
                    {
                        other.GetComponent<PlayerAttributes>().AddArmorProtectionUpgrade((int)loot.stateValueAdjustment);


                        DisplayLootedText(loot.displayedItemName, loot.displayedItemNameColor);
                    }
                    else if (loot.statType == LootableCharacterUpgrade.StatType.Weapon)
                    {
                        other.GetComponent<PlayerAttributes>().AddWeaponDamageUpgrade((int)loot.stateValueAdjustment);
                        
                        DisplayLootedText(loot.displayedItemName, loot.displayedItemNameColor);
                    }
                    else // magic reuse time
                    {
                        other.GetComponent<PlayerAttributes>().AddMagicReuseTimeReductionUpgrade(loot.stateValueAdjustment);
                        
                        DisplayLootedText(loot.displayedItemName, loot.displayedItemNameColor);
                    }
                }

                isChestOpen = true;
            }
        }
        else
        {
            Debug.Log("you need to kill the enemies guarding this chest first");
        }
    }


    private int GetGuardCount()
    {
        int guardCount = 0;

        foreach (GameObject guard in guardsOfTheChest)
        {
            if (!guard)
                continue;

            guardCount++;
        }

        return guardCount;
    }


    private void DisplayLootedText(string lootName, Color32 color)//int colorType)
    {
        var timeSet = UnityEngine.Random.Range(3f, 16f);
        
        // instantiate object
        GameObject lootedItem = Instantiate(lootUI_TextPrefab, transform.position + new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(0.4f, 1.6f), UnityEngine.Random.Range(-0.8f, 0.8f)), Quaternion.identity);

        
        
        GameManager.Instance.PowerUpsCollected();
        
        

        var item = lootedItem.GetComponent<ItemLootedLerp>();
        
        // update text color
        color.a = 255; // make sure it's visible
        item.thisText.color = color; 
        
        
        // update text on object
        item.thisText.text = lootName;
        
        // update time for transition
        item.lerpTime = timeSet;

    }

}
