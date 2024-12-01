using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawnable playerRespawnable = other.GetComponent<PlayerRespawnable>();
            if (playerRespawnable != null)
            {
                playerRespawnable.SetRespawnPoint(transform.position);
            }
        }
    }
}