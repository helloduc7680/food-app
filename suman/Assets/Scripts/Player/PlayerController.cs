using UnityEngine;

/// <summary>
/// 玩家移动控制（探索模式）
/// 使用 Unity 新 Input System 或传统 Input 均可替换
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    [SerializeField] float moveSpeed = 4f;

    [Header("交互")]
    [SerializeField] float interactRadius = 0.8f;
    [SerializeField] LayerMask interactLayer;

    public PlayerStats Stats = new PlayerStats();

    Rigidbody2D _rb;
    Vector2 _input;
    Animator _animator;

    static readonly int AnimH = Animator.StringToHash("Horizontal");
    static readonly int AnimV = Animator.StringToHash("Vertical");
    static readonly int AnimSpeed = Animator.StringToHash("Speed");

    void Awake()
    {
        _rb       = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!GameManager.Instance.IsExploring) { _input = Vector2.zero; return; }

        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (_animator != null)
        {
            _animator.SetFloat(AnimH, _input.x);
            _animator.SetFloat(AnimV, _input.y);
            _animator.SetFloat(AnimSpeed, _input.magnitude);
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            TryInteract();
    }

    void FixedUpdate() => _rb.linearVelocity = _input * moveSpeed;

    void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRadius, interactLayer);
        hit?.GetComponent<IInteractable>()?.Interact();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}

public interface IInteractable { void Interact(); }
