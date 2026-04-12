using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话数据资产（ScriptableObject）
/// 在 Project 窗口右键 → Create → Suman → Dialogue 创建
/// </summary>
[CreateAssetMenu(menuName = "Suman/Dialogue", fileName = "NewDialogue")]
public class DialogueData : ScriptableObject
{
    public string DialogueId;
    public List<DialogueLine> Lines = new();
}

[System.Serializable]
public class DialogueLine
{
    public string SpeakerName;
    [TextArea(2, 5)]
    public string Text;
    public Sprite SpeakerPortrait;
    public AudioClip VoiceClip;         // 可选配音
    public List<DialogueChoice> Choices = new(); // 空 = 无选项，继续下一行
}

[System.Serializable]
public class DialogueChoice
{
    public string ChoiceText;
    public DialogueData NextDialogue;   // null = 结束对话
    [Tooltip("执行该选项时触发的剧情标记")]
    public string StoryFlag;
}
