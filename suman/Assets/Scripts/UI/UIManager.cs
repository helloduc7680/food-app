using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI 总管理器 — HUD 刷新、面板开关
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] Slider hpBar;
    [SerializeField] Slider mpBar;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text mpText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text goldText;

    [Header("面板")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject questPanel;
    [SerializeField] GameObject pausePanel;

    PlayerStats _stats;

    void Start()
    {
        _stats = FindObjectOfType<PlayerController>()?.Stats;
        if (_stats != null) RefreshHUD();

        GameManager.Instance.Inventory.OnInventoryChanged.AddListener(RefreshHUD);
    }

    public void RefreshHUD()
    {
        if (_stats == null) return;
        if (hpBar)   hpBar.value  = (float)_stats.HP  / _stats.MaxHP;
        if (mpBar)   mpBar.value  = (float)_stats.MP  / _stats.MaxMP;
        if (hpText)  hpText.text  = $"{_stats.HP}/{_stats.MaxHP}";
        if (mpText)  mpText.text  = $"{_stats.MP}/{_stats.MaxMP}";
        if (levelText) levelText.text = $"Lv.{_stats.Level}";
        if (goldText)  goldText.text  = $"{_stats.Gold} G";
    }

    // ── 面板切换 ────────────────────────────────────────────
    public void ToggleInventory() => inventoryPanel?.SetActive(!inventoryPanel.activeSelf);
    public void ToggleQuest()     => questPanel?.SetActive(!questPanel.activeSelf);

    public void TogglePause()
    {
        bool open = !pausePanel.activeSelf;
        pausePanel?.SetActive(open);
        GameManager.Instance.SetState(open ? GameManager.GameState.Pause : GameManager.GameState.Explore);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();
        if (Input.GetKeyDown(KeyCode.Q)) ToggleQuest();
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }
}
