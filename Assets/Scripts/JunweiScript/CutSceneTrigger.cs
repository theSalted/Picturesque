using UnityEngine;
using Cinemachine;
using System.Collections;
using PlayerInputSystem;

public class CutSceneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera cutSceneCamera; // �л��������������
    public GameObject player; // ��Ҷ���

    //�Ҳ�ȷ����������߼�
    public Animator deerAnimator; // ¹�Ķ���������


    public AudioSource voiceOver; // ������ƵԴ
    public string deerAnimationName; // ¹�Ķ�������

    private CinemachineVirtualCamera mainCamera; // �������
    private FirstPersonController playerController; // ��ҿ�����

    void Start()
    {
        // ��ȡ�����������ҿ�����
        mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        playerController = player.GetComponent<FirstPersonController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            StartCoroutine(PlayCutScene());
        }
    }

    private IEnumerator PlayCutScene()
    {
        // �л���cutSceneCamera
        mainCamera.Priority = 0;
        cutSceneCamera.Priority = 10;

        // ��������ƶ�
        playerController.enabled = false;

        // ����¹�Ķ���������
        deerAnimator.Play(deerAnimationName);
        voiceOver.Play();

        // �ȴ������������������
        yield return new WaitForSeconds(deerAnimator.GetCurrentAnimatorStateInfo(0).length);

        // �ָ��������
        cutSceneCamera.Priority = 0;
        mainCamera.Priority = 10;

        // �ָ���ҿ���
        playerController.enabled = true;
    }
}

