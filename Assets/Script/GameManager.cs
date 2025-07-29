using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        // 初始化游戏管理器
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 这里可以添加游戏初始化逻辑
        SLog.Info("Game Initialized");
    }

}