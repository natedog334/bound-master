using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    Rigidbody body;
    CapsuleCollider collider;

    float originalHeight;
    public float reducedHeight;

    public float slideSpeed;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponentInChildren<CapsuleCollider>();
        body = GetComponent<Rigidbody>();
        originalHeight = collider.height;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.W))
        {
            Sliding();
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            GetUp();
        }
    }

    void Sliding()
    {
        collider.height = reducedHeight;
        body.AddForce(transform.forward * slideSpeed, ForceMode.VelocityChange);
    }

    void GetUp()
    {
        collider.height = originalHeight;
    }
}
