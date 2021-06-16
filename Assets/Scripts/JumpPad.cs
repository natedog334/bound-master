using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] float jumpPadForce;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject bouncer = collision.gameObject;
        Rigidbody body = bouncer.GetComponent<Rigidbody>();

        body.AddForce(transform.up * jumpPadForce, ForceMode.Impulse);
    }
}
