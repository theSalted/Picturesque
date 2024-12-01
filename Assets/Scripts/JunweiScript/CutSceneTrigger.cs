using UnityEngine;
using Cinemachine;
using System.Collections;
using PlayerInputSystem;

public class CutSceneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera cutSceneCamera; // 切换到的虚拟摄像机
    public GameObject player; // 玩家对象
    public AgentFollowWaypoints deerAgent; // 鹿的AgentFollowWaypoints脚本
    public AudioSource voiceOver; // 语音音频源
    public float cutSceneDuration = 5.0f; // 切换镜头的时间

    private CinemachineVirtualCamera mainCamera; // 主摄像机
    private FirstPersonController playerController; // 玩家控制器
    private bool hasTriggered = false; // 触发器是否已经被触发

    void Start()
    {
        // 获取主摄像机和玩家控制器
        mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        playerController = player.GetComponent<FirstPersonController>();

        // 确保 AudioSource 的 playOnAwake 属性为 false
        if (voiceOver != null)
        {
            voiceOver.playOnAwake = false;
        }

        // 调试日志
        Debug.Log("Main Camera: " + mainCamera.name);
        Debug.Log("Player Controller: " + playerController.name);

        // 检查 AudioSource 和音频剪辑
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
            hasTriggered = true; // 标记为已触发
            StartCoroutine(PlayCutScene());
        }
    }

    private IEnumerator PlayCutScene()
    {
        // 切换到cutSceneCamera
        Debug.Log("Switching to CutScene Camera");
        mainCamera.Priority = 0;
        cutSceneCamera.Priority = 10;

        // 锁定玩家移动
        playerController.enabled = false;

        // 启动鹿的动画
        if (deerAgent != null)
        {
            deerAgent.startFollowing = true;
        }

        // 播放语音
        if (voiceOver != null && voiceOver.clip != null)
        {
            Debug.Log("Playing Voice Over");
            voiceOver.Play();
        }
        else
        {
            Debug.LogWarning("Voice Over clip is missing or AudioSource is not assigned.");
        }

        // 等待指定的切换镜头时间
        yield return new WaitForSeconds(cutSceneDuration);

        // 恢复主摄像机
        Debug.Log("Switching back to Main Camera");
        cutSceneCamera.Priority = 0;
        mainCamera.Priority = 10;

        // 恢复玩家控制
        playerController.enabled = true;

        // 等待语音播放完毕
        if (voiceOver != null && voiceOver.clip != null)
        {
            yield return new WaitForSeconds(voiceOver.clip.length - cutSceneDuration);
        }

        // 销毁触发对象
        Destroy(gameObject);
    }
}
