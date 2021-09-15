using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Color colorStartMajorDamage = Color.red;
    Color colorNoDamage = Color.blue;
    Color colorStartMediumDamage = Color.yellow;
    float damageDisplayDuration = 0.8f;
    Renderer rend;

    float currentMajorDamageTimer = 0.0f;
    float currentMediumDamageTimer = 0.0f;

    public float mediumDamageCollisionForce = 200.0f;
    public float majorDamageCollisionForce = 350.0f;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        colorNoDamage = rend.material.color;

        currentMajorDamageTimer = 0.0f;
        currentMediumDamageTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMajorDamageTimer > 0.0f )
        {
            float lerp = Mathf.PingPong(Time.time, currentMajorDamageTimer) / currentMajorDamageTimer;
            rend.material.color = Color.Lerp(colorStartMajorDamage, colorNoDamage, lerp);

            currentMajorDamageTimer -= Time.deltaTime;
        }
        else if ( currentMediumDamageTimer > 0.0f )
        {
            float lerp = Mathf.PingPong(Time.time, currentMediumDamageTimer) / currentMediumDamageTimer;
            rend.material.color = Color.Lerp(colorStartMediumDamage, colorNoDamage, lerp);

            currentMediumDamageTimer -= Time.deltaTime;
        }
        else
        {
            rend.material.color = colorNoDamage;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;

        if (collisionForce < 100.0F)
        {
            // This collision has not damaged anyone...
        }
        else if (collisionForce < 200.0F)
        {
            currentMediumDamageTimer = damageDisplayDuration;
        }
        else
        {
            currentMajorDamageTimer = damageDisplayDuration;
        }
    }
}
