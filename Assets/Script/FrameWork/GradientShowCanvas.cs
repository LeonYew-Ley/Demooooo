using UnityEngine;

public class GradientShowCanvas : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Header("动画参数设置")]
    public float fadeDuration = 1f;
    public bool enableFadeIn = true;
    public bool enableFadeOut = true;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void OnEnable()
    {
        if (canvasGroup != null)
        {
            StopAllCoroutines();
            if (enableFadeIn)
            {
                canvasGroup.alpha = 0f;
                StartCoroutine(FadeIn());
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
    // 不要在 OnDisable 做动画，直接设为 0
    void OnDisable()
    {
        if (canvasGroup != null)
        {
            if (enableFadeOut)
                canvasGroup.alpha = 0f;
            else
                canvasGroup.alpha = 1f;
        }
    }

    // 提供外部调用的隐藏方法，带渐隐动画
    public void HideAndDisable()
    {
        SLog.Info("Hiding canvas with fade out");
        StopAllCoroutines();
        if (enableFadeOut)
            StartCoroutine(FadeOutAndDisable());
        else
            gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator FadeOutAndDisable()
    {
        if (canvasGroup != null)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(startAlpha * (1f - (elapsed / fadeDuration)));
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
        gameObject.SetActive(false);
    }
}
