using UnityEngine;
using System.Collections;

public class HexColumn : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color triggeredColor = new Color(0.6f, 0f, 1f, 1f); // 紫色
    public float disappearDelay = 2f;
    private bool triggered = false;
    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!triggered && collision.gameObject.CompareTag("Player"))
        {
            triggered = true;
            if (rend != null)
            {
                rend.material.color = triggeredColor;
            }
            StartCoroutine(DisappearAfterDelay());
        }
    }

    private bool isDisappearing = false;

    IEnumerator DisappearAfterDelay()
    {
        // 如果已经在消失中，则不再执行，防止重置冷却时间
        if (isDisappearing) yield break;

        isDisappearing = true;
        yield return new WaitForSeconds(disappearDelay);
        Destroy(gameObject);
    }


}
