using UnityEngine;
using Cinemachine;
using System.Collections;
using PlayerInputSystem;

public class CutSceneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera cutSceneCamera; // 切换到的虚拟摄像机
    public GameObject player; // 玩家对象

    //我不确定这个动画逻辑
    public Animator deerAnimator; // 鹿的动画控制器


    public AudioSource voiceOver; // 语音音频源
    public string deerAnimationName; // 鹿的动画名称

    private CinemachineVirtualCamera mainCamera; // 主摄像机
    private FirstPersonController playerController; // 玩家控制器

    void Start()
    {
        // 获取主摄像机和玩家控制器
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
        // 切换到cutSceneCamera
        mainCamera.Priority = 0;
        cutSceneCamera.Priority = 10;

        // 锁定玩家移动
        playerController.enabled = false;

        // 播放鹿的动画和语音
        deerAnimator.Play(deerAnimationName);
        voiceOver.Play();

        // 等待动画和语音播放完毕
        yield return new WaitForSeconds(deerAnimator.GetCurrentAnimatorStateInfo(0).length);

        // 恢复主摄像机
        cutSceneCamera.Priority = 0;
        mainCamera.Priority = 10;

        // 恢复玩家控制
        playerController.enabled = true;
    }
}

