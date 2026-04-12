using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 背包管理器 — 增删查，触发 UI 刷新事件
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public List<Item> Items { get; private set; } = new();
    public int MaxSlots = 30;

    public UnityEvent OnInventoryChanged;

    public bool AddItem(ItemDefinition def, int qty = 1)
    {
        // 先尝试叠加
        var existing = Items.Find(i => i.Data.Id == def.Id);
        if (existing != null && existing.Quantity < def.MaxStack)
        {
            int canAdd = def.MaxStack - existing.Quantity;
            existing.Quantity += Mathf.Min(qty, canAdd);
            OnInventoryChanged?.Invoke();
            return true;
        }
        // 新格子
        if (Items.Count >= MaxSlots) { Debug.Log("[Inventory] 背包已满！"); return false; }
        Items.Add(new Item(def, qty));
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(string id, int qty = 1)
    {
        var item = Items.Find(i => i.Data.Id == id);
        if (item == null) return false;
        item.Quantity -= qty;
        if (item.Quantity <= 0) Items.Remove(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(string id, int qty = 1)
    {
        var item = Items.Find(i => i.Data.Id == id);
        return item != null && item.Quantity >= qty;
    }

    public Item GetItem(string id) => Items.Find(i => i.Data.Id == id);
}
