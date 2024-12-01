using UnityEngine;
using Cinemachine;
using System.Collections;
using PlayerInputSystem;

public class CutSceneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera cutSceneCamera; // �л��������������
    public GameObject player; // ��Ҷ���
    public AgentFollowWaypoints deerAgent; // ¹��AgentFollowWaypoints�ű�
    public AudioSource voiceOver; // ������ƵԴ
    public float cutSceneDuration = 5.0f; // �л���ͷ��ʱ��

    private CinemachineVirtualCamera mainCamera; // �������
    private FirstPersonController playerController; // ��ҿ�����
    private bool hasTriggered = false; // �������Ƿ��Ѿ�������

    void Start()
    {
        // ��ȡ�����������ҿ�����
        mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        playerController = player.GetComponent<FirstPersonController>();

        // ȷ�� AudioSource �� playOnAwake ����Ϊ false
        if (voiceOver != null)
        {
            voiceOver.playOnAwake = false;
        }

        // ������־
        Debug.Log("Main Camera: " + mainCamera.name);
        Debug.Log("Player Controller: " + playerController.name);

        // ��� AudioSource ����Ƶ����
        if (voiceOver == null)
        {
            Debug.LogError("AudioSource is not assigned.");
        }
        else if (voiceOver.clip == null)
        {
            Debug.LogError("AudioSource clip is not assigned.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && !hasTriggered)
        {
            Debug.Log("Trigger Entered by Player");
            hasTriggered = true; // ���Ϊ�Ѵ���
            StartCoroutine(PlayCutScene());
        }
    }

    private IEnumerator PlayCutScene()
    {
        // �л���cutSceneCamera
        Debug.Log("Switching to CutScene Camera");
        mainCamera.Priority = 0;
        cutSceneCamera.Priority = 10;

        // ��������ƶ�
        playerController.enabled = false;

        // ����¹�Ķ���
        if (deerAgent != null)
        {
            deerAgent.startFollowing = true;
        }

        // ��������
        if (voiceOver != null && voiceOver.clip != null)
        {
            Debug.Log("Playing Voice Over");
            voiceOver.Play();
        }
        else
        {
            Debug.LogWarning("Voice Over clip is missing or AudioSource is not assigned.");
        }

        // �ȴ�ָ�����л���ͷʱ��
        yield return new WaitForSeconds(cutSceneDuration);

        // �ָ��������
        Debug.Log("Switching back to Main Camera");
        cutSceneCamera.Priority = 0;
        mainCamera.Priority = 10;

        // �ָ���ҿ���
        playerController.enabled = true;

        // �ȴ������������
        if (voiceOver != null && voiceOver.clip != null)
        {
            yield return new WaitForSeconds(voiceOver.clip.length - cutSceneDuration);
        }

        // ���ٴ�������
        Destroy(gameObject);
    }
}
