using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 剧情管理器 — 追踪剧情进度、主线任务状态
/// </summary>
public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    [System.Serializable]
    public class Quest
    {
        public string Id;
        public string Title;
        [TextArea] public string Description;
        public QuestState State = QuestState.Inactive;
        public List<QuestObjective> Objectives = new();
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string Description;
        public bool   Completed;
    }

    public enum QuestState { Inactive, Active, Completed, Failed }

    public List<Quest> AllQuests = new();
    public UnityEvent<Quest> OnQuestStarted;
    public UnityEvent<Quest> OnQuestCompleted;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartQuest(string questId)
    {
        var q = AllQuests.Find(x => x.Id == questId);
        if (q == null || q.State != QuestState.Inactive) return;
        q.State = QuestState.Active;
        Debug.Log($"[Story] 任务开始：{q.Title}");
        OnQuestStarted?.Invoke(q);
    }

    public void CompleteObjective(string questId, int objectiveIndex)
    {
        var q = AllQuests.Find(x => x.Id == questId);
        if (q == null || q.State != QuestState.Active) return;
        if (objectiveIndex < q.Objectives.Count)
            q.Objectives[objectiveIndex].Completed = true;

        if (q.Objectives.TrueForAll(o => o.Completed))
        {
            q.State = QuestState.Completed;
            Debug.Log($"[Story] 任务完成：{q.Title}");
            OnQuestCompleted?.Invoke(q);
        }
    }

    public QuestState GetQuestState(string questId)
    {
        var q = AllQuests.Find(x => x.Id == questId);
        return q?.State ?? QuestState.Inactive;
    }
}
