using UnityEngine;

public class DeadDetection : MonoBehaviour
{
    public Transform respawnPoint; // 重生点

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的是否是玩家
        if (other.CompareTag("Player"))
        {
            // 重置玩家位置到重生点
            other.transform.position = respawnPoint.position;
        }
    }
}
