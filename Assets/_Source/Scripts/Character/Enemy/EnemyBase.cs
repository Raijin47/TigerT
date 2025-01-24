using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : PoolMember, IDamageable
{
    public event Action<int> OnTakeDamage;

    #region Component
    [SerializeField] private GameObject[] _skins;
    [SerializeField] private Animator[] _animators;
    [SerializeField] private ParticleSystem _particle;

    private IState _currentState;
    private Health _health;
    private NavMeshAgent _agent;
    private Transform _transform;
    private Coroutine _coroutine;
    private EnemySearch _enemySearch;

    private int _mode;
    public int Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            bool isPig = value == 0;
            _health.MaxHealh = isPig ? 1 : 30;
            _skins[0].SetActive(isPig);
            _skins[1].SetActive(!isPig);
        }
    }

    private bool _isActive;
    private bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Agent.isStopped = !value;
            Animator.SetBool("IsDeath", !value);
            Release();
        }
    }

    public Animator Animator => _animators[Mode];
    public NavMeshAgent Agent => _agent;
    public Transform Transform => _transform;
    #endregion

    #region State
    private StateIdle _stateIdle;
    private StatePatrol _statePatrol;
    private StatePanic _statePanic;
    private StatePursuit _statePursuit;
    private StateAttack _stateAttack;

    public StateAttack StateAttack => _stateAttack;
    public StatePursuit StatePursuit => _statePursuit;
    public StatePatrol StatePatrol => _statePatrol;
    public StateIdle StateIdle => _stateIdle;

    #endregion
    public override void Init()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;

        _transform = transform;
        _enemySearch = GetComponent<EnemySearch>();

        _stateIdle = new(this);
        _statePatrol = new(this);
        _statePanic = new(this);
        _statePursuit = new(this);
        _stateAttack = new(this);
        _health = new(this);

        Resurrect();
    }

    public override void Resurrect()
    {
        IsActive = true;
        Mode = Game.Locator.Spawner.IsDangerousTime ? 1 : 0;
        _statePursuit.OnCanAttack += ChangeState;
        _stateAttack.OnCannotAttack += ChangeState;
        _stateIdle.OnEndIdle += ChangeState;
        _statePatrol.OnEndPatrol += ChangeState;
        _enemySearch.OnPlayerFound += Action_OnPlayerSearch;
        _health.OnDie += OnDie;
        _health.AddListaner();
        Game.Action.OnLose += Action_OnLose;
        ChangeState(_statePatrol);
    }

    private void Action_OnLose()
    {
        _isActive = false;
        Agent.isStopped = true;
        Animator.SetFloat("Velocity", 0);
        Release();
    }

    private void OnDie()
    {
        if (!IsActive) return;
        IsActive = false;

        bool isPig = Mode == 0;

        Game.Locator.Karma.Karma += isPig ? -2 : 2;
        if (isPig)
        {
            Game.Locator.Stats.PigKilled++;
            Game.Audio.PlayClip(3);
        }

        else
        {
            Game.Locator.Stats.OrcKilled++;
            Game.Audio.PlayClip(4);
        }

        _statePursuit.OnCanAttack -= ChangeState;
        _stateAttack.OnCannotAttack -= ChangeState;
        _stateIdle.OnEndIdle -= ChangeState;
        _statePatrol.OnEndPatrol -= ChangeState;
        _enemySearch.OnPlayerFound -= Action_OnPlayerSearch;
        _health.OnDie -= OnDie;
        _health.RemoveListaner();
        Game.Action.OnLose -= Action_OnLose;

        Invoke(nameof(SpawnLoot), 3f);
    }

    private void SpawnLoot()
    {
        ReturnToPool();
    }

    public void Change()
    {
        if (!IsActive && Mode == 1) return;
        Mode = 1;
        _particle.Play();
        
        ChangeState(_enemySearch.IsViewPlayer ? _statePursuit : _stateIdle);
    }

    public override void Release()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void ApplyDamage(int value)
    {
        if (!IsActive) return;
        OnTakeDamage?.Invoke(value);
    }

    private void Action_OnPlayerSearch(bool value)
    {
        if (!IsActive) return;
        ChangeState(value ? (Mode == 0 ? _statePanic : _statePursuit) : _stateIdle);
    }

    private void ChangeState(IState state)
    {
        if (!IsActive) return;
        Release();

        _currentState = state;
        state.Enter();
        _coroutine = StartCoroutine(state.UpdateProcess());
    }
}