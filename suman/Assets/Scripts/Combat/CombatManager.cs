using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 回合制战斗管理器
/// 流程：玩家行动 → 敌人行动 → 检测结束条件 → 循环
/// </summary>
public class CombatManager : MonoBehaviour
{
    public enum TurnState { PlayerTurn, EnemyTurn, Victory, Defeat }
    public TurnState CurrentTurn { get; private set; }

    [HideInInspector] public List<EnemyData> Enemies = new();

    public UnityEvent<TurnState> OnTurnChanged;
    public UnityEvent OnCombatStart;
    public UnityEvent<bool> OnCombatEnd; // true = 胜利

    PlayerStats _player;
    int _enemyIndex;

    public void StartCombat(List<EnemyData> enemies)
    {
        _player  = FindObjectOfType<PlayerController>().Stats;
        Enemies  = enemies;
        _enemyIndex = 0;
        GameManager.Instance.SetState(GameManager.GameState.Battle);
        OnCombatStart?.Invoke();
        SetTurn(TurnState.PlayerTurn);
    }

    void SetTurn(TurnState t)
    {
        CurrentTurn = t;
        OnTurnChanged?.Invoke(t);
    }

    // ── 玩家行动 ────────────────────────────────────────────
    public void PlayerAttack()
    {
        if (CurrentTurn != TurnState.PlayerTurn) return;
        var enemy = Enemies[_enemyIndex];
        int dmg = Mathf.Max(_player.Attack - enemy.Defense, 1);
        enemy.HP = Mathf.Max(enemy.HP - dmg, 0);
        Debug.Log($"[Combat] 玩家攻击 {enemy.Name}，造成 {dmg} 点伤害（剩余 HP: {enemy.HP}）");
        CheckEnemyDead();
    }

    public void PlayerUseSkill(SkillData skill)
    {
        if (CurrentTurn != TurnState.PlayerTurn) return;
        if (_player.MP < skill.ManaCost) { Debug.Log("[Combat] MP 不足！"); return; }
        _player.UseMana(skill.ManaCost);
        var enemy = Enemies[_enemyIndex];
        int dmg = Mathf.Max(_player.Magic * skill.PowerMultiplier - enemy.Defense, 1);
        enemy.HP = Mathf.Max(enemy.HP - (int)dmg, 0);
        Debug.Log($"[Combat] 使用技能 {skill.Name}，造成 {dmg} 点伤害");
        CheckEnemyDead();
    }

    public void PlayerUseItem(Item item)
    {
        if (CurrentTurn != TurnState.PlayerTurn) return;
        item.Use(_player);
        GameManager.Instance.Inventory.RemoveItem(item.Data.Id, 1);
        StartCoroutine(EnemyTurnRoutine());
    }

    public void PlayerFlee()
    {
        if (CurrentTurn != TurnState.PlayerTurn) return;
        bool success = Random.value > 0.4f;
        Debug.Log(success ? "[Combat] 成功逃跑！" : "[Combat] 逃跑失败！");
        if (success) EndCombat(false, fled: true);
        else StartCoroutine(EnemyTurnRoutine());
    }

    // ── 内部逻辑 ────────────────────────────────────────────
    void CheckEnemyDead()
    {
        if (Enemies[_enemyIndex].HP <= 0)
        {
            Debug.Log($"[Combat] {Enemies[_enemyIndex].Name} 被击败！");
            Enemies.RemoveAt(_enemyIndex);
            if (Enemies.Count == 0) { EndCombat(true); return; }
            _enemyIndex = Mathf.Clamp(_enemyIndex, 0, Enemies.Count - 1);
        }
        StartCoroutine(EnemyTurnRoutine());
    }

    IEnumerator EnemyTurnRoutine()
    {
        SetTurn(TurnState.EnemyTurn);
        yield return new WaitForSeconds(0.8f);

        foreach (var enemy in Enemies)
        {
            int dmg = Mathf.Max(enemy.Attack - _player.Defense, 1);
            _player.TakeDamage(enemy.Attack);
            Debug.Log($"[Combat] {enemy.Name} 攻击玩家，造成 {dmg} 点伤害（玩家 HP: {_player.HP}）");
            if (!_player.IsAlive) { EndCombat(false); yield break; }
            yield return new WaitForSeconds(0.5f);
        }

        SetTurn(TurnState.PlayerTurn);
    }

    void EndCombat(bool victory, bool fled = false)
    {
        if (victory)
        {
            SetTurn(TurnState.Victory);
            int totalExp  = 0, totalGold = 0;
            foreach (var e in Enemies) { totalExp += e.ExpReward; totalGold += e.GoldReward; }
            _player.GainExp(totalExp);
            _player.Gold += totalGold;
            Debug.Log($"[Combat] 战斗胜利！获得经验 {totalExp}，金币 {totalGold}");
        }
        else if (!fled) SetTurn(TurnState.Defeat);

        GameManager.Instance.SetState(GameManager.GameState.Explore);
        OnCombatEnd?.Invoke(victory);
    }
}

// ── 数据结构 ─────────────────────────────────────────────────
[System.Serializable]
public class EnemyData
{
    public string Name     = "史莱姆";
    public int HP          = 30;
    public int MaxHP       = 30;
    public int Attack      = 8;
    public int Defense     = 2;
    public int ExpReward   = 20;
    public int GoldReward  = 5;
    public Sprite Sprite;
}

[System.Serializable]
public class SkillData
{
    public string Name       = "火球术";
    public int    ManaCost   = 10;
    public float  PowerMultiplier = 2f;
    public string Description;
}
