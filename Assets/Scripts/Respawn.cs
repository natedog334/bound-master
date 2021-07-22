using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    // Variables
    [SerializeField] Transform spawnPoint;
    [SerializeField] float minHeightForDeath;
    [SerializeField] GameObject player;

    /// <summary>
    /// If the Player falls to far downward, reload the current scene
    /// </summary>
    void Update()
    {
        if (player.transform.position.y < minHeightForDeath)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
