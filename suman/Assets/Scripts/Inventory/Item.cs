using UnityEngine;

/// <summary>
/// 物品定义（ScriptableObject）
/// 右键 → Create → Suman → Item 创建具体物品资产
/// </summary>
[CreateAssetMenu(menuName = "Suman/Item", fileName = "NewItem")]
public class ItemDefinition : ScriptableObject
{
    public string Id;
    public string ItemName;
    [TextArea] public string Description;
    public Sprite Icon;
    public ItemType Type;
    public int MaxStack = 99;

    [Header("效果（仅消耗品）")]
    public int HealAmount;
    public int ManaAmount;
}

public enum ItemType { Consumable, Equipment, KeyItem, Material }

// ── 背包里的实例 ─────────────────────────────────────────────
[System.Serializable]
public class Item
{
    public ItemDefinition Data;
    public int Quantity;

    public Item(ItemDefinition def, int qty = 1) { Data = def; Quantity = qty; }

    public void Use(PlayerStats target)
    {
        if (Data.Type != ItemType.Consumable) return;
        target.Heal(Data.HealAmount);
        target.RestoreMana(Data.ManaAmount);
        Debug.Log($"[Item] 使用 {Data.ItemName}（回复 HP:{Data.HealAmount} MP:{Data.ManaAmount}）");
    }
}
