using System;
using System.Collections;

public class StatePatrol : IState
{
    public event Action<IState> OnEndPatrol;

    private readonly EnemyBase Enemy;

    private const float _speed = 0.8f;

    public StatePatrol(EnemyBase enemy) => Enemy = enemy;

    public void Enter()
    {
        Enemy.Agent.isStopped = false;
        Enemy.Agent.speed = _speed;
        Enemy.Agent.SetDestination(Game.Locator.Spawner.GetPath());
        Enemy.Animator.SetFloat("Velocity", 0.5f);
    }

    public IEnumerator UpdateProcess()
    {
        while(Enemy.Agent.remainingDistance > 0.5f)     
            yield return null;
        
        OnEndPatrol?.Invoke(Enemy.StateIdle);
    }
}