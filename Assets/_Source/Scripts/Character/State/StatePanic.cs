using System.Collections;

public class StatePanic : IState
{
    private readonly EnemyBase Enemy;
    private const float _speed = 2f;
    public StatePanic(EnemyBase enemy) => Enemy = enemy;

    public void Enter()
    {
        Enemy.Agent.isStopped = false;
        Enemy.Agent.speed = _speed;
        Enemy.Animator.SetFloat("Velocity", 1f);
    }

    public IEnumerator UpdateProcess()
    {
        while(true)
        {
            while (Enemy.Agent.remainingDistance > 0.5f)
                yield return null;

            Enemy.Agent.SetDestination(Game.Locator.Spawner.GetPath());
            yield return null;
        }
    }
}