/// <summary>
/// 事件名常量类,String类型
/// 用于定义和管理事件名称
/// </summary>
public static class EventName
{
    // 其他事件
    public const string ToggleMainCamera = nameof(ToggleMainCamera);

    // 平台翻转相关事件
    public const string FlipPlatform = nameof(FlipPlatform);
    public const string OnRotation = nameof(OnRotation);
    public const string EndRotation = nameof(EndRotation);
    // 游戏流程相关事件
    public const string GenerateHexPlatform = nameof(GenerateHexPlatform);
    public const string SpawnPlayer = nameof(SpawnPlayer);
    public const string GameStart = nameof(GameStart);
    public const string AllPlayerDead = nameof(AllPlayerDead);
    public const string GameEnter = nameof(GameEnter);
    public const string GameCountDown = nameof(GameCountDown);
    public const string GameRestart = nameof(GameRestart);
    public const string DestroyPlatform = nameof(DestroyPlatform);


    // Canvas 开闭
    public const string ShowGameOver = nameof(ShowGameOver);
    public const string HideGameOver = nameof(HideGameOver);
    public const string ShowHomeCanvas = nameof(ShowHomeCanvas);
    public const string HideHomeCanvas = nameof(HideHomeCanvas);
    public const string ShowCountDownCanvas = nameof(ShowCountDownCanvas);
    public const string HideCountDownCanvas = nameof(HideCountDownCanvas);
    public const string ShowReadyToStartCanvas = nameof(ShowReadyToStartCanvas);
    public const string HideReadyToStartCanvas = nameof(HideReadyToStartCanvas);
    public const string UpdateReadyToStartCanvas = nameof(UpdateReadyToStartCanvas);

}
