using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    [Header("玩家出生点")]
    public Transform spawnPoint;
    [Header("玩家预制体")]
    public GameObject playerPrefab;
    [Header("玩家实例父物体")]
    public Transform playerParent;
    [HideInInspector]
    public GameObject playerInstance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 可选：如果你希望该对象在场景切换时不被销毁，取消注释下一行
        // DontDestroyOnLoad(gameObject);
    }

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
            Bounds bounds = playerPrefab.GetComponent<Renderer>() ? playerPrefab.GetComponent<Renderer>().bounds : new Bounds();
            Vector3 offset = new Vector3(0, bounds.min.y, 0);
            if (playerParent != null)
                playerInstance = Instantiate(playerPrefab, spawnPoint.position - offset, spawnPoint.rotation, playerParent);
            else
                playerInstance = Instantiate(playerPrefab, spawnPoint.position - offset, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("PlayerManager: 请设置玩家预制体和出生点");
        }
    }
}
