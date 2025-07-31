using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
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
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    void OnEnable()
    {
        playerInputManager.onPlayerJoined += SpawnPlayers;
        // SEvent.Instance.AddListener(EventName.SpawnPlayer, SpawnPlayer);
        SEvent.Instance.AddListener(EventName.EnablePlayer, EnablePlayer);
    }
    void OnDisable()
    {
        playerInputManager.onPlayerJoined -= SpawnPlayers;
        // SEvent.Instance.RemoveListener(EventName.SpawnPlayer, SpawnPlayer);
        SEvent.Instance.RemoveListener(EventName.EnablePlayer, EnablePlayer);
    }
    public void SpawnPlayers(PlayerInput playerInput)
    {
        SLog.Info($"Spawn Player: {playerInput.playerIndex}");
        playerInputs.Add(playerInput);

        // 设置出生点坐标
        Transform playerControllerObj = playerInput.transform; // PlayerController
        playerControllerObj.position = spawnPoints[playerInputs.Count - 1].position;

        // 设置摄像机层级
        CinemachineCamera cinemachineCamera = playerControllerObj.GetComponentInChildren<CinemachineCamera>();
        int layerToAdd = (int)Mathf.Log(playerLayers[playerInputs.Count - 1], 2);
        cinemachineCamera.gameObject.layer = layerToAdd;

        // 设置Cinemachine Chanel
        cinemachineCamera.OutputChannel = (OutputChannels)System.Enum.Parse(typeof(OutputChannels), $"Channel0{playerInput.playerIndex + 1}");
        playerControllerObj.GetComponentInChildren<CinemachineBrain>().ChannelMask = cinemachineCamera.OutputChannel;

        // BUG:没生效，CullingMask没变化
        // playerControllerObj.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
        // playerControllerObj.GetComponentInChildren<InputHandler>().horizontal = playerInput.actions.FindAction("Look");
    }

    public void EnablePlayer()
    {
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
}
