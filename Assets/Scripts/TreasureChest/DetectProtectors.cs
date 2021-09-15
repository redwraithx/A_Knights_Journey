
using UnityEngine;

public class DetectProtectors : MonoBehaviour
{
    public GuardedChest guardedChest;

    private float timer = 5f;

    private void Start()
    {
        guardedChest = GetComponentInParent<GuardedChest>();
        
        
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        
        if(timer < 0f)
            Destroy(this);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
            guardedChest.AddGuard(other.gameObject);
    }
    
    
    
}
