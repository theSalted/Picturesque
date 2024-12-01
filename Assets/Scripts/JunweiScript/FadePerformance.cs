using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadePerformance : MonoBehaviour
{
    public float fadeDuration = 1.0f; // 渐变时间
    public float waitDuration = 2.0f; // 等待时间
    public GameObject itemPrefab; // 生成的物品预制件
    public Transform spawnPoint; // 生成物品的位置
    public CanvasGroup canvasGroup; // 引用CanvasGroup组件

    private bool hasTriggered = false; // 标记是否已经触发过

    void Start()
    {
        // 确保屏幕启动时是透明的
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; // 设置标记为已触发
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        // 渐暗屏幕
        yield return StartCoroutine(FadeScreen(1.0f));

        // 生成物品
        Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

        // 等待若干秒
        yield return new WaitForSeconds(waitDuration);

        // 渐亮屏幕
        yield return StartCoroutine(FadeScreen(0.0f));

        // 调用空方法
        OnCutsceneEnd();

        // 禁用触发器的Collider
        GetComponent<Collider>().enabled = false;
    }

    IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    void OnCutsceneEnd()
    {
        // 在这里编写新的逻辑
    }
}
