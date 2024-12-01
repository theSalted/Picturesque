using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadePerformance : MonoBehaviour
{
    public float fadeDuration = 1.0f; // ����ʱ��
    public float waitDuration = 2.0f; // �ȴ�ʱ��
    public GameObject itemPrefab; // ���ɵ���ƷԤ�Ƽ�
    public Transform spawnPoint; // ������Ʒ��λ��
    public CanvasGroup canvasGroup; // ����CanvasGroup���

    private bool hasTriggered = false; // ����Ƿ��Ѿ�������

    void Start()
    {
        // ȷ����Ļ����ʱ��͸����
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; // ���ñ��Ϊ�Ѵ���
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        // ������Ļ
        yield return StartCoroutine(FadeScreen(1.0f));

        // ������Ʒ
        Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);

        // �ȴ�������
        yield return new WaitForSeconds(waitDuration);

        // ������Ļ
        yield return StartCoroutine(FadeScreen(0.0f));

        // ���ÿշ���
        OnCutsceneEnd();

        // ���ô�������Collider
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
        // �������д�µ��߼�
    }
}
