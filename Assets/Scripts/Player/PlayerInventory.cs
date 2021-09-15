
using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    public bool gotCageKey = false;

    public GameObject cageDoor = null;
    

    private void Update()
    {
       

        if (gotCageKey)
        {
            if (cageDoor)
            {
                Debug.Log("display key to open door - E");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    cageDoor.GetComponent<OpenCage>().PlayerOpensGame();
                    
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("touch key maybe?");

        if (other.gameObject.CompareTag("Treasure"))
        {
            Debug.Log("found treasure");
            
            if (other.gameObject.name == "rust_key" || other.gameObject.name == "rust_key(Clone)")
            {
                Debug.Log("found Rushy_Key");
                gotCageKey = true;

                Destroy(other.gameObject);
            }
        }

        if (other.gameObject.CompareTag("Door"))
        {
            Debug.Log("found door");
            
            cageDoor = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (cageDoor && other.gameObject.CompareTag("Door"))
            cageDoor = null;
    }
}
