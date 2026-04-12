using System.IO;
using UnityEngine;

/// <summary>
/// 简易存档系统（JSON 文件）
/// 调用 SaveSystem.Save() / SaveSystem.Load()
/// </summary>
public static class SaveSystem
{
    static string SavePath => Path.Combine(Application.persistentDataPath, "suman_save.json");

    [System.Serializable]
    class SaveData
    {
        public PlayerStats Player;
        public string CurrentScene;
        public string SpawnId;
        // 扩展：背包、任务标记等
    }

    public static void Save()
    {
        var player = Object.FindObjectOfType<PlayerController>();
        if (player == null) return;

        var data = new SaveData
        {
            Player       = player.Stats,
            CurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            SpawnId      = "default"
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, prettyPrint: true));
        Debug.Log($"[Save] 存档成功 → {SavePath}");
    }

    public static bool Load()
    {
        if (!File.Exists(SavePath)) return false;
        var data   = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        var player = Object.FindObjectOfType<PlayerController>();
        if (player != null) player.Stats = data.Player;
        Debug.Log("[Save] 读档成功");
        return true;
    }

    public static bool HasSave() => File.Exists(SavePath);
    public static void DeleteSave() => File.Delete(SavePath);
}
