using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float moveSpeed, lifeTime;

    public Rigidbody rb;

    public GameObject impactEffect;
    public bool selfDestroy;

    private bool canCountdown = false;

    // Update is called once per frame
    void Update()
    {
        //theRB.velocity = transform.forward * moveSpeed;

        if (canCountdown)
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int layerMask = other.gameObject.layer;

        if (layerMask == 6)
        {
            //Debug.Log("BOOM");
            Destroy(other.gameObject);
            Instantiate(impactEffect, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
            if (selfDestroy)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (selfDestroy)
            {
                Instantiate(impactEffect, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    public void Countdown()
    {
        canCountdown = true;
    }
}
