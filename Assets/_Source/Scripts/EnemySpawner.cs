using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int _maxEnemyCount = 5;
    [SerializeField] private EnemyBase _enemy;
    [SerializeField] private Vector3 _spawnPosition;

    private readonly List<PoolMember> Pig = new();
    private readonly List<PoolMember> Orc = new();
    private readonly List<SpawnPoint> PathPoints = new();
    private readonly WaitForSeconds Interval = new(10f);
    private readonly WaitForSeconds Delay = new(5f);

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
            if (!point.IsUsed) PathPoints.Add(point);
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
        for(int i = Pig.Count - 1; i >= 0; i--)      
            Pig[i].ReturnToPool();

        for (int i = Orc.Count - 1; i >= 0; i--)
            Orc[i].ReturnToPool();
    }

    private void Action_OnPause(bool onPause)
    {
        Release();
        if(!onPause) _coroutine = StartCoroutine(SpawnProcess());
    }

    private void Point_OnUsedPoint(SpawnPoint point, bool active)
    {
        if (active) PathPoints.Remove(point);
        else PathPoints.Add(point);
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
            yield return new WaitWhile(() => Pig.Count >= _maxEnemyCount);
            yield return Interval;
            Spawn();
        }
    }

    private IEnumerator ChangeProcess()
    {
        while (Pig.Count != 0)
        {
            int i = Random.Range(0, Pig.Count);
            var pig = Pig[i] as EnemyBase;

            pig.Change();

            pig.Die -= Pig_Die;
            pig.Die += Orc_Die;
            Orc.Add(pig);
            Pig.Remove(pig);
            yield return Delay;

        }
        while(true)
        {
            yield return new WaitWhile(() => Orc.Count >= _maxEnemyCount);
            yield return Interval;

            var enemy = _enemyPool.Spawn(_spawnPosition);

            Orc.Add(enemy);
            enemy.Die += Orc_Die;
        }
    }

    private void Spawn()
    {
        var enemy = _enemyPool.Spawn(_spawnPosition);

        Pig.Add(enemy);
        enemy.Die += Pig_Die;
    }

    private void Change(bool value)
    {
        _isDangerousTime = value;

        Release();
        _coroutine = StartCoroutine(_isDangerousTime ? ChangeProcess() : SpawnProcess());
    }

    private void Pig_Die(PoolMember pig)
    {
        Pig.Remove(pig);
        pig.Die -= Pig_Die;
    }

    private void Orc_Die(PoolMember orc)
    {
        Orc.Remove(orc);
        orc.Die -= Orc_Die;
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
        int r = Random.Range(0, PathPoints.Count);
        return PathPoints[r].transform.position;
    }
}