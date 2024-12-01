using PlayerInputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnable : Respawnable
{
    private Vector3? respawnPoint = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame


    public override void Respawn()
    {
        // Reload Scene
        FirstPersonController.instance.canMove = false;
        base.Respawn();
        // Çå¿ÕÍæ¼ÒµÄ¿â´æ
        // PlayerManager.Instance.inventory = null;

        if (respawnPoint.HasValue)
        {
            FirstPersonController.instance.transform.position = respawnPoint.Value;
        }

        FirstPersonController.instance.canMove = true;
    }

    public void SetRespawnPoint(Vector3 position)
    {
        respawnPoint = position;
    }
}