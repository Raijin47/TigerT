using DG.Tweening;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Joystick _rotateJoystick;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private Transform _transform;
    [SerializeField] private Vector3 _attackArea;
    [SerializeField] private float _rayDistance;

    private CharacterAnimation _animator;
    private Sequence _sequence;
    private LayerMask _layer;

    private bool _isPause;
    private bool _isAttack;

    private float _horizontal;
    private float _vertical;
    private float _moveAmount;

    private bool IsActive
    {
        get => _isPause;
        set
        {
            _isPause = value;
            _movement.MovementDirection = Vector3.zero;
        }
    }

    private void Awake()
    {
        _movement.Init(GetComponent<Rigidbody>());
        _animator = new(GetComponentInChildren<Animator>());
        _layer = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        _isPause = true;
        Game.Action.OnStart += () => IsActive = false;
        Game.Action.OnLose += () => IsActive = true;
        Game.Action.OnRestart += Action_Reset;
        Game.Action.OnExit += Action_Reset;
    }

    private void Action_Reset() => transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
    private void Update()
    {
        if (IsActive || _isAttack) return;
        GetInput();
    }
    private void FixedUpdate()
    {
        if (IsActive || _isAttack) return;
        Movement();
    }

    private void GetInput()
    {
        _horizontal = _rotateJoystick.Horizontal;
        _vertical = _joystick.Vertical;
    }

    public void Attack()
    {
        if (_isAttack || IsActive) return;
        _isAttack = true;
        _animator.Attack();
        _particle.Play();

        _sequence?.Kill();

        _sequence = DOTween.Sequence();

        _sequence.Append(_transform.DOLocalMoveZ(1, 0.5f).OnComplete(CheckEnemy)).
            Append(_transform.DOLocalMoveZ(0, .5f))
            .OnComplete(() => 
            {
                _isAttack = false;
                _particle.Stop();
            });
    }

    private void CheckEnemy()
    {
        var enemies = Physics.BoxCastAll(transform.position, _attackArea, Vector3.forward, transform.rotation, _rayDistance, _layer);

        foreach (RaycastHit enemy in enemies)   
            if (enemy.collider.TryGetComponent(out EnemyBase e))
                e.ApplyDamage(Mathf.RoundToInt(10));      
    }

    private void Movement()
    {
        _movement.MovementDirection = _vertical * transform.forward;
        _movement.RotateDirection = _horizontal * transform.right;

        _moveAmount = Mathf.Clamp01(Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));

        _movement.Move(_moveAmount);
        _movement.Rotate();
        _animator.MovementAnimations(_moveAmount);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new(1, 0, 0, 0.4f);
        Gizmos.DrawCube(transform.position + transform.forward * _rayDistance, _attackArea);
    }
}