using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    private bool smash;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            smash = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            smash = false;
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            smash = true;
            rb.velocity = new Vector3(0, -100 * Time.fixedDeltaTime * 7, 0);
        }

        if (rb.velocity.y > 5)
            rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!smash)
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
        }
        else
        {
            if(collision.gameObject.tag == "enemy")
            {
                Destroy(collision.transform.parent.gameObject);
            }

            if(collision.gameObject.tag == "plane")
            {
                print("Game Over");
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!smash)
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
        }
    }
}
