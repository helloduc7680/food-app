using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 对话管理器 — 驱动对话 UI，处理分支与剧情标记
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] UnityEngine.UI.Image portraitImage;
    [SerializeField] GameObject choiceContainer;
    [SerializeField] GameObject choiceButtonPrefab;

    [Header("打字速度（秒/字）")]
    [SerializeField] float typeSpeed = 0.03f;

    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    DialogueData _data;
    int _lineIndex;
    bool _isTyping;
    Coroutine _typeCoroutine;

    // 剧情标记（存档时持久化）
    readonly HashSet<string> _storyFlags = new();
    public bool HasFlag(string flag) => _storyFlags.Contains(flag);

    public void StartDialogue(DialogueData data)
    {
        _data = data;
        _lineIndex = 0;
        dialoguePanel.SetActive(true);
        GameManager.Instance.SetState(GameManager.GameState.Dialogue);
        OnDialogueStart?.Invoke();
        ShowLine();
    }

    public void Advance()
    {
        if (_isTyping) { SkipTyping(); return; }

        var line = _data.Lines[_lineIndex];
        if (line.Choices.Count > 0) return; // 等玩家选择

        _lineIndex++;
        if (_lineIndex >= _data.Lines.Count) EndDialogue();
        else ShowLine();
    }

    void ShowLine()
    {
        var line = _data.Lines[_lineIndex];
        nameText.text = line.SpeakerName;
        if (portraitImage != null)
        {
            portraitImage.sprite  = line.SpeakerPortrait;
            portraitImage.enabled = line.SpeakerPortrait != null;
        }
        ClearChoices();

        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        _typeCoroutine = StartCoroutine(TypeText(line));
    }

    IEnumerator TypeText(DialogueLine line)
    {
        _isTyping = true;
        bodyText.text = "";
        if (line.VoiceClip != null)
            AudioSource.PlayClipAtPoint(line.VoiceClip, Camera.main.transform.position);

        foreach (char c in line.Text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        _isTyping = false;

        if (line.Choices.Count > 0) ShowChoices(line.Choices);
    }

    void SkipTyping()
    {
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        bodyText.text = _data.Lines[_lineIndex].Text;
        _isTyping = false;
        var line = _data.Lines[_lineIndex];
        if (line.Choices.Count > 0) ShowChoices(line.Choices);
    }

    void ShowChoices(List<DialogueChoice> choices)
    {
        choiceContainer.SetActive(true);
        foreach (var choice in choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choiceContainer.transform);
            btn.GetComponentInChildren<TMP_Text>().text = choice.ChoiceText;
            var c = choice; // 闭包捕获
            btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectChoice(c));
        }
    }

    void SelectChoice(DialogueChoice choice)
    {
        if (!string.IsNullOrEmpty(choice.StoryFlag))
            _storyFlags.Add(choice.StoryFlag);

        ClearChoices();
        if (choice.NextDialogue != null) StartDialogue(choice.NextDialogue);
        else EndDialogue();
    }

    void ClearChoices()
    {
        choiceContainer.SetActive(false);
        foreach (Transform child in choiceContainer.transform)
            Destroy(child.gameObject);
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        GameManager.Instance.SetState(GameManager.GameState.Explore);
        OnDialogueEnd?.Invoke();
    }

    void Update()
    {
        if (GameManager.Instance.IsInDialogue &&
            (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)))
            Advance();
    }
}
