using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // Variables
    [SerializeField] Transform cameraPosition;

    /// <summary>
    /// Moves camera along with camera holder
    /// </summary>
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
