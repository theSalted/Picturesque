using PlayerInputSystem;
using UnityEngine.SceneManagement;

public class PlayerRespawnable : Respawnable
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
   

    public override void Respawn() {
        // Reload Scene
        FirstPersonController.instance.canMove = false;
        base.Respawn();
        PlayerManager.Instance.inventory = null;
        FirstPersonController.instance.canMove = true;
    }
}
