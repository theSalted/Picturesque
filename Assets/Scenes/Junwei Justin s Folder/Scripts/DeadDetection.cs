using UnityEngine;

public class DeadDetection : MonoBehaviour
{
    public Transform respawnPoint; // ������

    private void OnTriggerEnter(Collider other)
    {
        // �����ײ���Ƿ������
        if (other.CompareTag("Player"))
        {
            // �������λ�õ�������
            other.transform.position = respawnPoint.position;
        }
    }
}
