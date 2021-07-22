using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // Variables
    [SerializeField] float jumpPadForce;

    /// <summary>
    /// Launch objects that collide with the Jump Pad
    /// </summary>
    /// <param name="collision">Collision object</param>
    private void OnCollisionEnter(Collision collision)
    {
        GameObject bouncer = collision.gameObject;
        Rigidbody body = bouncer.GetComponent<Rigidbody>();

        body.AddForce(transform.up * jumpPadForce, ForceMode.Impulse);
    }
}
