using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    [Header("玩家出生点")]
    public Transform spawnPoint;
    [Header("玩家预制体")]
    public GameObject playerPrefab;
    [Header("玩家摄像机")]
    public CinemachineCamera playerCamera;
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
        SEvent.Instance.AddListener(EventName.EnablePlayer, EnablePlayer);
    }
    void OnDisable()
    {
        SEvent.Instance.RemoveListener(EventName.SpawnPlayer, SpawnPlayer);
        SEvent.Instance.RemoveListener(EventName.EnablePlayer, EnablePlayer);
    }
    public void SpawnPlayer()
    {
        SLog.Hello();
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

    public void EnablePlayer()
    {
        // 设置玩家相关属性
        playerInstance.gameObject.GetComponent<Rigidbody>().useGravity = true;
        if (playerCamera != null)
        {
            playerCamera.Follow = playerInstance.transform;
            // 查找Player对象下名为"LookAt"的子对象
            Transform lookAtTarget = playerInstance.transform.Find("LookAt");
            if (lookAtTarget != null)
            {
                playerCamera.LookAt = lookAtTarget;
            }
            else
            {
                playerCamera.LookAt = playerInstance.transform;
            }
        }
    }
}
