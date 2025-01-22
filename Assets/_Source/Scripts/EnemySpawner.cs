using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int _maxEnemyCount = 5;
    [SerializeField] private EnemyBase _enemy;
    [SerializeField] private Vector3 _spawnPosition;

    private readonly List<PoolMember> UsedEnemy = new();
    private readonly List<SpawnPoint> SpawnPoints = new();
    private readonly WaitForSeconds Interval = new(5f);

    private Coroutine _coroutine;
    private Pool _enemyPool;

    private bool _isDangerousTime;
    public bool IsDangerousTime => _isDangerousTime;

    private void Start()
    {
        _enemyPool = new(_enemy);

        foreach(SpawnPoint point in GetComponentsInChildren<SpawnPoint>())
        {
            point.OnUsedPoint += Point_OnUsedPoint;
            if (!point.IsUsed) SpawnPoints.Add(point);
        }

        Game.Action.OnStart += Action_OnStart;
        Game.Action.OnLose += Release;
        Game.Locator.Timer.OnChangeState += Change;
        Game.Action.OnPause += Action_OnPause;
        Game.Action.OnExit += Action_Reset;
        Game.Action.OnRestart += Action_Reset;
    }

    private void Action_Reset()
    {
        for(int i = UsedEnemy.Count - 1; i >= 0; i--)      
            UsedEnemy[i].ReturnToPool();       
    }

    private void Action_OnPause(bool onPause)
    {
        Release();
        if(!onPause) _coroutine = StartCoroutine(SpawnProcess());
    }

    private void Point_OnUsedPoint(SpawnPoint point, bool active)
    {
        if (active) SpawnPoints.Remove(point);
        else SpawnPoints.Add(point);
    }

    private void Action_OnStart()
    {
        Release();
        _isDangerousTime = false;
        _coroutine = StartCoroutine(SpawnProcess());
    }

    private IEnumerator SpawnProcess()
    {
        while(true)
        {
            yield return new WaitWhile(() => UsedEnemy.Count >= _maxEnemyCount);
            yield return Interval;
            Spawn();
        }
    }

    private void Spawn()
    {
        var enemy = _enemyPool.Spawn(_spawnPosition);

        UsedEnemy.Add(enemy);
        enemy.Die += Enemy_Die;
    }

    private void Change(bool value)
    {
        _isDangerousTime = value;

        if (!_isDangerousTime) return;

        foreach (EnemyBase enemy in UsedEnemy)
            enemy.Change();
    }

    private void Enemy_Die(PoolMember enemy)
    {
        UsedEnemy.Remove(enemy);
        enemy.Die -= Enemy_Die;
    }

    private void Release()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public Vector3 GetPath()
    {
        int r = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[r].transform.position;
    }
}