
using TMPro;
using UnityEngine;

public class ItemLootedLerp : MonoBehaviour
{
    [Header("Lerping variables")]
    private bool isItemLerping = false;

    public float timeStartedLerping = 0f;
    public float lerpTime = 3f;

    public Vector3 startingPosition = Vector3.zero;
    public Vector3 endPosition = Vector3.zero;

    [Header("References")]
    public TMP_Text thisText = null;
    public GameObject playerRef = null;

    private void Start()
    {
        if (!thisText)
            thisText = GetComponentInChildren<TMP_Text>();

        if (!playerRef)
            playerRef = GameManager.Instance.playerReference;
        
        startingPosition = transform.position;

        endPosition = startingPosition + (lerpTime * Vector3.up);
        
        StartLerping();

        lerpTime += UnityEngine.Random.Range(0f, 3f);
        
        Destroy(gameObject, UnityEngine.Random.Range(2f, 3.5f));
    }

    private void Update()
    {
        if (!playerRef)
            if(GameManager.Instance.playerReference)
                playerRef = GameManager.Instance.playerReference;
        
        if (isItemLerping)
        {
            transform.position = MyLerp(startingPosition, endPosition, timeStartedLerping, lerpTime);
            
            var lookingDirection = transform.position - playerRef.transform.position;
            lookingDirection.y = 0f;

            var rotation = Quaternion.LookRotation(lookingDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
        }

        if (Vector3.Distance(startingPosition, endPosition) <= 0.2f)
        {
            Destroy(gameObject);
        }
    }


    private void StartLerping()
    {
        timeStartedLerping = Time.time;

        isItemLerping = true;
    }

    private Vector3 MyLerp(Vector3 startingPosition, Vector3 endPosition, float timeStartedLerping, float lerpTime = 1f)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        return Vector3.Lerp(startingPosition, endPosition, percentageComplete);
    }
}
