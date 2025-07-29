using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("玩家出生点")]
    public Transform spawnPoint;
    [Header("玩家预制体")]
    public GameObject playerPrefab;
    [HideInInspector]
    public GameObject playerInstance;

    void OnEnable()
    {
        SEvent.Instance.AddListener(EventName.SpawnPlayer, SpawnPlayer);
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.SpawnPlayer, SpawnPlayer);
    }
    public void SpawnPlayer()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
        if (playerPrefab != null && spawnPoint != null)
        {
            Bounds bounds = playerPrefab.GetComponent<Renderer>().bounds;
            Vector3 offset = new Vector3(0, bounds.min.y, 0);
            playerInstance = Instantiate(playerPrefab, spawnPoint.position - offset, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("PlayerManager: 请设置玩家预制体和出生点");
        }
    }
}
