using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] float minHeightForDeath;
    [SerializeField] GameObject player;

    void Update()
    {
        if (player.transform.position.y < minHeightForDeath)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
