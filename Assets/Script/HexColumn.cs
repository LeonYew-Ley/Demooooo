using UnityEngine;
using System.Collections;

public class HexColumn : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color triggeredColor = new Color(0.6f, 0f, 1f, 1f); // 紫色
    public float disappearDelay = 2f;
    private bool triggered = false;
    void OnEnable()
    {
    }
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

    IEnumerator DisappearAfterDelay()
    {
        Debug.Log("Column triggered, will disappear after delay.");
        yield return new WaitForSeconds(disappearDelay);
        Destroy(gameObject);
    }


}
