using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器，支持过渡动画和传送点
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("过渡")]
    [SerializeField] Animator transitionAnimator;
    [SerializeField] float transitionDuration = 0.5f;

    static string _targetSpawnId;

    public void LoadScene(string sceneName, string spawnId = "default")
    {
        _targetSpawnId = spawnId;
        StartCoroutine(Transition(sceneName));
    }

    IEnumerator Transition(string sceneName)
    {
        transitionAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(sceneName);
    }

    // 在目标场景的 Start() 里调用，定位玩家
    public static string GetTargetSpawnId() => _targetSpawnId ?? "default";
}
