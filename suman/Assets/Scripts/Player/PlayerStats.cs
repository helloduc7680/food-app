using UnityEngine;

/// <summary>
/// 玩家属性数据（可序列化，方便存档）
/// </summary>
[System.Serializable]
public class PlayerStats
{
    public string CharacterName = "旅者";
    public int Level  = 1;
    public int Exp    = 0;
    public int ExpToNextLevel = 100;

    public int MaxHP  = 100;
    public int HP     = 100;
    public int MaxMP  = 50;
    public int MP     = 50;

    public int Attack  = 10;
    public int Defense = 5;
    public int Speed   = 8;
    public int Magic   = 6;

    public int Gold = 0;

    public bool IsAlive => HP > 0;

    public void TakeDamage(int dmg)
    {
        int actual = Mathf.Max(dmg - Defense, 1);
        HP = Mathf.Max(HP - actual, 0);
    }

    public void Heal(int amount) => HP = Mathf.Min(HP + amount, MaxHP);
    public void UseMana(int amount) => MP = Mathf.Max(MP - amount, 0);
    public void RestoreMana(int amount) => MP = Mathf.Min(MP + amount, MaxMP);

    public bool GainExp(int amount)
    {
        Exp += amount;
        if (Exp >= ExpToNextLevel) { LevelUp(); return true; }
        return false;
    }

    void LevelUp()
    {
        Level++;
        Exp -= ExpToNextLevel;
        ExpToNextLevel = Mathf.RoundToInt(ExpToNextLevel * 1.4f);
        MaxHP  += 20; HP  = MaxHP;
        MaxMP  += 10; MP  = MaxMP;
        Attack  += 3;
        Defense += 2;
        Speed   += 1;
        Magic   += 2;
        Debug.Log($"[PlayerStats] 升级！当前等级：{Level}");
    }
}
