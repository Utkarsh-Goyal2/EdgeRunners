using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Death Panel")]
    public GameObject youDiedPanel; // Assign in Inspector

    [Header("Optional Settings")]
    public bool pauseGameOnDeath = true;
    public bool destroyPlayer = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the death zone
        if (other.CompareTag("Player"))
        {
            TriggerPlayerDeath(other.gameObject);
        }
    }

    void TriggerPlayerDeath(GameObject player)
    {
        // Enable the death panel
        if (youDiedPanel != null)
        {
            youDiedPanel.SetActive(true);
        }

        // Optional: Pause the game
        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
        }

        // Optional: Destroy or disable the player
        if (destroyPlayer)
        {
            Destroy(player);
        }
        else
        {
            player.SetActive(false);
        }

        Debug.Log("Player died from death zone!");
    }
}
