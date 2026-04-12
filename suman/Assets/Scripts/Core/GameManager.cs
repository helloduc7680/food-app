using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏全局管理器 — 单例，跨场景持久存在
/// 负责：游戏状态机、系统初始化、存档协调
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Explore, Battle, Dialogue, Pause, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Explore;

    [Header("子系统引用")]
    public InventoryManager Inventory;
    public DialogueManager  Dialogue;
    public CombatManager    Combat;
    public UIManager        UI;
    public SceneLoader      SceneLoader;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitSystems();
    }

    void InitSystems()
    {
        Inventory   ??= FindObjectOfType<InventoryManager>();
        Dialogue    ??= FindObjectOfType<DialogueManager>();
        Combat      ??= FindObjectOfType<CombatManager>();
        UI          ??= FindObjectOfType<UIManager>();
        SceneLoader ??= FindObjectOfType<SceneLoader>();
    }

    // ── 状态切换 ─────────────────────────────────────────────
    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Time.timeScale = (newState == GameState.Pause) ? 0f : 1f;
        Debug.Log($"[GameManager] 状态切换 → {newState}");
    }

    public bool IsExploring  => CurrentState == GameState.Explore;
    public bool IsInBattle   => CurrentState == GameState.Battle;
    public bool IsInDialogue => CurrentState == GameState.Dialogue;
}
