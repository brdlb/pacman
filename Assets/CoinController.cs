using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField]
    private float respawnDelay = 2f;

    private static readonly Vector3 HiddenPosition = new Vector3(-1000, -1000, 0);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddScore();
            }
            else
            {
                Debug.LogWarning("PlayerController not found on colliding object.", other.gameObject);
            }

            transform.position = HiddenPosition;
            StartCoroutine(WaitAndRespawn());
        }
    }

    IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        MazeRenderer mazeRenderer = gameObject.GetComponentInParent<MazeRenderer>();
        if (mazeRenderer != null)
        {
            mazeRenderer.SetCoinRandomPosition(this.transform);
        }
        else
        {
            Debug.LogError("MazeRenderer not found in parent!", this.gameObject);
        }
    }
}
