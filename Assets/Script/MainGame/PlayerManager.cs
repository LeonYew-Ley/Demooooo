using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{
    // 单例模式
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<PlayerManager>();
                    singletonObject.name = typeof(PlayerManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    [Header("平台对象")]
    public Transform platformObj; // 平台对象
    [Header("最大玩家数")]
    public int maxPlayers = 1; // 最大玩家数
    [Header("玩家输入实例")]
    public List<PlayerInput> playerInputs = new List<PlayerInput>(); // 统计PlayerInput实例
    [Header("玩家出生点列表")]
    public Transform[] spawnPoints;
    [Header("玩家层级（同步摄像机层级）")]
    public List<LayerMask> playerLayers;

    // 私有变量
    private PlayerInputManager playerInputManager; // 多人分屏管理组件

    void Awake()
    {
        // 确保只有一个实例存在
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        playerInputManager = GetComponent<PlayerInputManager>();
    }

    void OnEnable()
    {
        playerInputManager.onPlayerJoined += SpawnPlayers;
        SEvent.Instance.AddListener(EventName.GameStart, EnablePlayer);
        SEvent.Instance.AddListener(EventName.AllPlayerDead, ClearPlayers);
        SEvent.Instance.AddListener(EventName.OnRotation, DisablePlayers);
        SEvent.Instance.AddListener(EventName.EndRotation, GlidingPlayers);

    }
    void OnDisable()
    {
        playerInputManager.onPlayerJoined -= SpawnPlayers;
        SEvent.Instance.RemoveListener(EventName.GameStart, EnablePlayer);
        SEvent.Instance.RemoveListener(EventName.AllPlayerDead, ClearPlayers);
        SEvent.Instance.RemoveListener(EventName.OnRotation, DisablePlayers);
        SEvent.Instance.RemoveListener(EventName.EndRotation, GlidingPlayers);
    }
    public void SpawnPlayers(PlayerInput playerInput)
    {
        if (playerInput.playerIndex == 0)
            this.TriggerEvent(EventName.ToggleMainCamera);
        SLog.Info($"Spawn Player: {playerInput.playerIndex}");
        playerInputs.Add(playerInput);

        // 设置出生点坐标
        Transform playerControllerObj = playerInput.transform; // PlayerController
        playerControllerObj.position = spawnPoints[playerInputs.Count - 1].position;
        if (platformObj != null)
            playerControllerObj.SetParent(platformObj);// 将playerControllerObj放在Platform对象下

        // 设置摄像机层级
        CinemachineCamera cinemachineCamera = playerControllerObj.GetComponentInChildren<CinemachineCamera>();
        int layerToAdd = (int)Mathf.Log(playerLayers[playerInputs.Count - 1], 2);
        cinemachineCamera.gameObject.layer = layerToAdd;

        // 设置Cinemachine Chanel
        cinemachineCamera.OutputChannel = (OutputChannels)System.Enum.Parse(typeof(OutputChannels), $"Channel0{playerInput.playerIndex + 1}");
        playerControllerObj.GetComponentInChildren<CinemachineBrain>().ChannelMask = cinemachineCamera.OutputChannel;

        // 更新ReadyToStartCanvas的玩家数量
        this.TriggerEvent(EventName.UpdateReadyToStartCanvas, playerInputs.Count, maxPlayers);

        // 如果达到最大玩家数，开始游戏并禁用PlayerInputManager
        if (playerInputs.Count == maxPlayers)
        {
            SLog.Info("玩家数量已达上限，开始游戏");
            Invoke(nameof(UpdateReadyToStartCanvasDelayed), 0.5f);
            Invoke(nameof(TriggerGameStart), 1.5f);

        }

    }
    public void EnablePlayer()
    {
        SLog.Info("Enabling all playersInputs.");
        // 遍历所有PlayerInput，启用其子物体中的Rigidbody的重力
        foreach (var playerInput in playerInputs)
        {
            Rigidbody rb = playerInput.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }
        }
    }
    public void DisablePlayers()
    {
        SLog.Info("Disabling all playersInputs.");
        // 遍历所有PlayerInput，启用其子物体中的Rigidbody的重力
        foreach (var playerInput in playerInputs)
        {
            Rigidbody rb = playerInput.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
            }
        }
    }

    public void GlidingPlayers()
    {
        SLog.Info("Gliding all playersInputs.");
        EnablePlayer(); // 重新启用玩家输入
    }


    public void ClearPlayers()
    {
        SLog.Info("Clearing all playersInputs.");
        playerInputs.Clear();
    }


    void UpdateReadyToStartCanvasDelayed()
    {
        this.TriggerEvent(EventName.UpdateReadyToStartCanvas, -1, maxPlayers);//更新玩家数量为-1，表示已满
    }
    void TriggerGameStart()
    {
        SEvent.Instance.TriggerEvent(EventName.GameCountDown); // 触发倒计时，开始游戏
    }
    public int GetMaxPlayer()
    {
        return maxPlayers;
    }
}
