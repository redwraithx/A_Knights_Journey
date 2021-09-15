using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class BanditBoss_Inventory : MonoBehaviour
{
    public bool hasDropedKey = false;
    public Transform localForward;

    public IEnumerator SpawnKey(float time)
    {
        if (hasDropedKey)
            yield return null;


        Instantiate(Resources.Load("rust_key"), new Vector3(transform.position.x, 0.2f, transform.position.z), quaternion.identity);
        hasDropedKey = true;

        yield return new WaitForSeconds(time);
        
        Destroy(gameObject);
    }
}
