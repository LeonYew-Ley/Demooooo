using UnityEngine;

public class DeadDetection : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SEvent.Instance.TriggerEvent(EventName.AllPlayerDead);
        }
    }
}